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

    [Header("Restart Protection")]
    [SerializeField] private float restartButtonDelayAfterFinish = 1.5f;

    private float _countdownRemaining = 0f;
    private float _restartDelayTimer = 0f;
    private bool _restartDelayStartedForCurrentFinish = false;

    public bool IsStartInteractionLocked
    {
        get
        {
            if (GameManager.Instance == null)
                return true;

            return GameManager.Instance.IsCountdownRunning ||
                   GameManager.Instance.IsTrialRunning ||
                   _restartDelayTimer > 0f;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTrial();
        }

        _countdownRemaining = 0f;
        _restartDelayTimer = 0f;
        _restartDelayStartedForCurrentFinish = false;

        RefreshStartButtonVisibility();
        UpdateIdleMessage();
        UpdateStatsVisibility();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        HandleCountdown();
        HandleRestartDelay();
        UpdateHUD();
        UpdateStatsVisibility();
        RefreshStartButtonVisibility();
    }

    public bool RequestStartGame()
    {
        if (GameManager.Instance == null)
            return false;

        if (IsStartInteractionLocked)
            return false;

        StartCountdown();
        return true;
    }

    private void StartCountdown()
    {
        _countdownRemaining = countdownDuration;
        _restartDelayTimer = 0f;
        _restartDelayStartedForCurrentFinish = false;
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
            _restartDelayStartedForCurrentFinish = false;

            if (centerMessageText != null)
            {
                centerMessageText.text = "";
            }
        }
    }

    private void HandleRestartDelay()
    {
        if (!GameManager.Instance.IsTrialFinished)
        {
            _restartDelayStartedForCurrentFinish = false;
            return;
        }

        if (!_restartDelayStartedForCurrentFinish)
        {
            _restartDelayTimer = restartButtonDelayAfterFinish;
            _restartDelayStartedForCurrentFinish = true;
        }

        if (_restartDelayTimer > 0f)
        {
            _restartDelayTimer -= Time.deltaTime;

            if (_restartDelayTimer < 0f)
            {
                _restartDelayTimer = 0f;
            }
        }
    }

    private void UpdateHUD()
    {
        if (statsText != null && GameManager.Instance.IsTrialRunning)
        {
            statsText.text =
                $"Hits: {GameManager.Instance.ValidHitCount}/{GameManager.Instance.HitsRequiredToFinish}\n" +
                $"Time: {GameManager.Instance.TrialTimeElapsed:F2}s";
        }

        if (centerMessageText == null)
            return;

        if (GameManager.Instance.IsTrialFinished)
        {
            if (_restartDelayTimer > 0f)
            {
                centerMessageText.text =
                    $"Trial Complete\nTime: {GameManager.Instance.TrialTimeElapsed:F2}s\nPlease wait...";
            }
            else
            {
                centerMessageText.text =
                    $"Trial Complete\nTime: {GameManager.Instance.TrialTimeElapsed:F2}s\nTouch Start to Restart";
            }
        }
        else if (!GameManager.Instance.IsTrialRunning &&
                 !GameManager.Instance.IsCountdownRunning)
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

    private void UpdateStatsVisibility()
    {
        if (statsText == null || GameManager.Instance == null)
            return;

        statsText.gameObject.SetActive(GameManager.Instance.IsTrialRunning);
    }

    private void RefreshStartButtonVisibility()
    {
        if (startButtonRoot == null || GameManager.Instance == null)
            return;

        bool shouldShow = !GameManager.Instance.IsCountdownRunning &&
                          !GameManager.Instance.IsTrialRunning;

        if (startButtonRoot.activeSelf != shouldShow)
        {
            startButtonRoot.SetActive(shouldShow);
        }
    }
}