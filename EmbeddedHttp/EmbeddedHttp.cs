using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CleckTech.EmbeddedHttp
{
    public class EmbeddedHttp
    {
        private const string DefaultPage = "index.html";
        private const int ErrorSleep = 10;

        private readonly HttpListener listener;
        private readonly Task listenerTask;
        private readonly ILogger logger;
        private readonly string rootDirectory = null;
        private Dictionary<string, ContentType> contentTypes;
        private List<IRoute> routes;
        private bool stopRequested;

        public EmbeddedHttp(
            List<Uri> httpPrefixes,
            string rootDirectory = null,
            List<IRoute> routes = null,
            List<ContentType> contentTypes = null,
            ILogger logger = null)
        {
            this.rootDirectory = rootDirectory;

            this.logger = logger;

            this.listener = new HttpListener();

            foreach (var uri in httpPrefixes)
            {
                this.listener.Prefixes.Add(uri.ToString());
            }

            this.InitializeRoutes(routes);

            this.InitializeContentTypes(contentTypes);

            this.listenerTask = new Task(() =>
            {
                this.listener.Start();

                while (!this.stopRequested)
                {
                    try
                    {
                        this.listener.BeginGetContext(HandleConnection, null).AsyncWaitHandle.WaitOne();
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                        Thread.Sleep(ErrorSleep);
                    }
                }
            });
        }

        public List<ContentType> DefaultContentTypes => new List<ContentType>()
        {
            ContentType.Css,
            ContentType.Html,
            ContentType.Ico,
            ContentType.JavaScript,
            ContentType.JavaScriptModule,
            ContentType.Jpeg,
            ContentType.Jpg,
            ContentType.Png,
            ContentType.Json,
            ContentType.Text,
            ContentType.Gif,
            ContentType.Mp3,
            ContentType.Mp4,
            ContentType.Pdf,
            ContentType.Xhtml,
            ContentType.Xml,
            ContentType.Zip
        };

        public Encoding DefaultDecoding => Encoding.UTF8;

        public void Start()
        {
            this.listenerTask.Start();
        }

        public void Stop()
        {
            this.stopRequested = true;
        }

        private ContentType GetContentType(string localPath)
        {
            var contentType = ContentType.Binary;

            var extension = Path.GetExtension(localPath);

            if (this.contentTypes.ContainsKey(extension))
            {
                contentType = this.contentTypes[extension];
            }

            return contentType;
        }

        private void HandleConnection(IAsyncResult ar)
        {
            try
            {
                var context = this.listener.EndGetContext(ar);

                this.logger?.Info($"Processing request. Request URL:{context.Request.Url}, IP: {context.Request.RemoteEndPoint}, UserAgent: {context.Request.UserAgent}");

                foreach (var route in this.routes)
                {
                    if (Regex.IsMatch(context.Request.RawUrl, route.RegEx))
                    {
                        var response = route.Handler.Invoke(context);

                        this.WriteResponse(context, response);
                        return;
                    }
                }

                if (!this.HandleStaticRoute(context))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                    this.logger?.Info($"Handler for requested URL not found.");

                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
            }
        }

        private bool HandleStaticRoute(HttpListenerContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.rootDirectory))
                {
                    return false;
                }

                var localPath = context.Request.Url.LocalPath;

                if (localPath.StartsWith("/"))
                {
                    localPath = localPath.Substring(1);
                }

                if (localPath.Length < 1 || localPath == "/")
                {
                    localPath = DefaultPage;
                }

                localPath = Path.Combine(this.rootDirectory, localPath.Replace('/', Path.DirectorySeparatorChar));

                if (this.rootDirectory.Equals(localPath, StringComparison.OrdinalIgnoreCase))
                {
                    localPath = Path.Combine(localPath, DefaultPage);
                }

                if (Directory.Exists(localPath))
                {
                    localPath = Path.Combine(localPath, DefaultPage);
                }

                this.logger.Debug($"Mapped Request '{context.Request.Url}' to '{localPath}'");

                if (!File.Exists(localPath))
                {
                    return false;
                }

                this.WriteResponse(context, new Response(this.GetContentType(localPath), File.ReadAllBytes(localPath)));

                return true;
            }
            catch (Exception ex)
            {
                this.LogException(ex);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                throw ex;
            }
        }

        private void InitializeContentTypes(List<ContentType> contentTypes)
        {
            this.contentTypes = new Dictionary<string, ContentType>(StringComparer.OrdinalIgnoreCase);

            if (contentTypes == null)
            {
                contentTypes = this.DefaultContentTypes;
            }

            foreach (var contentType in contentTypes)
            {
                if (!this.contentTypes.ContainsKey(contentType.Extension))
                {
                    this.contentTypes.Add(contentType.Extension, contentType);
                }
            }

            if (!this.contentTypes.ContainsKey(ContentType.Binary.Extension))
            {
                this.contentTypes.Add(ContentType.Binary.Extension, ContentType.Binary);
            }
        }

        private void InitializeRoutes(List<IRoute> routes)
        {
            if (routes != null)
            {
                this.routes = routes;
            }
            else
            {
                this.routes = new List<IRoute>();
            }
        }

        private void LogException(Exception ex)
        {
            this.logger?.Error(ex.Message);

            var innerEx = ex.InnerException;

            while (innerEx != null)
            {
                this.logger?.Debug(innerEx.Message);

                innerEx = innerEx.InnerException;
            }

            this.logger?.Debug(ex.StackTrace);
        }

        private void WriteResponse(HttpListenerContext context, IResponse response)
        {
            context.Response.ContentType = response.ContentType.MimeType;
            context.Response.ContentLength64 = response.Data.Length;

            context.Response.OutputStream.Write(response.Data, 0, response.Data.Length);
            context.Response.OutputStream.Close();

            context.Response.StatusCode = (int)HttpStatusCode.OK;

            context.Response.Close();
        }
    }
}