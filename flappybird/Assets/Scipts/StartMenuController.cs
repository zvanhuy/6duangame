using UnityEngine;
using UnityEngine.InputSystem;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private BirdController birdController;
    [SerializeField] private PipeSpawner pipeSpawner;

    private bool started;

    private void Start()
    {
        Time.timeScale = 1f;
        started = false;

        if (startPanel != null)
            startPanel.SetActive(true);

        if (scoreText != null)
            scoreText.SetActive(false);

        if (pipeSpawner != null)
            pipeSpawner.enabled = false;

        if (birdController != null)
            birdController.SetGameplayStarted(false);
    }

    private void Update()
    {
        if (started)
            return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (started)
            return;

        started = true;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (scoreText != null)
            scoreText.SetActive(true);

        if (pipeSpawner != null)
            pipeSpawner.enabled = true;

        if (birdController != null)
            birdController.StartGame();
    }
}