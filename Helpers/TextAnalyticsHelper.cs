using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;

namespace Serverless.Indexer
{
	
	public class TextAnalyticsHelper
	{
		private static string endpoint = Environment.GetEnvironmentVariable("Cog_Service_Endpoint");
		private static string apiKey = Environment.GetEnvironmentVariable("Cog_Service_Key");
		private static TextAnalyticsClient client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

		public static async Task<List<Language>> DetectLanguageInput (string input)
		{
			List<Language> languages = new List<Language>();

			try
			{
				var chunks = ChunksUpto(input, 5120);

				foreach(var chunk in chunks)
				{
					Response<DetectedLanguage> response = await client.DetectLanguageAsync(chunk);
					Language l = new Language();
					l.Confidence = response.Value.ConfidenceScore;
					l.Name = response.Value.Name;
					l.Iso6391Name = response.Value.Iso6391Name;
					if(languages.Where(lang => lang.Iso6391Name != l.Iso6391Name).Count() == 0)
					{
						languages.Add(l);
					}
					// Console.WriteLine($"Detected language {response.Value.Name} with confidence score {response.Value.ConfidenceScore}.");
				}
				
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
				
			}

			return languages;
		}

		public static async Task<List<SentenceSentiment>> DetectedSentiment(string input)
		{
			List<SentenceSentiment> sentiments = new List<SentenceSentiment>();

			try
			{
				var chunks = ChunksUpto(input, 5120);

				foreach(var chunk in chunks)
				{
					Response<DocumentSentiment> response = await client.AnalyzeSentimentAsync(chunk);
					foreach(SentenceSentiment sentenceSentiment in response.Value.Sentences)
					{
						sentiments.Add(sentenceSentiment);
					}
    				
				}
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
			}

			return sentiments;
		}

		public static async Task<List<string>> DetectedKeyPhrases(string input)
		{
			List<string> keyPhraseList = new List<string>();

			try
			{
				var chunks = ChunksUpto(input, 5120);

				foreach(var chunk in chunks)
				{
					Response<KeyPhraseCollection> response = await client.ExtractKeyPhrasesAsync(chunk);
					KeyPhraseCollection keyPhrases = response.Value;
					

					Console.WriteLine($"Extracted {keyPhrases.Count} key phrases:");
					foreach (string keyPhrase in keyPhrases)
					{
						if(!keyPhraseList.Contains(keyPhrase))
						{
							keyPhraseList.Add(keyPhrase);
							Console.WriteLine($"  {keyPhrase}");
						}
					}
				}
				
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
			}

			return keyPhraseList;
		}

		public static async Task<List<Entity>> DetectedEntities(string input)
		{
			List<Entity> entityList = new List<Entity>();

			try
			{
				var chunks = ChunksUpto(input, 5120);

				foreach(var chunk in chunks)
				{
					Response<CategorizedEntityCollection> response = await client.RecognizeEntitiesAsync(chunk);
					CategorizedEntityCollection entitiesInDocument = response.Value;
					

					Console.WriteLine($"Recognized {entitiesInDocument.Count} entities:");
					foreach (CategorizedEntity entity in entitiesInDocument)
					{
						Entity e = new Entity();
						e.Category = ((string)entity.Category);
						e.Subcategory = entity.SubCategory;
						e.Text = entity.Text;
						if(entityList.Where(ent => ent.Text == e.Text).Count() == 0)
						{
							entityList.Add(e);
							Console.WriteLine($"  Text: {entity.Text}");
							Console.WriteLine($"  Offset: {entity.Offset}");
							Console.WriteLine($"  Length: {entity.Length}");
							Console.WriteLine($"  Category: {entity.Category}");
							if (!string.IsNullOrEmpty(entity.SubCategory))
								Console.WriteLine($"  SubCategory: {entity.SubCategory}");
							Console.WriteLine($"  Confidence score: {entity.ConfidenceScore}");
							Console.WriteLine("");
						}
					}
				}
				
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
			}

			return entityList;
		}
		
		public static async Task<string> RedactedText(string input)
		{
			// List<PiiEntityCollection> piiList = new List<PiiEntityCollection>();
			string redactedText = "";

			try
			{
				var chunks = ChunksUpto(input, 5120);

				foreach(var chunk in chunks)
				{
					Response<PiiEntityCollection> response = await client.RecognizePiiEntitiesAsync(chunk);
					PiiEntityCollection entities = response.Value;
					// piiList.Add(entities);
					redactedText += entities.RedactedText;

					Console.WriteLine($"Redacted Text: {entities.RedactedText}");
					Console.WriteLine("");
					Console.WriteLine($"Recognized {entities.Count} PII entities:");
					foreach (PiiEntity entity in entities)
					{
						Console.WriteLine($"  Text: {entity.Text}");
						Console.WriteLine($"  Category: {entity.Category}");
						if (!string.IsNullOrEmpty(entity.SubCategory))
							Console.WriteLine($"  SubCategory: {entity.SubCategory}");
						Console.WriteLine($"  Confidence score: {entity.ConfidenceScore}");
						Console.WriteLine("");
					}
				}
				
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
			}

			return redactedText;
		}

		public async static Task<List<SummarySentence>> ExtractSummaryResultsAsync(string input)
		{
			List<SummarySentence> summaryList = new List<SummarySentence>();

			try
			{
				var chunks = ChunksUpto(input, 125000);

				foreach(string chunk in chunks)
				{				
					TextAnalyticsActions actions = new TextAnalyticsActions()
					{
						ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction() }
					};

					var doc = new List<string>();
					doc.Add(chunk);
					var operation = await client.StartAnalyzeActionsAsync(doc, actions);
					await operation.WaitForCompletionAsync();
					await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
					{
						IReadOnlyCollection<ExtractSummaryActionResult> summaryResults = documentsInPage.ExtractSummaryResults;
				
						foreach (ExtractSummaryActionResult summaryActionResults in summaryResults)
						{
							if (summaryActionResults.HasError)
							{
								Console.WriteLine($"  Error!");
								Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
								Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
								continue;
							}
				
							foreach (ExtractSummaryResult documentResults in summaryActionResults.DocumentsResults)
							{
								if (documentResults.HasError)
								{
									Console.WriteLine($"  Error!");
									Console.WriteLine($"  Document error code: {documentResults.Error.ErrorCode}.");
									Console.WriteLine($"  Message: {documentResults.Error.Message}");
									continue;
								}
				
								Console.WriteLine($"  Extracted the following {documentResults.Sentences.Count} sentence(s):");
								Console.WriteLine();
				
								foreach (SummarySentence sentence in documentResults.Sentences)
								{
									summaryList.Add(sentence);
									Console.WriteLine($"  Sentence: {sentence.Text}");
									Console.WriteLine();
								}
							}
						}
					}
			
				}
			}
			catch (RequestFailedException exception)
			{
				Console.WriteLine($"Error Code: {exception.ErrorCode}");
				Console.WriteLine($"Message: {exception.Message}");
			}
			

			return summaryList;
		}
		private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize) 
		{
			for (int i = 0; i < str.Length; i += maxChunkSize) 
				yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
		}

	}
}