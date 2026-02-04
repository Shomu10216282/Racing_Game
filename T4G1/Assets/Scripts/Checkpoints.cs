using UnityEngine;
using System.Collections.Generic;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    [SerializeField] private Color touchedColor = Color.green;

    private int currentCheckpointIndex = 0;

    void Start()
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].Setup(this, i, touchedColor);
        }
    }

    public void CheckpointTouched(int index)
    {
        if (index == currentCheckpointIndex)
        {
            currentCheckpointIndex++;
            Debug.Log($"Checkpoint {index + 1}/{checkpoints.Count} passed!");

            if (currentCheckpointIndex >= checkpoints.Count)
            {
                //player controller's win function here
                this.gameObject.GetComponent<PlayerController>().GameWon();
            }
        }
    }

    public bool IsCorrectCheckpoint(int index)
    {
        return index == currentCheckpointIndex;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), $"Checkpoint: {currentCheckpointIndex}/{checkpoints.Count}"); //note to self CHANGE THIS TO USE STATIC UI LATER!!!!!!
    }
}
