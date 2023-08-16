using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    private readonly Dictionary<string, string> debugLogs = new();
    public Text display;

    private void Update()
    {
        Debug.Log("time:" + Time.time);
        Debug.Log(gameObject.name);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            var splitString = logString.Split(char.Parse(":"));
            var debugKey = splitString[0];
            var debugValue = splitString.Length > 1 ? splitString[1] : "";

            if (debugLogs.ContainsKey(debugKey))
                debugLogs[debugKey] = debugValue;
            else
                debugLogs.Add(debugKey, debugValue);
        }

        var displayText = "";
        foreach (var log in debugLogs)
            if (log.Value == "")
                displayText += log.Key + "\n";
            else
                displayText += log.Key + ": " + log.Value + "\n";

        display.text = displayText;
    }
}