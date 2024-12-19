using System;
using Coffee.UIExtensions;
using OneDay.Core.Debugging;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    [LogSection("Ui")]
    public class ParticleOnBox : BaseUiEffect
    {
        [SerializeField] private UIParticle uiParticle;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private float speed = 5f;
        [SerializeField] private int cycles = 1;
        
        private Vector3[] points;
        private int currentPoint;
        private int currentCycle;
        private bool isPlaying;
  
        public override void Run()
        {
            if (boxCollider == null)
            {
                Debug.LogError("BoxCollider not assigned!");
                return;
            }

            RefreshPoints();
            uiParticle.transform.position = points[0];
            uiParticle.Play();
            uiParticle.StartEmission();
            isPlaying = true;
            currentCycle = 0;
            currentPoint = 1;
        }

        public override void Stop()
        {
            uiParticle.StopEmission();
            isPlaying = false;
        }

        public override bool IsPlaying() => isPlaying;
        
        private void RefreshPoints()
        {
            points ??= new Vector3[4];
            Vector3 center = boxCollider.offset;
            Vector3 size = boxCollider.size * 0.5f;
            var tr = boxCollider.transform;

            points[0] = tr.TransformPoint(center + new Vector3(-size.x, size.y, 0));
            points[1] = tr.TransformPoint(center + new Vector3(size.x, size.y, 0));
            points[2] = tr.TransformPoint(center + new Vector3(size.x, -size.y, 0));
            points[3] = tr.TransformPoint(center + new Vector3(-size.x, -size.y, 0));
        }

        private void Update()
        {
            if (!isPlaying)
                return;

            RefreshPoints();
            uiParticle.transform.position = Vector3.MoveTowards(
                uiParticle.transform.position, points[currentPoint], speed * Time.deltaTime);

            if (Vector3.Distance(uiParticle.transform.position, points[currentPoint]) < 5)
            {
                currentPoint = (currentPoint + 1) % points.Length;
                if (currentPoint == 1)
                {
                    currentPoint = 0;
                    currentCycle++;
                    if (currentCycle >= cycles)
                    {
                        Stop();
                    }
                }
            }
        }
    }
}