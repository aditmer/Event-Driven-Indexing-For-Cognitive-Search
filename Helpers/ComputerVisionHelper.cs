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

namespace Serverless.Indexer
{
	
	public class ComputerVisionHelper
	{
		private static string subscriptionKey = Environment.GetEnvironmentVariable("Cog_Service_Key");
        private static string endpoint = Environment.GetEnvironmentVariable("Cog_Service_Endpoint");

		private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }
	}

	
}