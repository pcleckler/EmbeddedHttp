namespace CleckTech.EmbeddedHttp
{
    public class Response : IResponse
    {
        public Response(ContentType contentType, byte[] data)
        {
            this.ContentType = contentType;
            this.Data = data;
        }

        public ContentType ContentType { get; private set; }

        public byte[] Data { get; private set; }
    }
}