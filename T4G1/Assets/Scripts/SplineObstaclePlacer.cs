using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineObstaclePlacer : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;

    [Header("Obstacle Prefabs")]
    public List<GameObject> obstaclePrefabs = new();

    [Header("Placement Settings")]
    public float spacing = 20f;
    public float horizontalOffset = 0f; // 0 = center, +/- = left/right
    public float verticalOffset = 0.1f;

    [Header("Randomization")]
    public bool randomizeSide = false;
    public Vector2 randomOffsetRange = new(-1.5f, 1.5f);

    Transform obstacleParent;

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        if (splineContainer == null || obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("SplineObstaclePlacer: Missing spline or obstacle prefabs.");
            return;
        }

        Clear();

        Spline spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        int count = Mathf.FloorToInt(splineLength / spacing);

        obstacleParent = new GameObject("Obstacles").transform;
        obstacleParent.SetParent(transform);

        for (int i = 0; i < count; i++)
        {
            float distance = i * spacing;
            float t = spline.ConvertIndexUnit(
                distance,
                PathIndexUnit.Distance,
                PathIndexUnit.Normalized
            );

            Vector3 position = splineContainer.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)splineContainer.EvaluateTangent(t)).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

            float offset = horizontalOffset;

            if (randomizeSide)
                offset += Random.Range(randomOffsetRange.x, randomOffsetRange.y);

            Vector3 finalPosition =
                position +
                right * offset +
                Vector3.up * verticalOffset;

            GameObject prefab =
                obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

            GameObject obstacle = Instantiate(
                prefab,
                finalPosition,
                Quaternion.LookRotation(tangent),
                obstacleParent
            );

            obstacle.name = $"Obstacle_{i + 1}";
        }
    }

    public void Clear()
    {
        Transform existing = transform.Find("Obstacles");
        if (existing != null)
            DestroyImmediate(existing.gameObject);
    }
}
