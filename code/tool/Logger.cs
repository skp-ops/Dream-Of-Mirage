using Godot;
using System;
using System.IO;

public partial class Logger
{
	public enum LogLevel
	{
		DEBUG,
		INFO,
		WARN,
		ERROR,
		CRITICAL
	}

	private static readonly Lazy<Logger> _instance = new(() => new Logger());
	private readonly string _logDirectory;
	private readonly string _logFilePath;

	public static Logger Instance => _instance.Value;

	private Logger()
	{
		_logDirectory = ProjectSettings.GlobalizePath("res://logs");
		if (!Directory.Exists(_logDirectory))
		{
			Directory.CreateDirectory(_logDirectory);
			GD.Print($"Created log directory at {_logDirectory}");
		}
		// only one log file per run
		string logFileName = $"DOM_{DateTime.Now:yyyyMMdd_HHmmss}.log";
		_logFilePath = Path.Combine(_logDirectory, logFileName);
	}

	public void Log(LogLevel level,
					string logFile = "",
					int lineNumber = 0,
					params object[] messages)
	{
		// format log message
		string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadLeft(20);
		string fileInfo = (Path.GetFileName(logFile) + $":{lineNumber}").PadLeft(20);
		string logLevel = level.ToString().PadLeft(8);
		string combinedMessage = "";
		if (messages == null || messages.Length == 0)
		{
			combinedMessage = "<no message>";
		}
		else
		{
			try
			{
				combinedMessage = string.Join(" ", Array.ConvertAll(messages, m => m == null ? "<null>" : m.ToString()));
			}
			catch (Exception ex)
			{
				combinedMessage = $"<message format error: {ex.Message}>";
			}
		}
		string formatted = $"[{timeStamp}][{logLevel}][{fileInfo}] {combinedMessage}";

		switch (level)
		{
			case LogLevel.WARN:
				GD.PushWarning(formatted);
				break;
			case LogLevel.ERROR:
			case LogLevel.CRITICAL:
				GD.PushError(formatted);
				break;
			default:
				GD.Print(formatted);
				break;
		}

		// file output
		try
		{
			File.AppendAllText(_logFilePath, formatted + System.Environment.NewLine);
		}
		catch (Exception e)
		{
			GD.PushError($"Logger failed to write file: {e.Message}");
		}
	}

	public static void LogDebug(params object[] messages)
	{
		GetCallerInfo(out string logFile, out int lineNumber);
		Instance.Log(LogLevel.DEBUG, logFile, lineNumber, messages);
	}
	public static void LogInfo(params object[] messages)
	{
		GetCallerInfo(out string logFile, out int lineNumber);
		Instance.Log(LogLevel.INFO, logFile, lineNumber, messages);
	}
	public static void LogWarning(params object[] messages)
	{
		GetCallerInfo(out string logFile, out int lineNumber);
		Instance.Log(LogLevel.WARN, logFile, lineNumber, messages);
	}
	public static void LogError(params object[] messages)
	{
        GetCallerInfo(out string logFile, out int lineNumber);
        Instance.Log(LogLevel.ERROR, logFile, lineNumber, messages);
	}
	public static void LogCritical(params object[] messages)
	{
        GetCallerInfo(out string logFile, out int lineNumber);
        Instance.Log(LogLevel.CRITICAL, logFile, lineNumber, messages);
	}

	private static void GetCallerInfo(out string file, out int line)
	{
		file = "";
		line = 0;
	#if NET6_0_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
		var st = new System.Diagnostics.StackTrace(2, true);
	#else
		var st = new System.Diagnostics.StackTrace(true);
	#endif
		if (st.FrameCount > 0)
		{
			var frame = st.GetFrame(0);
			file = frame.GetFileName() ?? "";
			line = frame.GetFileLineNumber();
		}
	}
}
