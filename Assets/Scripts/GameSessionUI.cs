using TMPro;
using UnityEngine;

public class GameSessionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI centerMessageText;

    [Header("Countdown")]
    [SerializeField] private float countdownDuration = 3f;

    [Header("Start Button")]
    [SerializeField] private GameObject startButtonRoot;

    private float _countdownRemaining = 0f;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTrial();
        }

        RefreshStartButtonVisibility();
        UpdateIdleMessage();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        HandleCountdown();
        UpdateHUD();
        RefreshStartButtonVisibility();
    }

    public void RequestStartGame()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.IsTrialRunning || GameManager.Instance.IsCountdownRunning)
            return;

        StartCountdown();
    }

    private void StartCountdown()
    {
        _countdownRemaining = countdownDuration;
        GameManager.Instance.BeginCountdown();
    }

    private void HandleCountdown()
    {
        if (!GameManager.Instance.IsCountdownRunning)
            return;

        _countdownRemaining -= Time.deltaTime;

        if (_countdownRemaining > 0f)
        {
            if (centerMessageText != null)
            {
                centerMessageText.text = Mathf.CeilToInt(_countdownRemaining).ToString();
            }
        }
        else
        {
            GameManager.Instance.StartTrial();

            if (centerMessageText != null)
            {
                centerMessageText.text = "";
            }
        }
    }

    private void UpdateHUD()
    {
        if (statsText != null)
        {
            statsText.text =
                $"Hits: {GameManager.Instance.ValidHitCount}/{GameManager.Instance.HitsRequiredToFinish}\n" +
                $"Time: {GameManager.Instance.TrialTimeElapsed:F2}s";
        }

        if (centerMessageText == null)
            return;

        if (GameManager.Instance.IsTrialFinished)
        {
            centerMessageText.text =
                $"Trial Complete\nTime: {GameManager.Instance.TrialTimeElapsed:F2}s\nTouch Start to Restart";
        }
        else if (!GameManager.Instance.IsTrialRunning &&
                 !GameManager.Instance.IsCountdownRunning &&
                 !GameManager.Instance.IsTrialFinished)
        {
            UpdateIdleMessage();
        }
    }

    private void UpdateIdleMessage()
    {
        if (centerMessageText != null)
        {
            centerMessageText.text = "Touch Start to Begin";
        }
    }

    private void RefreshStartButtonVisibility()
    {
        if (startButtonRoot == null || GameManager.Instance == null)
            return;

        bool shouldShow =
            !GameManager.Instance.IsTrialRunning &&
            !GameManager.Instance.IsCountdownRunning;

        if (startButtonRoot.activeSelf != shouldShow)
        {
            startButtonRoot.SetActive(shouldShow);
        }
    }
}