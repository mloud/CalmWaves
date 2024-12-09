using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Conditions.Ui
{
    public class ConditionComponent : MonoBehaviour
    {
        [SerializeField] private string conditionName;

        private void OnEnable()
        {
            Refresh();
        }

        private async UniTask Refresh()
        {
            if (string.IsNullOrEmpty(conditionName))
            {
                Debug.LogError($"No condition set on {gameObject.name}", gameObject);
                return;
            }
            var result = await ServiceLocator.Get<IConditionManager>().Evaluate(conditionName);
            gameObject.SetActive(result);
        }
    }
}