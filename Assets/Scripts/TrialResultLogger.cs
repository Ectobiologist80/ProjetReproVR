using System;
using System.IO;
using UnityEngine;

public class TrialResultLogger : MonoBehaviour
{
    public static TrialResultLogger Instance { get; private set; }

    [Header("File Settings")]
    [SerializeField] private string fileName = "trial_results.csv";

    [Header("Debug")]
    [SerializeField] private bool logFilePathOnStart = true;
    [SerializeField] private bool logEachWrite = true;

    private string _filePath;
    private bool _initialized = false;

    public string FilePath => _filePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(_filePath))
        {
            WriteHeader();
        }

        _initialized = true;

        if (logFilePathOnStart)
        {
            Debug.Log($"Trial results file: {_filePath}");
        }
    }

    private void WriteHeader()
    {
        string header = "timestamp;trial_index;completion_time_s;valid_hits\n";
        File.WriteAllText(_filePath, header);
    }

    public void AppendTrialResult(int trialIndex, float completionTimeSeconds, int validHits)
    {
        if (!_initialized)
        {
            Initialize();
        }

        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        string line = $"{timestamp};{trialIndex};{completionTimeSeconds:F2};{validHits}\n";

        File.AppendAllText(_filePath, line);

        if (logEachWrite)
        {
            Debug.Log($"Saved trial result: {line.Trim()}");
        }
    }

    [ContextMenu("Open Log Path In Console")]
    private void LogPath()
    {
        if (!_initialized)
        {
            Initialize();
        }

        Debug.Log($"Trial results file path: {_filePath}");
    }

    [ContextMenu("Delete Results File")]
    public void DeleteResultsFile()
    {
        if (!_initialized)
        {
            Initialize();
        }

        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
            Debug.Log("Deleted results file.");

            WriteHeader();
        }
    }
}