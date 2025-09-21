using Godot;
using System;

public partial class Jump : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;

    /*
	I don't want to use coyote time. (reaction time is infinite)
	when the player is not on the floor, they can always jump, so coyote time is not needed.
	*/
    public int jumpCount;
    // Jump buffer
    public float jumpBufferTime;
    public float jumpBufferCounter;
    public bool wasOnFloor;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // Jump -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = pNode.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = pNode.GetNode<Node2D>(PlayerNodeName.GRAPHIC);

        jumpCount = 2;
        jumpBufferTime = 0.15f;
        jumpBufferCounter = 0;
        wasOnFloor = pNode.IsOnFloor();
        Logger.LogInfo("Query Player Node in [Jump] State done...");
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Player>(pNode);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
            Assert.IsNoneNode<Node2D>(graphic);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }
    }

    public override void StateEnter()
    {
        // Do not reset here; Fall will set remaining jumps on landing
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate(double delta)
    {
        // player not on floor and start to fall (Y > 0), then switch to Fall
        if ((pNode.IsOnFloor() == false) && pNode.Velocity.Y > 0f)
        {
            fsm.ChangeState(StateName.FALL);
            return;
        }
    }

    private void HandleJump()
    {
        pNode.Velocity = new Vector2(pNode.Velocity.X, pNode.jumpVelocity);
        jumpBufferCounter = 0f;
    }

    private void HandleGravity(double delta)
    {
        if (!pNode.IsOnFloor())
        {
            pNode.Velocity += new Vector2(0, ConstVar.GRAVITY * (float)delta);
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        var v = pNode.Velocity;
        if (direction != 0)
        {
            graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
            v.X = Mathf.MoveToward(v.X, direction * pNode.speed, pNode.acceleration * (float)delta);
        }
        else
        {
            v.X = Mathf.MoveToward(v.X, 0, pNode.acceleration * 0.9f * (float)delta);
        }
        pNode.Velocity = v;

        // handle jump buffer
        if (Input.IsActionJustPressed("KeyJump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - (float)delta);
        }

        bool executeJump = false;
        // if jump buffer time is valid and have jump count, then jump
        if ((jumpBufferCounter > 0f) && (jumpCount > 0))
        {
            executeJump = true;
        }
        if (executeJump)
        {
            HandleJump();
            jumpCount--;
            Input.ActionRelease("KeyJump");
        }

        // reset of remaining jumps is handled in Fall on landing
		// update the previous frame's on-floor status
		wasOnFloor = pNode.IsOnFloor();
        // gravity
        HandleGravity(delta);

        if (animationPlayer.CurrentAnimation != AnimationName.JUMP)
            animationPlayer.Play(AnimationName.JUMP);
        pNode.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
    }
}
