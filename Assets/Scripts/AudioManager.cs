using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;


    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;


    public AudioClip backgroundMusic;
    public AudioClip clickSound;
    public AudioClip launchSound;
    public AudioClip bellLaunchSound;
    public AudioClip landSound;
    public AudioClip vanishSound;
    public AudioClip gameWinSound;
    public AudioClip[] waterLandSounds;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSfx(AudioClip[] clips)
    {
        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
        PlaySFXWithRandomPitch(clipToPlay);
    }

    public void PlaySFXWithRandomPitch(AudioClip clip) {
        sfxSource.pitch = Random.Range(0.9f, 1.1f); // Slight variation
        sfxSource.PlayOneShot(clip);
    }
}
