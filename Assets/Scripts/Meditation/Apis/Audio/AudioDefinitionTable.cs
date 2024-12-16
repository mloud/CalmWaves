using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Apis.Audio
{
    [CreateAssetMenu(fileName = "AudioDefinitionTable", menuName = "ScriptableObjects/AudioDefinitionTable", order = 1)]

    public class AudioDefinitionTable : ScriptableObjectTable<AudioDefinition>
    { }
}