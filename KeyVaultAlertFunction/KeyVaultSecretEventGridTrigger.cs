using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KeyVaultAlertFunction.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace KeyVaultAlertFunction
{
    public static class KeyVaultSecretEventGridTrigger
    {
        private static readonly TelemetryClient telemetryClient = new(new TelemetryConfiguration(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")));

        [FunctionName("KeyVaultSecretEventGridTrigger")]
        public static async Task Run(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation("C# Event Grid trigger function processed a request.");

            var data = JsonConvert.DeserializeObject<KeyVaultSecretEvents>(eventGridEvent.Data.ToString());
            if (data == null)
            {
                log.LogError("Body inválido.");
                return;
            }

            string vaultUrl = $"https://{data.VaultName}.vault.azure.net/";
            var secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());

            try
            {
                string sOwner = string.Empty;
                string sEmail = string.Empty;
                string message = string.Empty;
                KeyVaultSecret secret = await secretClient.GetSecretAsync(data.ObjectName, data.Version);

                if (secret.Properties.Tags.TryGetValue("owner", out var owner))
                {
                    sOwner = owner;
                }

                if (secret.Properties.Tags.TryGetValue("email", out var email))
                {
                    sEmail = email;
                }
                message = $"SecretName: {secret.Name}; Owner: {sOwner}; E-mail: {sEmail}";
                log.LogInformation(message);

                telemetryClient.TrackEvent("azure-keyvault-eventgrid-poc", new System.Collections.Generic.Dictionary<string, string>
                {
                    { "Severity", "High" },
                    { "Message", message }
                });

                telemetryClient.Flush();
                await LogAnalyticsAlerts.CreateAlert(secret.Name, message, log);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Erro ao obter secret do Key Vault");
            }
        }
    }
}
