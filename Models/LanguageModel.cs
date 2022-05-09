using Azure.Search.Documents.Indexes;

namespace Serverless.Indexer
{
    public class Language
    {
		[SearchableField(IsFilterable = true, IsFacetable = true)]
		public string Name { get; set; }
		[SearchableField(IsFilterable = true, IsFacetable = true)]
		public string Iso6391Name { get; set; }
		public double Confidence { get; set; }
	}
}