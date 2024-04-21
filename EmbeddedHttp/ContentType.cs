namespace CleckTech.EmbeddedHttp
{
    public class ContentType
    {
        public ContentType(string extension, string mimeType)
        {
            this.Extension = extension.StartsWith(".") ? extension : $".{extension}";
            this.MimeType = mimeType;
        }

        public static ContentType Binary => new ContentType("*", "application/octet-stream");
        public static ContentType Css => new ContentType(".css", "text/css");
        public static ContentType Gif => new ContentType(".gif", "image/gif");
        public static ContentType Html => new ContentType(".html", "text/html");
        public static ContentType Ico => new ContentType(".ico", "image/x-icon");
        public static ContentType JavaScript => new ContentType(".js", "text/javascript");
        public static ContentType JavaScriptModule => new ContentType(".mjs", "text/javascript");
        public static ContentType Jpeg => new ContentType(".jpeg", "image/jpeg");
        public static ContentType Jpg => new ContentType(".jpg", "image/jpeg");
        public static ContentType Json => new ContentType(".json", "application/json");
        public static ContentType Mp3 => new ContentType(".mp3", "audio/mpeg");
        public static ContentType Mp4 => new ContentType(".mp4", "video/mp4");
        public static ContentType Pdf => new ContentType(".pdf", "application/pdf");
        public static ContentType Png => new ContentType(".png", "image/png");
        public static ContentType Text => new ContentType(".txt", "text/plain");
        public static ContentType Xhtml => new ContentType(".xhtml", "application/xhtml+xml");
        public static ContentType Xml => new ContentType(".xml", "application/xml");
        public static ContentType Zip => new ContentType(".zip", "application/zip");

        public string Extension { get; }
        public string MimeType { get; }

        public override string ToString()
        {
            return $"Extension={this.Extension};MimeType={this.MimeType};";
        }
    }
}