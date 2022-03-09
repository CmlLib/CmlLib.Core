using System.Net.Http;

namespace CmlLib.Utils
{
    public class HttpUtil
    {
        public static HttpClient HttpClient { get; } = new HttpClient();
    }
}
