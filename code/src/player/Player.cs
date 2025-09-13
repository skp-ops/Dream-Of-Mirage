using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public float speed;
	public float jumpVelocity;
	public int jumpCount = 0;
	private bool wasOnFloor = false;
	// Jump buffer
	private float jumpBufferTime = 0.15f; // 缓冲时长（秒）
	private float jumpBufferCounter = 0f;

	AnimationPlayer animatedSprite;
	Sprite2D sprite;

	public void CheckNode()
	{
		try
		{
			Assert.IsNoneNode<AnimationPlayer>(animatedSprite);
		}
		catch (NullReferenceException ex)
		{
			Logger.LogError("animatedSprite: " + ex.Message);
		}
		try
		{
			Assert.IsNoneNode<Sprite2D>(sprite);
		}
		catch (NullReferenceException ex)
		{
			Logger.LogError("sprite: " + ex.Message);
		}
	}

	public override void _Ready()
	{
		speed = 100.0f;
		jumpVelocity = -200.0f;
		animatedSprite = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite2D>("Sprite2D");
		CheckNode();
	}

	public override void _Process(double delta)
	{
	}

	public void PlayerMove(Vector2 velocity, float direction, double delta)
	{
		// horizontal movement
		if (direction != 0)
		{
			velocity.X = direction * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, speed);
		}

		// Send a jump request, reset jump buffer counter
		if (Input.IsActionJustPressed("KeyJump"))
		{
			jumpBufferCounter = jumpBufferTime;
		}
		else
		{
			jumpBufferCounter -= (float)delta;
		}

		// Jump execution: if there's a jump buffer and available jumps -> execute jump
		bool executeJump = false;
		if ((jumpBufferCounter > 0f) && (jumpCount > 0))
		{
			executeJump = true;
		}

		if (executeJump)
		{
			jumpCount--;
			velocity.Y = jumpVelocity;
			jumpBufferCounter = 0f; // 跳跃后清空缓冲
		}

		// only reset jump count when landing
		if (!wasOnFloor && IsOnFloor())
		{
			jumpCount = 2;
		}
		// update the previous frame's on-floor status
		wasOnFloor = IsOnFloor();

		if (!IsOnFloor())
		{
			velocity.Y += ConstVar.GRAVITY * (float)delta;
		}

		this.Velocity = velocity;
		MoveAndSlide();
	}

	public void AnimationPlay(float direction)
	{
		if (!IsOnFloor())
		{
			if (direction != 0)
				sprite.FlipH = direction < 0;
			animatedSprite.Play("jump");
		}
		else if (direction == 0)
		{
			animatedSprite.Play("idle");
		}
		else
		{
			sprite.FlipH = direction < 0;
			animatedSprite.Play("run");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = this.Velocity;
		float direction = Input.GetAxis("KeyLeft", "KeyRight");
		PlayerMove(velocity, direction, delta);
		AnimationPlay(direction);
	}
}