using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Managers.Messages.Data
{

    [CreateAssetMenu(fileName = "MessageDefinitionTable", menuName = "ScriptableObjects/MessageDefinitionTable", order = 1)]

    public class MessageDefinitionTable : ScriptableObjectTable<MessageDefinition>
    {
    }
}