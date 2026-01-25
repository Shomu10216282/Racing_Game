using UnityEngine;
using UnityEngine.Splines;

public class TrackGenerator : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;
    public GameObject trackSegmentPrefab;

    [Header("Default settings")]
    public float spacing = 2f;
    public float TrackWidth = 4f;
    public float maxSpacing;

    public void Generate()
    {
        Clear();

        Spline spline = splineContainer.Spline;
        float length = spline.GetLength();

        int SegmentCount = Mathf.Max(2, Mathf.CeilToInt(length / spacing));

        for (int i = 0; i < SegmentCount; i++)
        {
            float distance = i * spacing;
            float t = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);

            Vector3 position = splineContainer.EvaluatePosition(t);
            Vector3 tangent  = splineContainer.EvaluateTangent(t);
            
            Quaternion rotation = Quaternion.LookRotation(tangent, Vector3.up);

            GameObject segment = Instantiate(
            trackSegmentPrefab,
            position,
            rotation,
            transform
            );
            
            Vector3 scale = segment.transform.localScale;
            scale.x = TrackWidth;
            segment.transform.localScale = scale;
        }
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
