using System;
using System.IO;
using System.Threading.Tasks;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serverless.Indexer
{
    public class BlobIndexer
    {
        [FunctionName("BlobIndexer")]
        public async Task Run([BlobTrigger("wikipedia-documents/{name}", Connection = "serverlessindexing_STORAGE")]CloudBlockBlob myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n URI: {myBlob.Uri.ToString()} Bytes");

            Document d = new Document();
                d.Id = Base64EncodeString(myBlob.Uri.ToString());
                d.Content = await ComputerVisionHelper.ReadFile(myBlob);
                d.Title = name;
                d.Source = "blob";

                // Call Cognitive Services  for enrichment (skillset replacement)
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
                // SearchIndexHelper.CreateOrUpdateIndex("wikipedia-index");
                SearchIndexHelper.UploadDocuments(d);
        }

        private string Base64EncodeString(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
