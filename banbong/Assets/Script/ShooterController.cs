using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public GameObject[] bubblePrefabs;
    public Transform spawnPoint;
    public float shootForce = 10f;

    public LineRenderer aimLine;
    public float aimLength = 10f;

    private bool canShoot = true;

    private GameObject currentBubblePrefab;
    private GameObject currentLoadedBubble;

    void Start()
    {
        LoadNextBubble();
    }

    void Update()
    {
        DrawAimLine();

        if (canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void LoadNextBubble()
    {
        if (bubblePrefabs == null || bubblePrefabs.Length == 0 || spawnPoint == null)
            return;

        int randomIndex = Random.Range(0, bubblePrefabs.Length);
        currentBubblePrefab = bubblePrefabs[randomIndex];

        if (currentLoadedBubble != null)
        {
            Destroy(currentLoadedBubble);
        }

        currentLoadedBubble = Instantiate(currentBubblePrefab, spawnPoint.position, Quaternion.identity);
        currentLoadedBubble.transform.SetParent(spawnPoint);

        Rigidbody2D rb = currentLoadedBubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
        CircleCollider2D col = currentLoadedBubble.GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    void Shoot()
    {
        if (currentLoadedBubble == null)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - spawnPoint.position).normalized;

        currentLoadedBubble.transform.SetParent(null);

        Rigidbody2D rb = currentLoadedBubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = direction * shootForce;
        }

        BubbleStopOnCeiling stopScript = currentLoadedBubble.GetComponent<BubbleStopOnCeiling>();
        if (stopScript == null)
        {
            stopScript = currentLoadedBubble.AddComponent<BubbleStopOnCeiling>();
        }

        stopScript.shooter = this;

        currentLoadedBubble = null;
        canShoot = false;
    }

    public void AllowShootAgain()
    {
        canShoot = true;
        LoadNextBubble();
    }

    void DrawAimLine()
    {
        if (aimLine == null || spawnPoint == null)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - spawnPoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(spawnPoint.position, direction, aimLength);

        aimLine.SetPosition(0, spawnPoint.position);

        if (hit.collider != null)
        {
            aimLine.SetPosition(1, hit.point);

            if (hit.collider.gameObject.name.Contains("Wall"))
            {
                Vector2 reflectDir = Vector2.Reflect(direction, hit.normal);
                aimLine.SetPosition(2, hit.point + reflectDir * 5f);
            }
            else
            {
                aimLine.SetPosition(2, hit.point);
            }
        }
        else
        {
            Vector3 endPoint = spawnPoint.position + (Vector3)direction * aimLength;
            aimLine.SetPosition(1, endPoint);
            aimLine.SetPosition(2, endPoint);
        }
    }
}