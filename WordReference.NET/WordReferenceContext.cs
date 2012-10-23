using Newtonsoft.Json;

namespace WordReference
{
    public class WordReferenceContext
    {
		private string m_key;

		public WordReferenceContext(string apiKey)
		{
			m_key = apiKey;
		}

		public LanguageDictionary CreateDictionary(Language source, Language target)
		{
			return new LanguageDictionary(source, target, m_key);
		}
    }
}
