using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultAlertFunction.Models;

namespace KeyVaultAlertFunction
{

    public static class KeyVaultSecretExpiration
    {
        
        [FunctionName("KeyVaultSecretExpiration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processou uma solicitação.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<KeyVaultSecretEvents>(requestBody);

            if (data == null)
            {
                return new BadRequestObjectResult("Corpo de requisição inválido.");
            }

            // Monta a URL do Key Vault
            string secretName = data.ObjectName;
            string vaultName = data.VaultName;
            string vaultUrl = $"https://{vaultName}.vault.azure.net/secrets/{secretName}?api-version=7.3";

            // Cria o cliente do Key Vault
            var secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());

            try
            {
                // Obtém o Secret
                KeyVaultSecret secret = await secretClient.GetSecretAsync(data.ObjectName, data.Version);
                string responseMessage = $"SecretName: {secret.Name}, Value: {secret.Value}";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Erro ao obter secret do Key Vault");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
