using TMPro;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    public class WaveTextEffect : BaseUiEffect
    {
        public float waveSpeed = 2f; // Speed of the wave
        public float waveAmplitude = 5f; // Height of the wave
        public float waveFrequency = 1f; // Distance between wave peaks

        [SerializeField] TMP_Text textMeshPro;
        
        private Vector3[] originalVertices;
        private TMP_MeshInfo[] cachedMeshInfo;

        private bool isPlaying;
        
        public override void Run()
        {
            cachedMeshInfo = textMeshPro.textInfo.CopyMeshInfoVertexData();
            isPlaying = true;
        }

        public override void Stop()
        {
            isPlaying = false;
        }

        public override bool IsPlaying() => isPlaying;
        
        private void Start()
        {
            textMeshPro = GetComponent<TMP_Text>();
            textMeshPro.ForceMeshUpdate();
            cachedMeshInfo = textMeshPro.textInfo.CopyMeshInfoVertexData();
        }

        private void Update()
        {
            if (isPlaying)
            {
                AnimateWave();
            }
        }

        private void AnimateWave()
        {
            // Get the text information
            TMP_TextInfo textInfo = textMeshPro.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Calculate wave effect
                float offset = Mathf.Sin(Time.time * waveSpeed + i * waveFrequency) * waveAmplitude;

                // Apply offset to each vertex of the character
                vertices[vertexIndex + 0].y += offset;
                vertices[vertexIndex + 1].y += offset;
                vertices[vertexIndex + 2].y += offset;
                vertices[vertexIndex + 3].y += offset;
            }

            // Update the mesh with the modified vertices
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
    }
}