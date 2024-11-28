using System.Collections.Generic;
using System.Linq;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace OneDay.Core.Modules.Localization
{
    public class LocalizationDatabase : BaseDataObject, ILocalizationDatabase
    {
        private Dictionary<string, Dictionary<string, string>> LocalizedTexts { get; set; }
        
        public string GetText(string textId, string language)
        {
            Debug.Assert(language != null);
            Debug.Assert(textId != null);

            if (LocalizedTexts.TryGetValue(language, out var languageDatabase))
            {
                if (languageDatabase.TryGetValue(textId, out var localizedText))
                {
                    return localizedText;
                }

                Debug.LogError($"No such texts textId {textId} found in language dictionary {language}");
                return null;
            }
            Debug.LogError($"No such language {language} exists");
            return null;
        }

        public IEnumerable<string> GetTextIds() => 
            LocalizedTexts.SelectMany(x => x.Value.Keys).Distinct().ToList();
        
        public LocalizationDatabase AddText(string textId, string language, string text)
        {
            LocalizedTexts ??= new Dictionary<string, Dictionary<string, string>>();
            
            if (!LocalizedTexts.ContainsKey(language))
            {
                LocalizedTexts.Add(language, new Dictionary<string, string>());
            }

            LocalizedTexts[language].TryAdd(textId, text);
            return this;
        }
    }
}