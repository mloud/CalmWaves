using UnityEngine;

namespace Meditation
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private Vector3 speed;

        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }
    }
}