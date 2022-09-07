# Use an event-driven trigger for indexing in Azure Cognitive Search

This C# sample is an Azure Function app that demonstrates event-driven indexing in Azure Cognitive Search. If you've used indexers and skillsets in the past, you know that indexers can run on demand or on a schedule, but not in response to events like data updates. This demo shows you an equivalent approach to indexer-based indexing and skillset-based enrichment. It does so by providing event-detection and making direct calls to Cognitive Services and Cognitive Search.

This demo is based on a [serverless event-based architecture with Azure Cosmos DB and Azure Functions](https://docs.microsoft.com/azure/cosmos-db/sql/change-feed-functions). The workflow of this demo is as follows:

+ A function app listens for events in an Azure data source. This demo provides two functions, one for watching a Cosmos DB database, and another for watching an Azure blob container.

+ When a data update is detected, the function app starts an indexing and enrichment process:

  + First, the app calls Cognitive Services to enrich the content. It uses the Computer Vision OCR API to "crack" the document and find text. It then calls [Azure Cognitive Service for Language](https://docs.microsoft.com/azure/cognitive-services/language-service/overview) for language detection, entity recognition, and sentiment analysis. The enriched output is stored in a new blob container in Azure Storage.

  + Second, the app makes an indexing call to Azure Cognitive Search, indexing the enriched content created in the previous step. The search index in Azure Cognitive Search is updated to include the new information. The demo uses the [**Azure.Search.Documents**](https://www.nuget.org/packages/Azure.Search.Documents/) library from the Azure SDK for .NET to create, load, and query the search index.

To trigger this workflow, the demo makes a REST call to Wikipedia to grab a page in JSON and adds it to a Cosmos DB. This is the event that starts the workflow. When all processing is complete, you should be able to query a search index for content from the page.

## Prerequisites

+ [Visual Studio Code](https://code.visualstudio.com/download), with a [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and .NET Core tools
+ [Azure Cognitive Search](https://docs.microsoft.com/azure/search/search-create-service-portal), Basic or above
+ [Azure Cognitive Services](https://docs.microsoft.com//azure/cognitive-services/cognitive-services-apis-create-account), multi-service and multi-region
+ [Azure Cosmos DB (SQL API)](https://docs.microsoft.com/azure/cosmos-db/sql/how-to-create-account), with a database and container named "trigger-indexing-demo"
+ [Azure Storage](https://docs.microsoft.com/azure/storage/common/storage-account-create), with a blob container named "trigger-indexing-demo"
+ An [Azure Function app](https://docs.microsoft.com/azure/azure-functions/functions-create-function-app-portal#create-a-function-app), with a runtime stack of .NET 6 on a Windows operating system. The following screenshot illustrates the configuration.

  :::image type="content" source="readme-images/create-function-app.png" alt-text="Screenshot of the create function app.":::

## Objects created in this demo

This demo creates the following assets in your Azure resources.

+ In Azure Functions, a function app containing two functions:
  + BlobIndexer function that triggers enrichment and indexing when content is added to blob container ("pages")
  + CosmosIndexer function that triggers enrichment and indexing when content is added to blob container ("pdfs")

+ In Azure Storage, one container for input data ("pdfs") and a second container for enriched output produced by Cognitive Services ("knowledge-store")

+ In Cosmos DB, a "Wikipedia" database with one container for input data ("pages") and  a second container for workflow state ("leases")

+ In Azure Cognitive Search, a search index, based on the schema provided in DocumentModel class and the name provided in the JSON settings file

## Run the project locally

1. Clone the sample repo or download a ZIP of its contents to get the demo code.

1. Start Visual Studio Code and open the folder containing the project.

1. Modify **local.settings.json** to include the connection information needed to run the app. The following section tells you where to find the values.

Use the Azure portal to find the connection information.

+ For "AzureWebJobsStorage", navigate to your function app. Get the "AzureWebJobsStorage" connection string from the **Configuration** page.

+ For "serverlessindexing_DOCUMENTDB", navigate to your Cosmos DB account. Get the full connection string from the **Keys** page.

+ For "Cog_Service_Key" and "Cog_Service_Endpoint", navigate to your Cognitive Services multi-region account. Get the key and endpoint from the **Keys and Endpoint** page.

+ For "KS_Cosmos_Endpoint", "KS_Cosmos_Key", "KS_Cosmos_DB", and "KS_Cosmos_Container", get the individual values from **Data Explorer** and the **Keys** page. 

+ For "Search_Service_Name" and "Search_Admin_Key", navigate to your search service. Get the full URL endpoint from the **Overview** page. Get the admin API key from the **Keys** page. This demo creates a search index on your search service using the "Search_Index_Name" that you provide in settings.

## Run the sample

Press F5 to run the sample locally.

## Deploy to Azure

Optionally, if you'd like to switch from local to cloud-based processing, transfers connection information from **local.settings.json** to **appsettings.json** and deploy the app. 

1. Fill in **appsettings.json** with the connection information necessary to reach all Azure resources. The entries are similar to local.settings.json, but these entries will be read by your app once it's deployed on Azure.

1. In Visual Studio Code, right-click the function app and select **Deploy to Azure**.

This demo uses hard-code access keys and connection strings for the connection. We strongly recommend that you either encrypt the keys using Azure Key Vault, or use Azure Active Directory for authentication and authorized access.

## Explore the code

TO DO
