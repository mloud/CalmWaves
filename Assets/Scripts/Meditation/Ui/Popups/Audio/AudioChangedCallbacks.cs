using System;
using Meditation.Apis.Audio;

namespace Meditation.Ui.Audio
{
    public class AudioChangedCallbacks
    {
        public Action<bool, AudioDefinition> SelectionChanged { get; set; }
        public Action<float, AudioDefinition> VolumeChanged { get; set; }
    }

}