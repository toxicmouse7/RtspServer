using RtspServer.Abstract;
using RtspServer.Domain.Models;

namespace RtspServer.Services;

public class SessionService : ISessionService
{
    private readonly Dictionary<long, Session> _sessions = new();
    
    public Session CreateSession(long clientPort, string ip)
    {
        var session = new Session(clientPort, ip);
        _sessions.Add(session.Id, session);
        return session;
    }

    public Session GetSession(long sessionId)
    {
        return _sessions[sessionId];
    }
}