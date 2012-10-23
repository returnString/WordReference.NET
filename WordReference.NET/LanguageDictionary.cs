using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WordReference
{
	public class LanguageDictionary
	{
		private string m_lookup;
		private string m_key;
		private WebClient m_client;

		internal LanguageDictionary(Language sourceLang, Language targetLang, string apiKey)
		{
			m_lookup = CountryCodeAttribute.Get(sourceLang) + CountryCodeAttribute.Get(targetLang);
			m_key = apiKey;
			m_client = new WebClient { Encoding = Encoding.UTF8 };
		}

		private Uri GetUri(string word)
		{
			return new Uri(string.Format("http://api.wordreference.com/{0}/json/{1}/{2}", m_key, m_lookup, word));
		}

		private static readonly string[] Categories = { "Entries", "PrincipalTranslations", "AdditionalTranslations" };

		public async Task<IEnumerable<TranslationSet>> TranslateAsync(string word)
		{
			var raw = await m_client.DownloadStringTaskAsync(GetUri(word));
			var obj = JObject.Parse(raw);
			var list = new List<TranslationSet>();

			foreach(var pair in obj)
			{
				var key = pair.Key;

				// TODO: Add support for compounds etc
				if(!key.Contains("term"))
					continue;
				
				foreach(var cat in pair.Value)
				{
					var catProp = cat as JProperty;

					if(!Categories.Contains(catProp.Name))
						continue;

					foreach(var index in cat.Children().Children())
					{
						var set = new TranslationSet();
						list.Add(set);

						var translationList = new List<Phrase>();

						foreach(var translation in index.Children().Children())
						{
							var translationProp = translation as JProperty;

							switch(translationProp.Name)
							{
							case "Note":
								continue;

							case "OriginalTerm":
								set.Original = ParseWord(translationProp);
								break;

							default:
								translationList.Add(ParseWord(translationProp));
								break;
							}
						}

						set.Translations = translationList;
					}
				}
			}

			return list;
		}

		private Phrase ParseWord(JProperty prop)
		{
			return JsonConvert.DeserializeObject<Phrase>(prop.Value.ToString());
		}
	}
}
