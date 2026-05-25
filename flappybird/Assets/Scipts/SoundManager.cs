using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.35f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private AudioClip flyClip;
    private AudioClip scoreClip;
    private AudioClip gameOverClip;
    private AudioClip startClip;
    private AudioClip backgroundMusicClip;

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            UpdateVolumes();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        LoadAudioClips();
        UpdateVolumes();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateSoundManagerBeforeSceneLoad()
    {
        if (Instance != null)
            return;

        GameObject soundManagerObject = new GameObject("SoundManager");
        soundManagerObject.AddComponent<SoundManager>();
    }

    private void CreateAudioSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void LoadAudioClips()
    {
        flyClip = Resources.Load<AudioClip>("Audio/fly");
        scoreClip = Resources.Load<AudioClip>("Audio/score");
        gameOverClip = Resources.Load<AudioClip>("Audio/game_over");
        startClip = Resources.Load<AudioClip>("Audio/start");
        backgroundMusicClip = Resources.Load<AudioClip>("Audio/background_music");
    }

    private void UpdateVolumes()
    {
        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;

        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;
    }

    public void PlayFlySound()
    {
        PlaySfx(flyClip);
    }

    public void PlayScoreSound()
    {
        PlaySfx(scoreClip);
    }

    public void PlayGameOverSound()
    {
        PlaySfx(gameOverClip);
    }

    public void PlayStartSound()
    {
        PlaySfx(startClip);
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusicClip == null)
            return;

        if (musicSource.clip == backgroundMusicClip && musicSource.isPlaying)
            return;

        musicSource.clip = backgroundMusicClip;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }
}
