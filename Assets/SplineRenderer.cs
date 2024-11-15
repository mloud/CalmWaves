using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class SplineRenderer : MonoBehaviour
{
    public SplineContainer splineContainer; // Reference to the SplineContainer component
    public int samplesPerSegment = 1000; // Number of samples per spline segment
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        if (splineContainer != null)
        {
            RenderSpline();
        }
    }

    void RenderSpline()
    {
        // Calculate total points for Line Renderer
        int totalPoints = samplesPerSegment;
        lineRenderer.positionCount = totalPoints;

        // Sample points along the spline
        int index = 0;
        for (int i = 0; i < splineContainer.Spline.Count - 1; i++)
        {
            
            for (int j = 0; j < samplesPerSegment; j++)
            {
                float t = j / (float)(samplesPerSegment - 1);
                var position = splineContainer.EvaluatePosition(i, t);
                Vector3 p = new Vector3(position.x, position.y, position.z);
                lineRenderer.SetPosition(index, p);
                index++;
            }
        }
    }
}