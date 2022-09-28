param location string
param prefix string

var suffix = uniqueString(resourceGroup().id)
var functionappname = '${prefix}${suffix}-Api'
var storagenamebuilder = 'sa${toLower(prefix)}${suffix}'
var storagename = length(storagenamebuilder) > 24 ? take(storagenamebuilder, 24) : storagenamebuilder

resource functionAppSA 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storagename
  location: location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'    
  }
}

resource functionAppSP 'Microsoft.Web/serverfarms@2022-03-01' = {
  location: location
  name: '${functionappname}-Server'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'    
  }
  kind: 'functionapp'
}

resource functionAppSite 'Microsoft.Web/sites@2022-03-01' = {
  name: functionappname
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppSP.id
    httpsOnly: true 
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storagename};EndpointSuffix=${environment().suffixes.storage};AccountKey=${functionAppSA.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storagename};EndpointSuffix=${environment().suffixes.storage};AccountKey=${functionAppSA.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionappname)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
      ]
      netFrameworkVersion: 'v6.0'
    }   
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  location: location
  name: 'Insights-${functionappname}'
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

output functionappid string = functionAppSite.id
