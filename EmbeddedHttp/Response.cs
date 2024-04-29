using System.Text;

namespace CleckTech.EmbeddedHttp
{
    public class Response : IResponse
    {
        public Response(ContentType contentType, byte[] data)
        {
            this.ContentType = contentType;
            this.Data = data;
        }

        public Response(ContentType contentType, string data)
        {
            this.ContentType = contentType;
            this.Data = Encoding.UTF8.GetBytes(data);
        }

        public ContentType ContentType { get; private set; }

        public byte[] Data { get; private set; }
    }
}