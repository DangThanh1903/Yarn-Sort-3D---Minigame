using UnityEngine;

public class TestLerp : MonoBehaviour
{
    public Transform[] items;
    public float radius = 5f;
    public Vector3 center = Vector3.zero;
    public float moveSpeed = 50f; // degrees per second
    public float targetAngle = 0f; // Angle where all items should end up (in degrees)

    private float[] currentAngles;

    void Start()
    {
        currentAngles = new float[items.Length];
        float angleStep = 360f / items.Length;

        // Set initial angles around the circle
        for (int i = 0; i < items.Length; i++)
        {
            currentAngles[i] = i * angleStep;
        }
    }

    void Update()
    {
        for (int i = 0; i < items.Length; i++)
        {
            // Normalize angles to [0,360)
            currentAngles[i] = currentAngles[i] % 360f;
            float normalizedTarget = targetAngle % 360f;

            // Going clockwise
            float distClockwise = (normalizedTarget - currentAngles[i] + 360f) % 360f;
            // Going counter clockwise
            float distCounterClockwise = (currentAngles[i] - normalizedTarget + 360f) % 360f;

            // If distance is very small, snap to target angle
            if (distClockwise < 0.1f)
            {
                currentAngles[i] = normalizedTarget;
            }
            else
            {
                // Move clockwise by moveSpeed * deltaTime, but do not overshoot target
                float step = moveSpeed * Time.deltaTime;
                currentAngles[i] += Mathf.Min(step, distClockwise);
            }

            float rad = currentAngles[i] * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(0, Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
            items[i].position = center + offset;

            // Face outward from center
            items[i].LookAt(2 * items[i].position - center);
        }
    }
}
