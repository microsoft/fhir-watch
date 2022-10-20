@description('Secret that allows connection to Dataverse')
@secure()
param dataverseClientSecret string

param tagVersion string = 'fw-version:v1.0.0'
param location string = resourceGroup().location
param prefix string = 'fhirwatch'
param repositoryUrl string = 'https://github.com/microsoft/fhir-watch'
param repositoryBranch string = 'main'

var tagName = split(tagVersion, ':')[0]
var tagValue = split(tagVersion, ':')[1]

module apiApp 'functionapp.bicep' = {
  name: 'FhirWatchAPI'
  params: {
    location: location
    prefix: prefix
    tagVersion: tagVersion
    dataVerseClientSecret: dataverseClientSecret
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
  tags: {
    '${tagName}': tagValue
  }
  properties: {
    repositoryUrl: repositoryUrl
    branch: repositoryBranch
    provider: 'GitHub'
  }
}

resource linkedApi 'Microsoft.Web/staticSites/linkedBackends@2022-03-01' = {
  name: 'backend1'
  parent: clientApp
  properties: {
    backendResourceId: apiApp.outputs.functionAppProps.functionAppId
    region: location
  }
}

output swaProperties object = {
  appName: clientApp.name
  hostname: clientApp.properties.defaultHostname
}
output functionAppName string = apiApp.outputs.functionAppProps.functionAppName
output functionAppId string = apiApp.outputs.functionAppProps.functionAppId
