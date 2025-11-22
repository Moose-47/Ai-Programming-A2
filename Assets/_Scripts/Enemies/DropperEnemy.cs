using UnityEngine;

public class DropperEnemy : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    public bool loop = true;

    private int currentIndex = 0;

    [Header("Dropping")]
    public GameObject dropPrefab;
    public float dropInterval = 3f;
    public float dropHeightOffset = 2f;

    private float dropTimer;

    private void Update()
    {
        HandleMovement();
        HandleDropping();
    }

    void HandleMovement()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                currentIndex = loop ? 0 : waypoints.Length - 1;
            }
        }
    }

    void HandleDropping()
    {
        if (!dropPrefab) return;

        dropTimer += Time.deltaTime;

        if (dropTimer >= dropInterval)
        {
            DropObject();
            dropTimer = 0f;
        }
    }

    void DropObject()
    {
        Vector3 spawnPos = transform.position + Vector3.up * dropHeightOffset;
        GameObject newDrop = Instantiate(dropPrefab, spawnPos, Quaternion.identity);

        Destroy(newDrop, 5f);
    }
}
