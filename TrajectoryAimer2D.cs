using UnityEngine;
using System.Collections.Generic;

public class TrajectoryAimer2D : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private int maxSteps = 100;                // Max number of simulation steps
    [SerializeField] private float timeStep = 0.02f;            // Time per physics simulation step
    [SerializeField] private LayerMask collisionMask = ~0;      // Layers to check for collisions

    [Header("Physics Parameters")]
    [Range(0, 1)] public float friction = 0.1f;                 // Friction applied on bounce
    [Range(0, 1)] public float bounciness = 0.75f;              // Energy retained after bouncing
    public float gravityScale = 0.5f;                           // Custom gravity multiplier for simulation

    [Header("Visualization Settings")]
    [SerializeField] private GameObject dotPrefab;              // Prefab for each trajectory dot
    [SerializeField] private Transform dotParent;               // Parent transform to hold dots
    [SerializeField] private float minAlpha = 0.1f;             // Minimum alpha value for fading dots

    private readonly List<GameObject> spawnedDots = new List<GameObject>(); // Spawned dots cache

    // Simulates and shows the trajectory based on origin and initial velocity
    public void ShowTrajectory(Vector2 origin, Vector2 initialVelocity)
    {
        ClearDots(); // Remove any previously drawn dots

        Vector2 position = origin;
        Vector2 velocity = initialVelocity;
        float gravity = Physics2D.gravity.y * gravityScale;

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < maxSteps; i++)
        {
            // Apply gravity
            velocity += new Vector2(0, gravity) * timeStep;

            // Predict next position
            Vector2 nextPosition = position + velocity * timeStep;

            // Check for collision between current and next position
            RaycastHit2D hit = Physics2D.Raycast(position, (nextPosition - position).normalized, (nextPosition - position).magnitude, collisionMask);
            if (hit.collider != null)
            {
                // On collision, add hit point to trajectory
                points.Add(hit.point);

                // Reflect velocity based on collision normal and apply bounciness
                velocity = Vector2.Reflect(velocity, hit.normal) * bounciness;

                // Apply friction on bounce using tangent vector
                Vector2 tangent = new Vector2(-hit.normal.y, hit.normal.x);
                float vTangent = Vector2.Dot(velocity, tangent);
                vTangent *= (1 - friction);
                velocity = Vector2.Dot(velocity, hit.normal) * hit.normal + vTangent * tangent;

                // Slight offset to avoid sticking on surface
                position = hit.point + hit.normal * 0.01f;
            }
            else
            {
                // If no collision, continue trajectory normally
                points.Add(nextPosition);
                position = nextPosition;
            }

            // Stop simulation if velocity is very small
            if (velocity.magnitude < 0.01f)
                break;
        }

        // Visualize all calculated points as fading dots
        for (int i = 0; i < points.Count; i++)
        {
            float t = (float)i / Mathf.Max(1, points.Count - 1);
            float alpha = Mathf.Lerp(1f, minAlpha, t); // Fade alpha from 1 to minAlpha
            SpawnDot(points[i], alpha);
        }
    }

    // Instantiates a dot at a given position with specific alpha transparency
    private void SpawnDot(Vector2 position, float alpha)
    {
        GameObject dot = Instantiate(dotPrefab, position, Quaternion.identity, dotParent);
        if (dot.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
        spawnedDots.Add(dot);
    }

    // Removes all previously spawned dots
    private void ClearDots()
    {
        foreach (var dot in spawnedDots)
        {
            Destroy(dot);
        }
        spawnedDots.Clear();
    }

    // Public method to hide the trajectory
    public void Hide()
    {
        ClearDots();
    }
}
