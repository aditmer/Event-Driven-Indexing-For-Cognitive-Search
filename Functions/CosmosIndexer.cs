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

                //call enrichment pipeline (skillset)
                d.Languages = await TextAnalyticsHelper.DetectLanguageInput(d.Content);
                // d.Sentiments = await TextAnalyticsHelper.DetectedSentiment(d.Content);
                d.KeyPhrases = await TextAnalyticsHelper.DetectedKeyPhrases(d.Content);
                d.Entities = await TextAnalyticsHelper.DetectedEntities(d.Content);
                d.RedactedText = await TextAnalyticsHelper.RedactedText(d.Content);

                var summary = await TextAnalyticsHelper.ExtractSummaryResultsAsync(p.Parse.Text.Content);
                d.Summary = "";
                foreach(SummarySentence s in summary)
                {
                    d.Summary += s.Text + " ...\n\n";
                }

                //knowledge store
                await KnowledgeStoreHelper.UpsertDocumentAsync(d);

                //upsert into search index
                SearchIndexHelper.CreateOrUpdateIndex("wikipedia");
                SearchIndexHelper.UploadDocuments(d);
            }
        }
    }
}
