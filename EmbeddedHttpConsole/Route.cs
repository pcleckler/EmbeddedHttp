using CleckTech.EmbeddedHttp;

namespace CleckTech.EmbeddedHttpConsole
{
    internal class Route : IRoute
    {
        public Route(string regEx, RouteHandler handler)
        {
            this.RegEx = regEx;
            this.Handler = handler;
        }

        public RouteHandler Handler { get; }

        public string RegEx { get; }
    }
}