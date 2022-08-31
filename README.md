# Use an event-driven trigger for indexing in Azure Cognitive Search

This C# sample is an Azure Function app that demonstrates how to implement event-driving indexing in Azure Cognitive Search. If you've ever faced limitations using the indexer and skillset features of Azure Cognitive Search, this demo shows you an alternative for achieving similar outcomes for two related scenarios.

**Scenario 1: Event-driven indexing**

Indexers run on demand or on a schedule, pulling data from a specific set of supported data sources. In contrast, this example shows you how to implement event-driven indexing, triggered by infusion of data in the source database. Because this solution uses the push APIs of Cognitive Search, any indexer-related constraints around frequency, duration, volume, and data platforms do not apply.

The function app you'll create in this sample monitors a container for updates. When detected, the app initiates a full end-to-end process that includes enrichment and indexing.

**Scenario 2: Applied AI for transformations on content**

Indexers drive the native AI enrichment capabilities of Cognitive Search through skillsets. If your scenario requires AI enrichment but you can't use indexers, you can replace a skillset by making direct calls to Cognitive Services. To illustrate this technique, the demo invokes the following resources:

+ [Azure Cognitive Service for Language](https://docs.microsoft.com/azure/cognitive-services/language-service/overview) for language detection, entity recognition, and sentiment analysis.

+ [OCR Read API](https://docs.microsoft.com/azure/cognitive-services/computer-vision/overview-ocr) in [Computer Vision](https://docs.microsoft.com/azure/cognitive-services/computer-vision/overview) to recognize and read strings from PDFs.

This sample stores the finished output in a Cosmos DB database. 

Indexing operates over the stored data. The sample uses the [**Azure.Search.Documents**](https://www.nuget.org/packages/Azure.Search.Documents/) library from the Azure SDK for .NET to create, load, and query the search index.

## Objects created in this demo

This demo creates the following assets in your Azure resources.

+ A function app containing two functions:
  + BlobIndexer triggers search indexing when content is added to blob container ("pages")
  + CosmosIndexer function that triggers search indexing from your Cosmos DB database. Cosmos DB databases are also used in this workflow to store input and output data.

+ Storage for input data ("pages") and for enriched output produced by Cognitive Services ("knowledgeStore").

+ A search index based on the schema provided in DocumentModel class is created in your search service.

## Prerequisites

+ [Visual Studio Code](https://code.visualstudio.com/download), with a [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and .NET Core
+ [Azure Cognitive Search](https://docs.microsoft.com/azure/search/search-create-service-portal), Basic or above
+ [Azure Cognitive Services](https://docs.microsoft.com//azure/cognitive-services/cognitive-services-apis-create-account), multi-service and multi-region
+ [Azure Cosmos DB (SQL API)](https://docs.microsoft.com/azure/cosmos-db/sql/how-to-create-account), with a database and container named "trigger-indexing-demo"
+ [Azure Storage](https://docs.microsoft.com/azure/storage/common/storage-account-create), with a blob container named "trigger-indexing-demo"
+ An [Azure Function app](https://docs.microsoft.com/azure/azure-functions/functions-create-function-app-portal#create-a-function-app), with a runtime stack of .NET 6 on a Windows operating system. The following screenshot illustrates the configuration.

  :::image type="content" source="readme-images/create-function-app.png" alt-text="Screenshot of the create function app.":::

## Workflow

Event-based indexing is driven by the function app. The function app watches the "pages" container. When a document is added or updated, the event triggers the function to run.  

The "knowledgestore" container is where the enriched documents are collected, after calling Cognitive Services, and before sending it to the search index.

## Set up demo data

This sample uses content from Wikipedia. If you want to run this sample end-to-end, you'll need to create a Cosmos DB database, populated with JSON documents from Wikipedia.

1. Download "TBD" sample data from [https://github.com/Azure-Samples/azure-search-sample-data](https://github.com/Azure-Samples/azure-search-sample-data).
1. In Cosmos DB, create a new database named "trigger-indexing-demo-db" with a container named "???"
1. In Data Explorer, select the container, select **Items**, and select **Upload Item** from the command bar.
1. Upload the "TBD" JSON file.

## Run the project locally

Modify **local.settings.json** to include the connection information needed to run the app. This section explains where to obtain the values used in the settings JSON file.

1. Sign in to Azure portal.
1. For "AzureWebJobsStorage", navigate to your function app. Get the "AzureWebJobsStorage" connection string from the **Configuration** page.
1. For "serverlessindexing_DOCUMENTDB", navigate to your Cosmos DB account. Get the full connection string from the **Keys** page.
1. For "Cog_Service_Key" and "Cog_Service_Endpoint", navigate to your Cognitive Services multi-region account. Get the key and endpoint from the **Keys and Endpoint** page.
1. For "KS_Cosmos_Endpoint", "KS_Cosmos_Key", "KS_Cosmos_DB", and "KS_Cosmos_Container", get the individual values from **Data Explorer** and the **Keys** page. 
1. For "Search_Service_Name" and "Search_Admin_Key", navigate to your search service. Get the full URL endpoint from the **Overview** page. Get the admin API key from the **Keys** page. This demo creates a search index on your search service using the "Search_Index_Name" that you provide.

## Run the sample

TO DO

## Deploy to Azure

1. Fill in **appsettings.json** with the connection information necessary to reach all Azure resources. The entries are similar to local.settings.json, but these entries will be read by your app once it's deployed on Azure.

1. In Visual Studio Code, right-click the function app and select **Deploy to Azure**.

## Explore the code

TO DO

