using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadMeshTest))]
public class RoadMeshTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RoadMeshTest generator = (RoadMeshTest)target;

        EditorGUILayout.LabelField("Road Mesh Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        generator.splineContainer =
            (UnityEngine.Splines.SplineContainer)
            EditorGUILayout.ObjectField(
                "Spline Container",
                generator.splineContainer,
                typeof(UnityEngine.Splines.SplineContainer),
                true
            );

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Road Settings", EditorStyles.boldLabel);

        generator.trackWidth =
            EditorGUILayout.Slider("Track Width", generator.trackWidth, 1f, 20f);

        generator.segmentLength =
            EditorGUILayout.Slider("Segment Length", generator.segmentLength, 0.2f, 5f);

        generator.textureTiling =
            EditorGUILayout.Slider("Texture Tiling", generator.textureTiling, 0.1f, 5f);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Checkpoint Settings", EditorStyles.boldLabel);

        generator.checkpointPrefab =
            (GameObject)EditorGUILayout.ObjectField(
                "Checkpoint Prefab",
                generator.checkpointPrefab,
                typeof(GameObject),
                false
            );

        generator.checkpointCount =
            EditorGUILayout.IntSlider("Checkpoint Count", generator.checkpointCount, 0, 50);

        generator.checkpointSpacing =
            EditorGUILayout.Slider("Checkpoint Spacing", generator.checkpointSpacing, 5f, 200f);

        EditorGUILayout.HelpBox(
            "If Checkpoint Count > 0, checkpoints are evenly spaced.\nOtherwise spacing is used.",
            MessageType.Info
        );

        EditorGUILayout.Space(15);

        if (generator.splineContainer == null)
        {
            EditorGUILayout.HelpBox("Spline Container is required.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate Road", GUILayout.Height(40)))
        {
            Undo.RegisterCompleteObjectUndo(generator.gameObject, "Generate Road");
            generator.Generate();
            EditorUtility.SetDirty(generator);
        }

        if (GUILayout.Button("Clear", GUILayout.Height(40)))
        {
            Undo.RegisterCompleteObjectUndo(generator.gameObject, "Clear Road");
            generator.Clear();
            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.EndHorizontal();
    }
}
