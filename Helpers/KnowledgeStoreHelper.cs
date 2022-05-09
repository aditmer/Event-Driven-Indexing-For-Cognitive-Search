using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Cosmos;

namespace Serverless.Indexer
{
	
	public class KnowledgeStoreHelper
	{
		private static CosmosClient cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("KS_Cosmos_Endpoint"), Environment.GetEnvironmentVariable("KS_Cosmos_Key"));

        // The database we will create
        private static Database database = cosmosClient.GetDatabase(Environment.GetEnvironmentVariable("KS_Cosmos_DB"));

        // The container we will create.
        private static Container container = database.GetContainer(Environment.GetEnvironmentVariable("KS_Cosmos_Container"));

		public async static Task UpsertDocumentAsync(Document doc)
		{
			await container.UpsertItemAsync<Document>(doc);
		}
        
	}
}