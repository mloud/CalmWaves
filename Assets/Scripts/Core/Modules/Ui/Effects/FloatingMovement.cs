using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [Header("Floating Settings")]
    public float radius = 1f; // The maximum distance from the center point.
    public float speed = 1f; // Speed of the floating movement.
    public bool useLocalSpace = true; // Whether to use local or world space.

    public Vector3 centerPoint; // The center point around which the object floats.
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float progress = 0f;

    void Start()
    {
        centerPoint = useLocalSpace ? transform.localPosition : transform.position;
        startPosition = useLocalSpace ? transform.localPosition : transform.position;
        ChooseNewTarget();
    }

    void Update()
    {
        // Smoothly move towards the target position.
        progress += Time.deltaTime * speed;

        if (progress >= 1f)
        {
            progress = 0f;
            ChooseNewTarget();
        }

        // Interpolate between the current and target position.
        if (useLocalSpace)
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
        else
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
    }

    private void ChooseNewTarget()
    {
        // Choose a random position within a sphere of the specified radius.
        Vector3 randomOffset = Random.insideUnitSphere * radius;
        randomOffset.y = Mathf.Abs(randomOffset.y); // Keep movement mostly horizontal for buttons.
        targetPosition = centerPoint + randomOffset;

        // Update the start position for interpolation.
        startPosition = useLocalSpace ? transform.localPosition : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the movement radius in the scene view.
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(useLocalSpace ? transform.localPosition : transform.position, radius);
    }
}