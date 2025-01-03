﻿using AutoMapper;
using ManagementServer.Application.Commands;
using ManagementServer.Application.Queries;
using ManagementServer.Domain.Abstract;
using ManagementServer.Domain.Models;
using ManagementServer.Hubs;
using ManagementServer.Settings;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using RtspServer.Application.Queries;
using RtspServer.Domain.Models.Rtp;
using RtspServer.Domain.Models.Sessions;

namespace ManagementServer.Domain;

public class FuzzingService : IFuzzingService
{
    private readonly ILogger<FuzzingService> _logger;
    private readonly IHubContext<FuzzingHub> _hubContext;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly Dictionary<RtspSession, CancellationTokenSource> _stoppingTokens = new();

    public FuzzingService(
        IHubContext<FuzzingHub> hubContext,
        ILogger<FuzzingService> logger,
        IMapper mapper,
        ISender sender)
    {
        _hubContext = hubContext;
        _logger = logger;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<RtpFuzzingPreset> AddRtpPresetAsync(RtpFuzzingPreset preset)
    {
        var addPresetCommand = new AddRtpPresetCommand(preset);
        return await _sender.Send(addPresetCommand);
    }

    public async Task<IEnumerable<RtpFuzzingPreset>> GetAllPresetsAsync()
    {
        var getPresetsQuery = new GetFuzzingPresetsQuery();
        var presets = await _sender.Send(getPresetsQuery);

        return presets;
    }

    public async Task RemovePresetAsync(Guid presetId)
    {
        var removePresetCommand = new RemoveRtpPresetCommand(presetId);
        await _sender.Send(removePresetCommand);
    }

    public async Task StartFuzzingAsync(long sessionId)
    {
        var getPresetsQuery = new GetFuzzingPresetsQuery();
        var presets = await _sender.Send(getPresetsQuery);

        var session = await _sender.Send(new GetSessionQuery(sessionId));
        if (session is null)
        {
            await _hubContext.Clients.All.SendAsync("Error");
            return;
        }
        
        var cts = CancellationTokenSource.CreateLinkedTokenSource(session.Token);
        _stoppingTokens.Add(session, cts);
        
        var totalToSend = presets.Sum(p => p.RawFuzzingData.Count);
        var sent = 0;

        await _hubContext.Clients.All.SendAsync("PreFuzz", totalToSend, cancellationToken: cts.Token);
        await _hubContext.Clients.All.SendAsync("PacketSent", sent, cancellationToken: cts.Token);
        
        foreach (var preset in presets)
        {
            foreach (var rawData in preset.RawFuzzingData)
            {
                try
                {
                    if (rawData.RawData.Length < 20)
                    {
                        continue;
                    }

                    var fuzzingPacket = RtpPacket.Deserialize(rawData.RawData);

                    var appendPacketCommand = new AppendRtpPacketCommand(
                        session.Id,
                        fuzzingPacket,
                        new AppendSettings
                        {
                            UseOriginalPayload = fuzzingPacket.Content.Length == 0,
                            UseOriginalTimestamp = false,
                            UseOriginalSequence = false
                        });
                    await _sender.Send(appendPacketCommand, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                }
                catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
                {
                    await _hubContext.Clients.All.SendAsync("Error", CancellationToken.None);
                    break;
                }
                catch
                {
                    
                }

                sent++;
                await _hubContext.Clients.All.SendAsync("PacketSent", sent, CancellationToken.None);
                await Task.Delay(TimeSpan.FromSeconds(0.2), CancellationToken.None);
            }
        }

        _stoppingTokens.Remove(session, out var token);
        token?.Dispose();
    }

    public void StopFuzzing(RtspSession session)
    {
        if (!_stoppingTokens.Remove(session, out var cts))
        {
            return;
        }
        
        cts.Cancel();
        cts.Dispose();
        _logger.LogDebug("Fuzzing stopped. Session id: {sessionId}", session.Id);
    }
}