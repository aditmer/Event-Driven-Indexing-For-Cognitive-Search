using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using Microsoft.Azure.Storage.Blob;

namespace Serverless.Indexer
{
	
    // Use OCR's ReadFile API to extract text from PDF blobs
	public class ComputerVisionHelper
	{
		private static string subscriptionKey = Environment.GetEnvironmentVariable("Cog_Service_Key");
        private static string endpoint = Environment.GetEnvironmentVariable("Cog_Service_Endpoint");
        private static ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey)){ Endpoint = endpoint };
		
        public static async Task<string> ReadFile(CloudBlockBlob cloudBlockBlob)
        {
            string sasUrl = cloudBlockBlob.Uri.ToString() + GetSASURL(cloudBlockBlob);
            string returnText = "";

            // Read text from URL
            var textHeaders = await client.ReadAsync(sasUrl);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            Console.WriteLine($"Extracting text from URL file {Path.GetFileName(sasUrl)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    returnText += line.Text + "\n";
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
            return returnText;
        }

        private static string GetSASURL(CloudBlockBlob cloudBlockBlob)
        {
            // Get the SAS URL for the blob
            string sasUrl = cloudBlockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
            });

            return sasUrl;
        }
	}

	
}