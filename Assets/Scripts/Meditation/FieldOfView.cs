using UnityEngine;

namespace Meditation
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private float minFov = 30;
        [SerializeField] private float maxFov = 60;
        [SerializeField] private float speed = 1.0f;

        private Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            if (camera == null)
            {
                Debug.LogError("Camera component not found!");
            }
        }

        private void Update()
        {
            if (camera == null) return;
            camera.fieldOfView = Mathf.PingPong(Time.time * speed, maxFov - minFov) + minFov;
        }
    }
}