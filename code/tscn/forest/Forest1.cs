using Godot;
using System;

public partial class Forest1 : Node2D
{
	// Get tilemap node
	private TileMapLayer tileMap;
	// Get camera node
	private Camera2D camera;

	public void CheckNode()
	{
		try
		{
			Assert.IsNoneNode<TileMapLayer>(tileMap);
		}
		catch (NullReferenceException ex)
		{
			Logger.LogError("tileMap: " + ex.Message);
		}
		try
		{
			Assert.IsNoneNode<Camera2D>(camera);
		}
		catch (NullReferenceException ex)
		{
			Logger.LogError("camera: " + ex.Message);
		}
	}

	public void SetCameraLimits()
	{
		// Calculate the map bounds in pixels
		Rect2 mapBounds = tileMap.GetUsedRect().Grow(-1);
		Logger.LogInfo($"Map Bounds: {mapBounds}");

		Vector2 cellSize = tileMap.TileSet.TileSize; // Get the parent TileMap's CellSize
		Logger.LogInfo($"Cell Size: {cellSize}");

		// Set the camera limits
		camera.LimitLeft = (int)(mapBounds.Position.X * cellSize.X);
		camera.LimitTop = (int)(mapBounds.Position.Y * cellSize.Y);
		camera.LimitRight = (int)((mapBounds.Position.X + mapBounds.Size.X) * cellSize.X);
		camera.LimitBottom = (int)((mapBounds.Position.Y + mapBounds.Size.Y) * cellSize.Y);
		camera.ResetSmoothing();
		Logger.LogInfo($"Camera Limits: Left={camera.LimitLeft}, Top={camera.LimitTop}, Right={camera.LimitRight}, Bottom={camera.LimitBottom}");
	}

	public override void _Ready()
	{
		Logger.LogInfo("Forest1 scene is ready.");
		tileMap = GetNode<TileMapLayer>(TscnNodeName.FOREST1_PHYSICS_LAYER);
		camera = GetNode<Camera2D>(PlayerNodeName.ROOT + "/" + PlayerNodeName.CAMERA);
		CheckNode();
		SetCameraLimits();
	}

	public override void _Process(double delta)
	{
	}
}
