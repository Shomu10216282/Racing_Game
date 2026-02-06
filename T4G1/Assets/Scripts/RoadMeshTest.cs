using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadMeshTest : MonoBehaviour
{
    [Header("References")]
    public SplineContainer splineContainer;

    [Header("Road Shape")]
    public float trackWidth = 6f;
    public float segmentLength = 1f;
    public float trackThickness = 0.5f;

    [Header("UV Settings")]
    public float textureTiling = 5f;

    [Header("Checkpoint Settings")]
    public GameObject checkpointPrefab;
    public int checkpointCount = 0;
    public float checkpointSpacing = 50f;

    Mesh mesh;

    public void Generate()
    {
        if (splineContainer == null)
        {
            Debug.LogWarning("RoadMeshGenerator: Missing spline container.");
            return;
        }

        Clear();

        GenerateRoadMesh();
        GenerateCheckpoints();
    }

    void GenerateRoadMesh()
    {
        Spline spline = splineContainer.Spline;
        float splineLength = spline.GetLength();
        int steps = Mathf.CeilToInt(splineLength / segmentLength);

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        Vector3 prevPosition = splineContainer.EvaluatePosition(0f);
        float vCoord = 0f;

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;

            Vector3 position = splineContainer.EvaluatePosition(t);
            Vector3 tangent = ((Vector3)splineContainer.EvaluateTangent(t)).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;

            if ( i> 0)
            {
                vCoord += Vector3.Distance(prevPosition, position) / textureTiling;
                prevPosition = position;
            }

           Vector3 halfWidth = right * (trackWidth / 2f);

           Vector3 leftTop = position - halfWidth;
           Vector3 rightTop = position + halfWidth;

           Vector3 leftBottom = leftTop - Vector3.up * trackThickness;
           Vector3 rightBottom = rightTop - Vector3.up * trackThickness;

            vertices.Add(transform.InverseTransformPoint(leftTop));
            vertices.Add(transform.InverseTransformPoint(rightTop));
            vertices.Add(transform.InverseTransformPoint(leftBottom));
            vertices.Add(transform.InverseTransformPoint(rightBottom));

            // top vert uvs
            uvs.Add(new Vector2(0f, vCoord));
            uvs.Add(new Vector2(1f, vCoord));

            //bottom vert uvs
            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(1f, 0f));

            if(i < steps)
            {
                // Each spline step adds 4 vertices:
                // 0 = Left Top
                 // 1 = Right Top
                  // 2 = Left Bottom
                  // 3 = Right Bottom
                  //
                 // Next step starts at baseIndex + 4
                int baseIndex = i * 4;

                /* =========================
                  * TOP FACE (road surface)
                  * =========================
                  * Connects:
                  * current left/right top
                  * to next left/right top
                  *
                  * Winding order is CLOCKWISE
                  * when viewed from above
                  */

                // Triangle 1 (current left → next left → current right)
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 1);
                // Triangle 2 (current right → next left → next right)
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 5);

                 /* =========================
                  * BOTTOM FACE (underside)
                  * =========================
                  * Same as top, but using
                  * bottom vertices
                  */

                // Triangle 1 (current left bottom → next left bottom → current right bottom)
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 6);
                triangles.Add(baseIndex + 3);
                // Triangle 2 (current right bottom → next left bottom → next right bottom)
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 6);
                triangles.Add(baseIndex + 7);

                /* =========================
                * LEFT SIDE WALL
                * =========================
                * Connects left top to
                * left bottom along spline
                */
                // Triangle 1 (current left top → next left top → current left bottom)
                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 2);
                // Triangle 2 (current left bottom → next left top → next left bottom)
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 4);
                triangles.Add(baseIndex + 6);

                /* =========================
                 * RIGHT SIDE WALL
                 * =========================
                 * Connects right top to
                 * right bottom along spline
                 */

                // Triangle 1 (current right top → next right top → current right bottom)
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 5);
                // Triangle 2 (current right bottom → next right top → next right bottom)
                triangles.Add(baseIndex + 5);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 7);
            }
        }

        if (mesh == null)
        {
            mesh = new Mesh { name = "Road Mesh" };
        }
        else
        {
            mesh.Clear();
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void GenerateCheckpoints()
    {
        if (checkpointPrefab == null)
            return;

        Spline spline = splineContainer.Spline;
        float splineLength = spline.GetLength();

        Transform parent = new GameObject("Checkpoints").transform;
        parent.SetParent(transform);

        int count = checkpointCount > 0
            ? checkpointCount
            : Mathf.FloorToInt(splineLength / checkpointSpacing);

        for (int i = 0; i < count; i++)
        {
            float t = (i + 1f) / (count + 1f);

            Vector3 position = splineContainer.EvaluatePosition(t);
            position.y += trackThickness / 2f;
            Vector3 scale = new Vector3(trackWidth, trackThickness, 1f);
            Vector3 tangent = ((Vector3)splineContainer.EvaluateTangent(t)).normalized;


            Quaternion rotation =
                Quaternion.LookRotation(tangent) *
                Quaternion.Euler(0, 90f, 0f); // 90° Y rotation

            GameObject checkpoint = Instantiate(
                checkpointPrefab,
                position,
                rotation,
                parent
            );

            checkpoint.name = $"Checkpoint_{i + 1}";
        }
    }

    public void Clear()
    {
        if (mesh != null)
            mesh.Clear();

        // Remove checkpoints
        Transform cp = transform.Find("Checkpoints");
        if (cp != null)
            DestroyImmediate(cp.gameObject);
    }
}
