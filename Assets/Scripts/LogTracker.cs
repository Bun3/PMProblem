using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LogTracker : MonoBehaviour
{
	void Awake()
	{
		Application.logMessageReceived += LogCaughtException;
	}

	void LogCaughtException(string logText, string stackTrace, LogType logType)
	{
		if (logType == LogType.Exception)
		{
			return;
		}
	}
}
