using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Powerups")]
    public int bellcharges = 3;
    public float totalHeartsLit = 0f;

    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool gameFinished = false;
    private bool gameStarted = false;

    [Header("Difficulty Settings")]
    public Animator globeAnimator;

    [Header("Progress Control")]
    public float progressIncrement = 5f;
    public float bellBaseIncrement = 7f;
    public int longshotMultiplier = 2;
    public float longshotThreshold = 5f;


    public TextMeshProUGUI bellText;
    public TextMeshProUGUI percentageText;
    public UnityEngine.UI.Slider percentageSlider;
    public UnityEngine.UI.Image bellIcon;
    public GameObject startScreenPanel;
    public GameObject winscreenPanel;
    public TextMeshProUGUI finalTimeText;
    public GameObject aimer;

    void Awake()
    {
        Instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
        startScreenPanel.SetActive(true);
        gameStarted = false;
        PausePlayerInputs();
        UpdateUI();
    }

    public void AddProgress(float amount)
    {
        totalHeartsLit = Mathf.Clamp(totalHeartsLit + amount, 0, 100);
        UpdateUI();
        if (totalHeartsLit == 100)
        {
            WinGame();
        }
    }

    public void StartGame()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Instance.clickSound);
        startScreenPanel.SetActive(false);
        gameStarted = true;
        aimer.SetActive(true);
        ResumePlayerInputs();
        elapsedTime = 0f;
    }

    public void UseBell()
    {
        bellcharges--;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        bellIcon.enabled = PlayerController.Instance.isBellActive;
        if (gameStarted && !gameFinished)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }


    void UpdateUI()
    {
        bellText.text = "Bells: " + bellcharges;
        percentageText.text = totalHeartsLit.ToString("F0") + "%";
        percentageSlider.value = totalHeartsLit;
    }

    void UpdateTimerUI() {
        timerText.text = GetFormattedFinalTime();
    }

    String GetFormattedFinalTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    void PausePlayerInputs()
    {
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        playerInput.DeactivateInput();
    }

    void ResumePlayerInputs()
    {
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        playerInput.ActivateInput();
    }

    public void WinGame()
    {
        PausePlayerInputs();
        aimer.SetActive(false);
        gameFinished = true;
        winscreenPanel.SetActive(true);
        finalTimeText.text = string.Format("Time taken: {0}", GetFormattedFinalTime());
        AudioManager.Instance.musicSource.Pause();
        AudioManager.Instance.PlaySfx(AudioManager.Instance.gameWinSound);
    }

    public void SetDifficulty(int index) {
    // index 0 = Easy, 1 = Normal, 2 = Hard
    switch (index) {
        case 0:
            globeAnimator.speed = 0.25f; // Slow
            bellcharges = 5;
            progressIncrement = 7f;
            bellBaseIncrement = 9f;
            break;
        case 1:
            globeAnimator.speed = 1.0f; // Default
            bellcharges = 3;
            progressIncrement = 5f;
            bellBaseIncrement = 7f;
            break;
        case 2:
            globeAnimator.speed = 2.0f; // Fast
            bellcharges = 2;
            progressIncrement = 3f;
            bellBaseIncrement = 5f;
            break;
        case 3:
            globeAnimator.speed = 5.0f; // Faster
            bellcharges = 1;
            progressIncrement = 2f;
            bellBaseIncrement = 4f;
            break;
        case 4:
            globeAnimator.speed = 20.0f; // Fasterer
            bellcharges = 0;
            progressIncrement = 4f;
            bellBaseIncrement = 10000f;
            break;
    }
    Debug.Log("Difficulty set to index: " + index);
}
}
