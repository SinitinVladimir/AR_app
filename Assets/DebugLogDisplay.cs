using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class DebugLogDisplay : MonoBehaviour
{
    public TextMeshProUGUI debugText; // Use TextMeshProUGUI instead of standard Text
    private Queue<string> logQueue = new Queue<string>(); // Queue to store log entries
    public int maxLogs = 5; // Maximum number of logs displayed at once

    private void Awake()
    {
        Application.logMessageReceived += HandleLog; // Subscribe to log messages
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog; // Unsubscribe when object is destroyed
    }

    // Method to handle Unity log messages
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        logQueue.Enqueue(logString); // Add new log message to the queue

        // Remove old messages if there are too many logs in the queue
        if (logQueue.Count > maxLogs)
        {
            logQueue.Dequeue();
        }

        // Display the logs in the TextMeshPro UI component
        debugText.text = string.Join("\n", logQueue.ToArray());
    }
}
