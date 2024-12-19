using Coffee.UIExtensions;
using OneDay.Core.Debugging;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    [LogSection("Ui")]
    public class ParticleOnCircle : BaseUiEffect
    {
        [SerializeField] private UIParticle uiParticle;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private float speed = 5f;
        [SerializeField] private int cycles = 1;

        private float angle;
        private int currentCycle;
        private bool isPlaying;
  
        public override void Run()
        {
            if (circleCollider == null)
            {
                Debug.LogError("BoxCollider not assigned!");
                return;
            }

            currentCycle = 0;
            angle = 0;
            uiParticle.transform.position = GetPosition(angle);
            uiParticle.Play();
            uiParticle.StartEmission();
            isPlaying = true;
        }

        public override void Stop()
        {
            uiParticle.StopEmission();
            isPlaying = false;
        }

        public override bool IsPlaying() => isPlaying;
        
        private void Update()
        {
            if (!isPlaying)
                return;

            uiParticle.transform.position = GetPosition(angle);
            angle += speed * Time.deltaTime;
            if (angle > 360)
            {
                angle -= 360;
                currentCycle++;
                if (currentCycle >= cycles)
                {
                    Stop();
                }
            }
            
        }
        
        private Vector3 GetPosition(float angle)
        {
            float angleInRad = -angle * Mathf.Deg2Rad;
            
            Vector3 center = circleCollider.transform.TransformPoint(circleCollider.offset);

            var position =  center + new Vector3(
                circleCollider.radius / 2 * Mathf.Cos(angleInRad),
                circleCollider.radius / 2* Mathf.Sin(angleInRad),
                0);
            
            Debug.DrawLine(center, position, Color.red, 0.1f);
            return position;
        }
    }
}