using System;
using System.Linq;
using WordReference;

namespace SampleApp
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			// The context just requires an API key, see http://www.wordreference.com/docs/api.aspx for terms etc
			var context = new WordReferenceContext("51dfe");

			// A dictionary uses a source and target language to query the WordReference API
			var enfrDict = context.CreateDictionary(Language.English, Language.German);

			while(true)
			{
				Console.WriteLine("Please enter a search query");
				var query = Console.ReadLine();
				Console.WriteLine();

				var task = enfrDict.TranslateAsync(query);

				// In this simple console app, we just wait for the task to complete
				try
				{
					task.Wait();
					SuccessfulQuery(task.Result);
				}
				catch(AggregateException ae)
				{
					ae.Handle(ex =>
					{
						if(ex is NotSupportedException)
						{
							Console.WriteLine(ex.Message);
							return true;
						}

						return false;
					});
				}
			}
		}

		// The response from the TranslateAsync call is a TranslationResponse
		// This contains a collection of TranslationSets, and the original query string
		private static void SuccessfulQuery(TranslationResponse response)
		{
			Console.WriteLine("Found {0} results:", response.Translations.Count());
			var i = 0;

			// Each TranslationSet contains an original phrase, which indicates the sense
			// This is followed by a list of possible translations in that context
			foreach(var set in response.Translations)
			{
				Console.WriteLine("Result #{0}: original sense is \"{1}\" ({2})", ++i, set.Original.Sense, set.Original.PartOfSpeech);
				Console.WriteLine();

				foreach(var translation in set.Translations)
					Console.WriteLine(string.Format("\t{0}", translation.Term));

				Console.WriteLine();
			}

			Console.WriteLine();
			Console.WriteLine();
		}
	}
}
