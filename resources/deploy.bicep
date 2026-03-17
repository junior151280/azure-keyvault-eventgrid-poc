param location string = 'eastus'
param storageAccountName string = 'mystoragefunc123'
param functionAppName string = 'KeyVaultAlertFunction'
param keyVaultName string = 'MyKeyVaultAlert'
param sendGridApiKey string

resource storage 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
  }
}

resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', functionAppName)
    siteConfig: {
      appSettings: [
        { name: 'FUNCTIONS_EXTENSION_VERSION', value: '~4' }
        { name: 'AzureWebJobsStorage', value: storage.properties.primaryEndpoints.blob }
        { name: 'SendGridApiKey', value: sendGridApiKey }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource eventGrid 'Microsoft.EventGrid/systemTopics@2021-06-01-preview' = {
  name: 'KeyVaultEventGrid'
  location: location
  properties: {
    source: keyVault.id
    topicType: 'Microsoft.KeyVault.Vaults'
  }
}

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'SecretExpiryAlert'
  parent: eventGrid
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: functionApp.properties.hostNames[0] // Conectar com a Function
      }
    }
    filter: {
      includedEventTypes: ['Microsoft.KeyVault.SecretNearExpiry']
    }
  }
}
