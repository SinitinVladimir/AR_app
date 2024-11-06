using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogDisplay : MonoBehaviour
{
    public TextMeshProUGUI debugText; 
    private Queue<string> logQueue = new Queue<string>(); 
    public int maxLogs = 15; 

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        logQueue.Enqueue(logString); 

        if (logQueue.Count > maxLogs)
        {
            logQueue.Dequeue();
        }

        debugText.text = string.Join("\n", logQueue.ToArray());
    }
}
