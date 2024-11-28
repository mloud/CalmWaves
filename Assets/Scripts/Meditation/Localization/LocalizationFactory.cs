using System.IO;
using System.Text;
using OneDay.Core.Modules.Localization;
using UnityEngine;

namespace Meditation.Localization
{
    public static class LocalizationFactory
    {
        public static ILocalizationDatabase Create()
        {
            var db = new LocalizationDatabase()
                .AddText("STR_CONTINUE", "en","Continue")
                .AddText("STR_BACK", "en","Back")
                .AddText("STR_SAVE", "en","Save")
                .AddText("STR_INHALE", "en", "Inhale")
                .AddText("STR_EXHALE", "en", "Exhale")
                .AddText("STR_HOLD", "en", "Hold")
                .AddText("STR_ROUNDS", "en", "Rounds")
                .AddText("STR_RECENT_EXERCISE", "en", "Recent Exercise");

            return db;
        }

#if UNITY_EDITOR
        public static void GenerateClassWithConstants(ILocalizationDatabase db)
        {
            var textIds = db.GetTextIds();

            var file = new StringBuilder();
            file.AppendLine("public static class TextIds");
            file.AppendLine("{");

            foreach (var textId in textIds)
            {
                file.AppendLine($"  public const string {textId} = \"{textId}\";");
            }

            file.AppendLine("}");
            // save to file
            try
            {
                var path = Path.Combine(Application.dataPath, "Scripts/Meditation", "TextIds.cs");
                File.WriteAllText(path, file.ToString());
                Debug.Log($"TextIds were saved to {path}");
            }
            catch (IOException e)
            {
                Debug.LogError($"Could not save TextId database {e}");
                throw;
            }
        }
#endif
    }
}