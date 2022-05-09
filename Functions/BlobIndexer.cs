using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serverless.Indexer
{
    public class BlobIndexer
    {
        [FunctionName("BlobIndexer")]
        public async Task Run([BlobTrigger("documents/{name}", Connection = "serverlessindexing_STORAGE")]CloudBlockBlob myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n URI: {myBlob.Uri.ToString()} Bytes");

            Document d = new Document();
                d.Id = myBlob.Uri.ToString();
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
                // SearchIndexHelper.CreateOrUpdateIndex("wikipedia");
                SearchIndexHelper.UploadDocuments(d);
        }
    }
}
