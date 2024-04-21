using CleckTech.EmbeddedHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CleckTech.EmbeddedHttpConsole
{
    internal class Program
    {
        private static void Main()
        {
            var server = new EmbeddedHttp.EmbeddedHttp(
                httpPrefixes: new List<Uri>()
                {
                    new Uri("http://localhost:8080/EmbeddedHttpConsole/")
                },
                routes: new List<IRoute>()
                {
                    new Route("/data", context => new Response(ContentType.Text, Encoding.UTF8.GetBytes("This is some data")))
                },
                rootDirectory: Path.Combine(Environment.CurrentDirectory, "www"),
                logger: new Logger());

            server.Start();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}