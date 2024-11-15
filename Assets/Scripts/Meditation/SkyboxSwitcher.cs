using System.Collections.Generic;
using UnityEngine;

namespace Meditation
{
    public class SkyboxSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Material> materials;
        [SerializeField] private Material forcedMaterial;

        private void Awake()
        {
            if (forcedMaterial != null)
            {
                Set(forcedMaterial);
            }
            else
            {
                int skyboxIndex = PlayerPrefs.GetInt("Skybox", -1);
                skyboxIndex++;
                skyboxIndex %= materials.Count;
                PlayerPrefs.SetInt("Skybox", skyboxIndex);
                Set(materials[skyboxIndex]);
            }
        }

        private static void Set(Material material)
        {
            RenderSettings.skybox = material;
        }
    }
}