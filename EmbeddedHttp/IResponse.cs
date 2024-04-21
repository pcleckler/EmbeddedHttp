namespace CleckTech.EmbeddedHttp
{
    public interface IResponse
    {
        ContentType ContentType { get; }
        byte[] Data { get; }
    }
}