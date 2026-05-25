using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Animation")]
    [SerializeField] private float flapAnimTime = 0.2f;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private Rigidbody2D rb;
    private Animator animator;
    private Coroutine flapCoroutine;

    private bool isDead;
    private bool hasStarted;
    private int score;

    public bool HasStarted => hasStarted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        score = 0;
        HideGameOverUI();
        UpdateScoreUI();

        SetFlapping(false);
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

        if (!started)
        {
            StopFlapAnimation();
        }

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

        PlayFlapAnimation();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayFlySound();
        }
    }

    private void PlayFlapAnimation()
    {
        if (flapCoroutine != null)
        {
            StopCoroutine(flapCoroutine);
        }

        flapCoroutine = StartCoroutine(FlapOnce());
    }

    private IEnumerator FlapOnce()
    {
        SetFlapping(true);

        yield return new WaitForSeconds(flapAnimTime);

        SetFlapping(false);
        flapCoroutine = null;
    }

    private void StopFlapAnimation()
    {
        if (flapCoroutine != null)
        {
            StopCoroutine(flapCoroutine);
            flapCoroutine = null;
        }

        SetFlapping(false);
    }

    private void SetFlapping(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("IsFlapping", value);
        }
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

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayScoreSound();
            }

            Debug.Log("Score: " + score);
        }
    }

    private void GameOver()
    {
        isDead = true;

        StopFlapAnimation();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameOverSound();
            SoundManager.Instance.StopBackgroundMusic();
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