using System;
using System.Net;

namespace CleckTech.EmbeddedHttp
{
    public interface IRoute
    {
        RouteHandler Handler { get; }
        string RegEx { get; }
    }
}