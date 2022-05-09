using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Azure.Cosmos;

namespace Serverless.Indexer
{
	
	public class SearchIndexHelper
	{
		private static string serviceName = Environment.GetEnvironmentVariable("Search_Service_Name");
		private static string indexName = Environment.GetEnvironmentVariable("Search_Index_Name");
		private static string apiKey = Environment.GetEnvironmentVariable("Search_Admin_Key");

		// Create a SearchIndexClient to send create/delete index commands
		private static Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
		private static AzureKeyCredential credential = new AzureKeyCredential(apiKey);
		private static SearchIndexClient adminClient = new SearchIndexClient(serviceEndpoint, credential);

		// Create a SearchClient to load and query documents
		private static SearchClient searchClient = new SearchClient(serviceEndpoint, indexName, credential);

		// Create index
		public static void CreateOrUpdateIndex(string indexName)
		{
			FieldBuilder fieldBuilder = new FieldBuilder();
			var searchFields = fieldBuilder.Build(typeof(Document));

			var definition = new SearchIndex(indexName, searchFields);

			var suggester = new SearchSuggester("sg", new[] { "Title", "Id", "KeyPhrases"});
			definition.Suggesters.Add(suggester);

			adminClient.CreateOrUpdateIndex(definition);
		}

		// Upload documents in a single Upload request.
		public static void UploadDocuments(Document doc)
		{
			IndexDocumentsBatch<Document> batch = IndexDocumentsBatch.Create(
				IndexDocumentsAction.Upload(doc));

			try
			{
				IndexDocumentsResult result = searchClient.IndexDocuments(batch);
			}
			catch (Exception ex)
			{
				// If for some reason any documents are dropped during indexing, you can compensate by delaying and
				// retrying. This simple demo just logs the failed document keys and continues.
				Console.WriteLine($"Failed to index some of the documents: {ex.Message}");
			}
		}
	}
}