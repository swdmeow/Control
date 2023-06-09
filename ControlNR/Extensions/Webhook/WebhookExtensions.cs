using Exiled.API.Features;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static Mono.Security.X509.X520;

// Повзаимствовано у https://github.com/XLEBYSHEK003/XLEB_Utils2/blob/master/XLEB_Utils2/Webhook/Webhook.cs 
namespace Control.Extensions
{
    public class WebhookExtensions
    {
        public static void SendMessage(string URL, string content)
        {
            var message = new
            {
                content = content,
            };

            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(message);

            var response = client.PostAsync(
            URL, new StringContent(json, Encoding.UTF8, "application/json")).Result;

            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Невозможно отправить вебхук");
            }
        }
    }
}