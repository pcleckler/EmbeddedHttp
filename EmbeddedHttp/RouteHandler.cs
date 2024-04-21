using System.Net;

namespace CleckTech.EmbeddedHttp
{
    public delegate IResponse RouteHandler(HttpListenerContext context);
}