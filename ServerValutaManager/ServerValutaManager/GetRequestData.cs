using System.Net;


namespace ServerValutaManager
{
    internal class GetRequestData
    {
        /// <summary>
        /// Получить тело POST-запроса
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (Stream body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
