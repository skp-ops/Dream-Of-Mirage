using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public float speed;
	public float jumpVelocity;
	public float acceleration;

	/*
	I don't want to use coyote time. (reaction time is infinite)
	when the player is not on the floor, they can always jump, so coyote time is not needed.
	*/
	public int jumpCount;
	public bool wasOnFloor;
	// Jump buffer
	public float jumpBufferTime;
	public float jumpBufferCounter;

	public override void _Ready()
	{
		speed = 100.0f;
		jumpVelocity = -285.0f;
		acceleration = speed / 0.1f;
		// initial state
		jumpCount = 2;
		wasOnFloor = IsOnFloor();
		jumpBufferTime = 0.15f;
		jumpBufferCounter = 0f;
	}

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	public int GetJumpCount()
	{
		return jumpCount;
	}

	public void SetJumpCount(int count)
	{
		jumpCount = count;
		if (jumpCount < 0)
		{
			jumpCount = 0;
		}
	}

	public void HandleGravity(double delta)
	{
		if (!this.IsOnFloor())
		{
			this.Velocity += new Vector2(0, ConstVar.GRAVITY * (float)delta);
		}
	}
}