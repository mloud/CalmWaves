using System.Collections.Generic;
using OneDay.Core.Modules.Data;

namespace Meditation.Apis.Audio
{
    [System.Serializable]
    public class AudioDefinition : BaseDataObject
    {
        public AudioType Type;
        public string Name;
        public string Category;
        public string AudioSourceName;
        public string IconKey;
        public List<string> Tags;
    }
}