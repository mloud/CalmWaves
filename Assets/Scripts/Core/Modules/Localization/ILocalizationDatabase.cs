using System.Collections.Generic;

namespace OneDay.Core.Modules.Localization
{
    public interface ILocalizationDatabase
    {
        string GetText(string textId, string language);

        IEnumerable<string> GetTextIds();
    }
}