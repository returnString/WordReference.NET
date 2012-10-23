using System.Collections.Generic;
using Newtonsoft.Json;

namespace WordReference
{
	public class TranslationResponse
	{
		public IEnumerable<TranslationSet> Translations { get; internal set; }
	}

	public class TranslationSet
	{
		public Phrase Original { get; internal set; }
		public IEnumerable<Phrase> Translations { get; internal set; }
	}
	
	public class Phrase
	{
		[JsonProperty("term")]
		public string Term { get; internal set; }
		[JsonProperty("POS")]
		public string PartOfSpeech { get; internal set; }
		[JsonProperty("sense")]
		public string Sense { get; internal set; }
		[JsonProperty("usage")]
		public string Usage { get; internal set; }
	}
}
