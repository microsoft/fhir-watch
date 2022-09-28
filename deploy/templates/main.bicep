param location string = resourceGroup().location
param prefix string = 'fhirwatch'
param repositoryUrl string = 'https://github.com/microsoft/fhir-watch'
param repositoryBranch string = 'main'

module apiApp 'functionapp.bicep' = {
  name: 'FhirWatchAPI'
  params: {
    location: location
    prefix: prefix
  }
}

var suffix = uniqueString(resourceGroup().id)
var clientappname = '${prefix}${suffix}-Client'

resource clientApp 'Microsoft.Web/staticSites@2022-03-01' = {
  location: location
  name: clientappname
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    repositoryUrl: repositoryUrl
    branch: repositoryBranch
    provider: 'GitHub'
  }
}

resource linkedApi 'Microsoft.Web/staticSites/linkedBackends@2022-03-01' = {
  name: '${clientappname}/backend1'
  properties: {
    backendResourceId: apiApp.outputs.functionappid
    region: location
  }
}

resource linkedFunctionApp 'Microsoft.Web/staticSites/userProvidedFunctionApps@2022-03-01' = {
  name: '${clientappname}/backend1'
  properties: {
    functionAppResourceId: apiApp.outputs.functionappid
    functionAppRegion: location
  }
}
