﻿using System.Net;
using RtspServer.Domain.Models.Abstract;

namespace RtspServer.Domain.Models.RtspRequests;

public record RtspOptionsRequest(IReadOnlyDictionary<string, string> Headers, IPAddress Address) 
    : RtspRequest(Headers, Address);