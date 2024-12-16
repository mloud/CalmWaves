using Cysharp.Threading.Tasks;
using OneDay.Core.Debugging;
using UnityEngine;

namespace OneDay.Core.Modules.Conditions.Ui
{
    [LogSection("Ui")]
    public class ConditionComponent : MonoBehaviour
    {
        [SerializeField] private string conditionName;
        [SerializeField] private bool refreshOnEnable;
        [SerializeField] private bool useNegative;
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
                D.LogError($"No condition set on {gameObject.name}", this);
                return;
            }

            var conditionManager = ServiceLocator.Get<IConditionManager>();
            var result = await conditionManager.Evaluate(conditionName);
            D.LogInfo($"Refreshing {nameof(ConditionComponent)} on {gameObject.name} to {result}", this);
            gameObject.SetActive(useNegative ?!result : result);
        }
    }
}