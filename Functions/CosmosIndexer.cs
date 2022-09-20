using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serverless.Indexer
{
    public static class CosmosIndexer
    {
        [FunctionName("CosmosIndexer")]
        public async static Task Run([CosmosDBTrigger(
            databaseName: "Wikipedia",
            containerName: "pages",
            Connection = "serverlessindexing_DOCUMENTDB",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]IReadOnlyList<ParseResults> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                ParseResults p = input[0];

                Document d = new Document();
                d.Id = p.Id;
                d.Content = p.Parse.Text.Content;
                d.Title = p.Parse.Title;
                d.Source = "cosmos";

                // Call Cognitive Services for enrichment (skillset replacement)
                d.KeyPhrases = await TextAnalyticsHelper.DetectedKeyPhrases(d.Content);
                d.Languages = await TextAnalyticsHelper.DetectLanguageInput(d.Content);
                d.Entities = await TextAnalyticsHelper.DetectedEntities(d.Content);
                // d.Sentiments = await TextAnalyticsHelper.DetectedSentiment(d.Content);
                // d.RedactedText = await TextAnalyticsHelper.RedactedText(d.Content);

                var summary = await TextAnalyticsHelper.ExtractSummaryResultsAsync(d.Content);
                d.Summary = "";
                foreach(SummarySentence s in summary)
                {
                    d.Summary += s.Text + " ...\n\n";
                }

                // Store enriched output in Cosmos DB (KnowledgeStore database)
                await KnowledgeStoreHelper.UpsertDocumentAsync(d);

                // Index enriched content in Cognitive Search
                SearchIndexHelper.CreateOrUpdateIndex("wikipedia-index");
                SearchIndexHelper.UploadDocuments(d);
            }
        }
    }
}
