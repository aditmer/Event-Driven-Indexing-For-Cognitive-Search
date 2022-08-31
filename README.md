# Use an event-driven trigger for indexing in Azure Cognitive Search

This C# sample is an Azure Function app that demonstrates how to implement event-driving indexing in Azure Cognitive Search. If you've run into limitations using the indexer and skillset features of Azure Cognitive Search, this demo shows you an alternative for achieving similar outcomes:

+ Indexers pull data from supported data sources on demand or on a schedule. In contrast, this example uses the push APIs of Cognitive Search, eliminating constraints around frequency, duration, volume, and platform. In this example, event-driven indexing is triggered by data updates in the source database.

+ Indexers drive the native AI enrichment capabilities of COgnitive Search. If your scenario requires AI enrichment but you can't use indexers, you can replace a skillset by making direct calls to Cognitive Services. To illustrate this technique, the demo invokes Text Analytics for language detection, entity recognition, and sentiment analysis. It also invokes Computer Vision for ???

## Objects created in this demo

This demo creates the following assets in your Azure resources.

+ A function app containing two functions:
  + BlobIndexer triggers search indexing when content is added to blob container ("pages")
  + CosmosIndexer function that triggers search indexing from your Cosmos DB database. Cosmos DB databases are also used in this workflow to store input and output data.

+ Storage for input data ("pages"), enriched content produced by Cognitive Services ("knowledgeStore").

+ A search index based on the schema provided in DocumentModel class is created in your search service.

## Prerequisites

+ Visual Studio Code, with a C# extension and .NET Core
+ Azure.Search.Documents library from the Azure SDK for .NET
+ Azure Cognitive Search, Basic or above
+ Azure Cognitive Services, multi-region
+ Azure Cosmos DB (SQL API), with a database and container named "trigger-indexing-demo"
+ Azure Storage, with a blob container named "trigger-indexing-demo"
+ An Azure Function app, with a runtime stack of .NET 6 on a Windows operating system. The following screenshot illustrates the configuration.

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

