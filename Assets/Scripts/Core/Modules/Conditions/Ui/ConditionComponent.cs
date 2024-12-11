using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Conditions.Ui
{
    public class ConditionComponent : MonoBehaviour
    {
        [SerializeField] private string conditionName;
        [SerializeField] private bool refreshOnEnable;
        private void OnEnable()
        {
            if (refreshOnEnable)
            {
                Refresh();
            }
        }

        public async UniTask Refresh()
        {
            if (string.IsNullOrEmpty(conditionName))
            {
                Debug.LogError($"No condition set on {gameObject.name}", gameObject);
                return;
            }

            var conditionManager = ServiceLocator.Get<IConditionManager>();
            var result = await conditionManager.Evaluate(conditionName);
            gameObject.SetActive(result);
        }
    }
}