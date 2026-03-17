$KEYVAULT_NAME = "kv-demo-secrets"
$FUNCTION_NAME = "KeyVaultSecretExpirationHandler"
$FUNCTIONS_APP_NAME = "functions-demo-secrets"
$FUNCTIONS_APP_PLAN_NAME = "MinhaAzureFunctionAppPlan"
$FUNCTIONS_APP_PLAN_SKU = "S1"
$FUNCTIONS_APP_PLAN_TIER = "Standard"
$RESOURCE_GROUP_NAME = "rg-demo-secrets"
$LOCATION = "brazilsouth"
$SUBSCRIPTION_ID = "cad9e0b6-be6e-4cb0-a4bc-3f81708540f9"

az keyvault secret set --vault-name $KEYVAULT_NAME --name "MinhaSecret01" --value "valorSecreto" --expires "2025-03-16T14:33:00Z"

az login

az group create --resource-group $RESOURCE_GROUP_NAME --location $LOCATION

az keyvault create --name $KEYVAULT_NAME --resource-group $RESOURCE_GROUP_NAME --location $LOCATION

az keyvault secret set --vault-name $KEYVAULT_NAME --name "MinhaSecret" --value "Teste123"

az keyvault set-policy --name $KEYVAULT_NAME --resource-group $RESOURCE_GROUP_NAME --event-hub-permissions Listen

az eventgrid event-subscription create \
    --name KeyVaultSecretExpiration \
    --source-resource-id /subscriptions/$SUBSCRIPTION_ID/resourceGroups/MeuResourceGroup/providers/Microsoft.KeyVault/vaults/$KEYVAULT_NAME \
    --endpoint-type azurefunction \
    --endpoint /subscriptions/$SUBSCRIPTION_ID/resourceGroups/MeuResourceGroup/providers/Microsoft.Web/sites/$FUNCTIONS_APP_NAME/functions/$FUNCTION_NAME
