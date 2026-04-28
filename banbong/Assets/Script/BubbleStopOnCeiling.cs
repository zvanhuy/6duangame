using System.Collections.Generic;
using UnityEngine;
public class BubbleStopOnCeiling : MonoBehaviour
{
    private Rigidbody2D rb;
    public ShooterController shooter;
    [SerializeField] private float gridSize = 0.45f;
    [SerializeField] private float topLimitY = 5.2f;
    [SerializeField] private float searchRadius = 1.0f;
    [SerializeField] private float occupiedCheckRadius = 0.22f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ceiling") && !collision.gameObject.CompareTag("Bubble"))
            return;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        Vector3 snappedPos;
        if (collision.gameObject.CompareTag("Ceiling"))
        {
            Vector3 pos = transform.position;
            float snappedX = Mathf.Round(pos.x / gridSize) * gridSize;
            snappedPos = new Vector3(snappedX, topLimitY, 0f);
        }
        else
        {
            Vector3 hitBubblePos = collision.transform.position;
            Vector3 dir = (transform.position - hitBubblePos).normalized;
            if (dir.sqrMagnitude < 0.001f)
                dir = Vector3.up;
            Vector3 candidatePos = hitBubblePos + dir * gridSize;
            snappedPos = SnapToNearestFree(candidatePos);
        }
        transform.position = snappedPos;
        CheckMatchAndPop();
        if (shooter != null)
        {
            shooter.AllowShootAgain();
        }
    }
    Vector3 SnapToNearestFree(Vector3 originalPos)
    {
        float snappedX = Mathf.Round(originalPos.x / gridSize) * gridSize;
        float snappedY = Mathf.Round(originalPos.y / gridSize) * gridSize;
        if (snappedY > topLimitY)
            snappedY = topLimitY;
        Vector3 centerPos = new Vector3(snappedX, snappedY, 0f);
        if (!IsPositionOccupied(centerPos))
            return centerPos;
        Vector3[] offsets = new Vector3[]
        {
 new Vector3(gridSize, 0f, 0f),
 new Vector3(-gridSize, 0f, 0f),
 new Vector3(0f, gridSize, 0f),
 new Vector3(gridSize, gridSize, 0f),
 new Vector3(-gridSize, gridSize, 0f),
 new Vector3(gridSize, -gridSize, 0f),
 new Vector3(-gridSize, -gridSize, 0f)
        };
        foreach (Vector3 offset in offsets)
        {
            Vector3 testPos = centerPos + offset;
            if (testPos.y > topLimitY)
                testPos.y = topLimitY;
            if (!IsPositionOccupied(testPos))
                return testPos;
        }
        return centerPos;
    }
    bool IsPositionOccupied(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, occupiedCheckRadius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Bubble")) continue;
            if (hit.gameObject == gameObject) continue;
            return true;
        }
        return false;
    }
    void CheckMatchAndPop()
    {
        BubbleColor myColor = GetComponent<BubbleColor>();
        if (myColor == null) return;
        List<GameObject> cluster = FindConnectedSameColor(gameObject, myColor.colorId);
        if (cluster.Count >= 3)
        {
            foreach (GameObject bubble in cluster)
            {
                if (bubble != null)
                    Destroy(bubble);
            }
        }
    }
    List<GameObject> FindConnectedSameColor(GameObject startBubble, string targetColor)
    {
        List<GameObject> result = new List<GameObject>();
        Queue<GameObject> queue = new Queue<GameObject>();
        HashSet<GameObject> visited = new HashSet<GameObject>();
        queue.Enqueue(startBubble);
        visited.Add(startBubble);
        while (queue.Count > 0)
        {
            GameObject current = queue.Dequeue();
            result.Add(current);
            Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, searchRadius);
            foreach (Collider2D hit in hits)
            {
                if (!hit.CompareTag("Bubble")) continue;
                GameObject other = hit.gameObject;
                if (other == current) continue;
                if (visited.Contains(other)) continue;
                BubbleColor otherColor = other.GetComponent<BubbleColor>();
                if (otherColor == null) continue;
                if (otherColor.colorId != targetColor) continue;
                visited.Add(other);
                queue.Enqueue(other);
            }
        }
        return result;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LoseLine"))
        {
            Debug.Log("GAME OVER");
            Time.timeScale = 0f;
        }
    }
}