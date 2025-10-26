using Godot;
using System;

public partial class Jump : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;

    /*
	I don't want to use coyote time. (reaction time is infinite)
	when the player is not on the floor, they can always jump, so coyote time is not needed.
	*/
    // Jump buffer
    public float jumpBufferTime;
    public float jumpBufferCounter;
    public bool wasOnFloor;
    private RayCast2D handCheck;
    private RayCast2D footCheck;

    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // Jump -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = player.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        handCheck = player.GetNode<RayCast2D>(PlayerNodeName.HAND_CHECK);
        footCheck = player.GetNode<RayCast2D>(PlayerNodeName.FOOT_CHECK);


        jumpBufferTime = 0.15f;
        jumpBufferCounter = 0;
        wasOnFloor = player.IsOnFloor();
        Logger.LogInfo("Query Player Node in [Jump] State done...");
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Player>(player);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
            Assert.IsNoneNode<Node2D>(graphic);
            Assert.IsNoneNode<RayCast2D>(handCheck);
            Assert.IsNoneNode<RayCast2D>(footCheck);
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
        jumpBufferTime = 0.15f;
        jumpBufferCounter = 0;
    }

    public override void StateUpdate(double delta)
    {
        // player not on floor and start to fall (Y > 0), then switch to Fall
        if ((player.IsOnFloor() == false) && player.Velocity.Y > 0f)
        {
            fsm.ChangeState(StateName.FALL);
            return;
        }
        if ((player.IsOnWall() == true) && (player.IsOnFloor() == false) &&
            (handCheck.IsColliding() == true) && (footCheck.IsColliding() == true) &&
            (player.Velocity.Y == 0))
        {
            fsm.ChangeState(StateName.ONWALL);
            return;
        }
    }

    private void HandleJump()
    {
        player.Velocity = new Vector2(player.Velocity.X, player.jumpVelocity);
        jumpBufferCounter = 0f;
    }

    public override void StatePhysicsUpdate(double delta)
    {
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        var v = player.Velocity;
        if (direction != 0)
        {
            graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
            v.X = Mathf.MoveToward(v.X, direction * player.speed, player.acceleration * (float)delta);
        }
        else
        {
            v.X = Mathf.MoveToward(v.X, 0, player.acceleration * 0.9f * (float)delta);
        }
        player.Velocity = v;

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
        if ((jumpBufferCounter > 0f) && (player.GetJumpCount() > 0))
        {
            executeJump = true;
        }
        if (executeJump)
        {
            HandleJump();
            player.SetJumpCount(player.GetJumpCount() - 1);
            Input.ActionRelease("KeyJump");
        }

        // reset of remaining jumps is handled in Fall on landing
        // update the previous frame's on-floor status
        wasOnFloor = player.IsOnFloor();
        // gravity
        player.HandleGravity(delta);

        if (animationPlayer.CurrentAnimation != AnimationName.JUMP)
            animationPlayer.Play(AnimationName.JUMP);
        player.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
    }
}
