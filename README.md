# Use an event-driven trigger for indexing in Azure Cognitive Search

This C# sample is an Azure Function app that demonstrates how to implement event-driving indexing in Azure Cognitive Search. If you've run into limitations using the indexer and skillset features of Azure Cognitive Search, this demo shows you an alternative for achieving similar outcomes:

+ Indexers are schedule-based, whereas this demo triggers indexing from an event. In this example, the event is data ingestion into a source database,

+ Indexing (creating and loading a search index) uses the push APIs of Cognitive Search. Indexing through a push API eliminates constraints around frequency, duration, and volume.

+ If your scenario requires AI enrichment, you can replace a skillset by making direct calls to Cognitive Services. To illustrate this technique, the demo invokes Computer Vision and Text Analytics for sentiment analysis.

## Objects created in this demo

The demo creates two functions, deployed to an Azure Function app configured for .NET. It also creates a search index in Azure Cognitive Search.

+ A CosmosIndexer function that triggers search indexing from your Cosmos DB database
+ A BlobIndexer function that triggers search indexing from a blob container
+ A search index based on the schema provided in DocumentModel.cs.

## Prerequisites

+ Visual Studio Code, with a C# extension and .NET Core
+ Azure.Search.Documents library from the Azure SDK for .NET
+ Azure Cognitive Search, Basic or above
+ Azure Cognitive Services, multi-region
+ Azure Cosmos DB (SQL API), with a database and container named "trigger-indexing-demo"
+ Azure Storage, with a blob container named "trigger-indexing-demo"
+ An Azure Function app, with a runtime stack of .NET 6 on a Windows operating system

  :::image type="content" source="readme-images/create-function-app.png" alt-text="Screenshot of the create function app":::

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


Sample data?

## Run the sample

TO DO

## Explore the code

### Trigger indexing based on data updates

TO DO

### Trigger applied AI

TO DO

