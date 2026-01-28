using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 moveDistance = new Vector3(5f, 0f, 0f);
    [SerializeField] private float speed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float movement = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = startPosition + moveDistance * movement;
    }
}
