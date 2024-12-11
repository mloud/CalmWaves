using Meditation.Core.Utils;
using TMPro;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public class VersionComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionLabel;
        [SerializeField] private TextMeshProUGUI bundleCodeLabel;
        [SerializeField] private bool showOnDebugBuildOnly;
        private void Awake()
        {
            if (!showOnDebugBuildOnly || Debug.isDebugBuild)
            {
                versionLabel.text = Application.version;
                bundleCodeLabel.text = AppUtils.GetVersionCode().ToString();
            }
            else
            {
                versionLabel.enabled = false;
                bundleCodeLabel.enabled = false;
            }
        }
    }
}