using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private Rigidbody2D rb;
    private bool isDead;
    private bool hasStarted;
    private int score;

    public bool HasStarted => hasStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        score = 0;
        HideGameOverUI();
        UpdateScoreUI();
        SetGameplayStarted(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (isDead)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                RestartGame();
            }

            return;
        }

        if (!hasStarted)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Fly();
        }
    }

    public void SetGameplayStarted(bool started)
    {
        hasStarted = started;

        if (rb == null)
            return;

        if (!started)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
        else
        {
            rb.simulated = true;
        }
    }

    public void StartGame()
    {
        if (hasStarted || isDead)
            return;

        SetGameplayStarted(true);
        Fly();
    }

    private void Fly()
    {
        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || !hasStarted)
            return;

        GameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !hasStarted)
            return;

        if (other.CompareTag("ScoreZone"))
        {
            score++;
            UpdateScoreUI();
            Debug.Log("Score: " + score);
        }
    }

    private void GameOver()
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        ShowGameOverUI();
        Time.timeScale = 0f;
        Debug.Log("Game Over");
    }

    private void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }
    }

    private void HideGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}