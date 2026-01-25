using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TrackGenerator))]
public class TrackGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrackGenerator generator = (TrackGenerator)target;

        //draw default fields
        EditorGUILayout.LabelField("Track Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        generator.splineContainer = (UnityEngine.Splines.SplineContainer)
        EditorGUILayout.ObjectField("Spline Container",
        generator.splineContainer,
        typeof(UnityEngine.Splines.SplineContainer),
        true);

        generator.trackSegmentPrefab = (GameObject)
        EditorGUILayout.ObjectField("Track Segment Prefab",
        generator.trackSegmentPrefab,
        typeof(GameObject),
        false);

        generator.spacing = EditorGUILayout.Slider("Spacing", generator.spacing, 0.1f, 10f);

        generator.TrackWidth = EditorGUILayout.Slider("Track Width", generator.TrackWidth, 0.1f, 20f);

        EditorGUILayout.Space(10);

        //validate
        if (generator.splineContainer == null ||
            generator.trackSegmentPrefab == null)
        {
            EditorGUILayout.HelpBox("Please assign all fields.", MessageType.Warning);
            return;
        }

        if (generator.trackSegmentPrefab == null)
        {
            EditorGUILayout.HelpBox("Track Segment Prefab cannot be null.", MessageType.Warning);
            return;
        }

        //buttons
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = generator.splineContainer != null &&
                      generator.trackSegmentPrefab != null;

        if (GUILayout.Button("Generate Track", GUILayout.Height(40)))
        {
            Undo.RegisterFullObjectHierarchyUndo(generator.gameObject, "Generate Track");
            generator.Generate();
        }

        GUI.enabled = true;

        if (GUILayout.Button("Clear Track", GUILayout.Height(40)))
        {
            Undo.RegisterFullObjectHierarchyUndo(generator.gameObject, "Clear Track");
            generator.Clear();
        }

        EditorGUILayout.EndHorizontal();
    }
}