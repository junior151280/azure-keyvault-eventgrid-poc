using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeyVaultAlertFunction
{
    public class LogAnalyticsAlerts
    {
        private static readonly HttpClient httpClient = new();

        public static async Task CreateAlert(string secretName, string message, ILogger log)
        {
            string workspaceId = Environment.GetEnvironmentVariable("LOG_ANALYTICS_WORKSPACE_ID");
            string sharedKey = Environment.GetEnvironmentVariable("LOG_ANALYTICS_SHARED_KEY");
            string logType = "azure-keyvault-eventgrid-poc"; // Nome do log personalizado

            var logData = new
            {
                Time = DateTime.UtcNow,
                Message = message,
                Level = "Error"
            };

            string jsonData = JsonConvert.SerializeObject(new[] { logData });
            string dateString = DateTime.UtcNow.ToString("r");
            string signature = GetSignature(jsonData, dateString, workspaceId, sharedKey);

            string url = $"https://{workspaceId}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", signature);
            request.Headers.Add("Log-Type", logType);
            request.Headers.Add("x-ms-date", dateString);
            request.Headers.Add("time-generated-field", "Time");

            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            log.LogInformation($"Log enviado. Status: {response.StatusCode}");
        }

        private static string GetSignature(string json, string date, string workspaceId, string sharedKey)
        {
            string stringToHash = $"POST\n{json.Length}\napplication/json\nx-ms-date:{date}\n/api/logs";
            byte[] keyBytes = Convert.FromBase64String(sharedKey);
            using var hasher = new System.Security.Cryptography.HMACSHA256(keyBytes);
            string hashedString = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToHash)));
            return $"SharedKey {workspaceId}:{hashedString}";
        }
    }
}
