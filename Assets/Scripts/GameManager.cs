using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Trial Settings")]
    [SerializeField] private int hitsRequiredToFinish = 9;

    public int HitsRequiredToFinish => hitsRequiredToFinish;
    public int ValidHitCount { get; private set; }

    public bool IsCountdownRunning { get; private set; }
    public bool IsTrialRunning { get; private set; }
    public bool IsTrialFinished { get; private set; }

    public float TrialTimeElapsed { get; private set; }
    public int TrialIndex { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!IsTrialRunning)
            return;

        TrialTimeElapsed += Time.deltaTime;
    }

    public void ResetTrial()
    {
        ValidHitCount = 0;
        TrialTimeElapsed = 0f;

        IsCountdownRunning = false;
        IsTrialRunning = false;
        IsTrialFinished = false;
    }

    public void BeginCountdown()
    {
        ResetTrial();
        IsCountdownRunning = true;
    }

    public void StartTrial()
    {
        TrialIndex++;

        ValidHitCount = 0;
        TrialTimeElapsed = 0f;

        IsCountdownRunning = false;
        IsTrialRunning = true;
        IsTrialFinished = false;

        Debug.Log($"Trial {TrialIndex} started");
    }

    public void RegisterValidHit()
    {
        if (!IsTrialRunning)
            return;

        ValidHitCount++;

        Debug.Log($"Valid hit: {ValidHitCount}/{hitsRequiredToFinish}");

        if (ValidHitCount >= hitsRequiredToFinish)
        {
            FinishTrial();
        }
    }

    public void FinishTrial()
    {
        if (!IsTrialRunning)
            return;

        IsTrialRunning = false;
        IsCountdownRunning = false;
        IsTrialFinished = true;

        Debug.Log($"Trial {TrialIndex} finished in {TrialTimeElapsed:F2} seconds");

        if (TrialResultLogger.Instance != null)
        {
            TrialResultLogger.Instance.AppendTrialResult(
                TrialIndex,
                TrialTimeElapsed,
                ValidHitCount
            );
        }
        else
        {
            Debug.LogWarning("TrialResultLogger not found. Result not saved.");
        }
    }
}