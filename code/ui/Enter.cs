using Godot;
using System;
using static Logger;

public partial class Enter : Control
{
	public override void _Ready()
	{
		LogInfo("Application started...");
		LogInfo("Logger Test Started");
		LogDebug("This is a debug message.");
		LogInfo("This is an info message.");
		LogWarning("This is a warning message.");
		LogError("This is an error message.");
		LogCritical("This is a critical message.");
		LogInfo("Logger Test Ended");
	}

	public override void _Process(double delta)
	{
	}
}
