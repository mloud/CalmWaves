using TMPro;
using UnityEngine;

namespace OneDay.Core.Modules.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string textId;

        private TMP_Text textLabel;
        
        private void Start()
        {
            textLabel = GetComponent<TMP_Text>();
            if (textLabel == null)
            {
                Debug.LogError($"No TMP_Text component found on {gameObject.name}", gameObject);
                return;
            }

            ServiceLocator.Get<ILocalizationManager>().LanguageChanged += OnLanguageChanged;
            Localize();
        }

        private void OnLanguageChanged(string _) => Localize();

        private void Localize()
        {
            if (string.IsNullOrEmpty(textId))
                return;
            
            if (textLabel == null)
                return;
            
            textLabel.text = ServiceLocator.Get<ILocalizationManager>().Localize(textId);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<ILocalizationManager>().LanguageChanged -= OnLanguageChanged;
        }
    }
}