namespace Serverless.Indexer
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
	using Azure.AI.TextAnalytics;
	using Azure.Search.Documents.Indexes;
	using Azure.Search.Documents.Indexes.Models;
	using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Document
    {
		[SimpleField(IsKey = true, IsFilterable = true)]
		[JsonProperty("id")]
		public string Id { get; set; }
		[SearchableField(IsSortable = true)]
		public string Title { get; set; }
		[SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
		public string Content { get; set; }
		[SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
		[JsonProperty("source")]
		public string Source { get; set; }
		[SearchableField(IsFilterable = true, IsFacetable = true)]
		public List<string> KeyPhrases { get; set; }
		[SearchableField(IsFilterable = true, IsFacetable = true)]
		public List<Entity> Entities { get; set; }
		// [SearchableField(IsFilterable = true, IsFacetable = true)]
		// public List<SentenceSentiment> Sentiments { get; set; }
		[SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
		public string Summary { get; set; }
		[SearchableField(IsFilterable = true, IsFacetable = true)]
		public List<Language> Languages { get; set; }
		[SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
		public string RedactedText { get; set; }
	}
}