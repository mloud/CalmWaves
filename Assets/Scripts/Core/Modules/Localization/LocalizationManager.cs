using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Localization
{
    public interface ILocalizationManager
    {
        public Action<string> LanguageChanged { get; set; }
        string Language { get; set; }
        string DefaultLanguage { get; set; }
        ILocalizationDatabase LocalizationDatabase { get; set; }
        string Localize(string stringId);
    }

    public class LocalizationManager : MonoBehaviour, IService, ILocalizationManager
   {
       public Action<string> LanguageChanged { get; set; }

       public string Language
       {
           get => language;
           set
           {
               if (language == value) return;
               language = value;
               LanguageChanged?.Invoke(value);
           }
       }
       public string DefaultLanguage { get; set; }
       public ILocalizationDatabase LocalizationDatabase { get; set; }

       private string language;
       public string Localize(string stringId)
       {
           var localizedText =
               LocalizationDatabase.GetText(stringId, Language) ??
               LocalizationDatabase.GetText(stringId, DefaultLanguage);

           return localizedText ??= "???";
       }

       public UniTask Initialize() => UniTask.CompletedTask;
   }
}