using ServerValutaManager;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        EveryDayAddDB.FillDBNewData();

        HttpListener httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:8080/");
        httpListener.Start();

        while (true)
        {
            var requestContext = httpListener.GetContext();

            if (requestContext.Request.HttpMethod == "POST")
            {
                var contentRequest = requestContext.Request;
                var bodyRequest = GetRequestData.GetRequestPostData(contentRequest);
                if (bodyRequest.Contains("first"))
                {
                    var responseBody = new Regex(@"[A-Z]{3}|[\d\/]{8,10}").Matches(bodyRequest);
                    string firstValuta = responseBody[0].Value;
                    string secondValuta = responseBody[1].Value;
                    string[] finalValue = CalculateCourse.ReturnCurse(firstValuta, secondValuta, Convert.ToDateTime(responseBody[2].Value));

                    requestContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    requestContext.Response.AppendHeader("Access-Control-Allow-Headers", "*");
                    requestContext.Response.AppendHeader("Access-Control-Allow-Methods", "*");
                    requestContext.Response.StatusCode = 200;
                    var stream = requestContext.Response.OutputStream;
                    var bytes = Encoding.UTF8.GetBytes($"{finalValue[0]}&{finalValue[1]}");
                    stream.Write(bytes, 0, bytes.Length);
                    requestContext.Response.Close();
                }
                else if (bodyRequest.Contains("GetActualValuta"))
                {
                    var responseBody = new Regex(@"(?<=:)[\d]{2}|[\d\/]{8,10}").Matches(bodyRequest);
                    string ActualValuta = CalculateCourse.GetAllCurseNowDay(Convert.ToInt32(responseBody[0].Value), Convert.ToDateTime(responseBody[1].Value));

                    requestContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                    requestContext.Response.AppendHeader("Access-Control-Allow-Headers", "*");
                    requestContext.Response.AppendHeader("Access-Control-Allow-Methods", "*");
                    requestContext.Response.StatusCode = 200;
                    var stream = requestContext.Response.OutputStream;
                    var bytes = Encoding.UTF8.GetBytes(ActualValuta);
                    stream.Write(bytes, 0, bytes.Length);
                    requestContext.Response.Close();
                }
            }
            else
            {
                requestContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                requestContext.Response.AppendHeader("Access-Control-Allow-Headers", "*");
                requestContext.Response.AppendHeader("Access-Control-Allow-Methods", "*");
                requestContext.Response.StatusCode = 200;
                var stream = requestContext.Response.OutputStream;
                var bytes = Encoding.UTF8.GetBytes("Метод предварительного просмотра");
                stream.Write(bytes, 0, bytes.Length);
                requestContext.Response.Close();
            }
        }
        httpListener.Stop();

        httpListener.Close();
    }
}