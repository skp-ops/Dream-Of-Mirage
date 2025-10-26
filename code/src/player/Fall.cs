using Godot;
using System;

public partial class Fall : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;
    // Reference to Jump state for shared jump counters
    private Jump jumpState;
    private RayCast2D handCheck;
    private RayCast2D footCheck;

    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // Fall -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = player.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        handCheck = player.GetNode<RayCast2D>(PlayerNodeName.HAND_CHECK);
        footCheck = player.GetNode<RayCast2D>(PlayerNodeName.FOOT_CHECK);
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Player>(player);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
            Assert.IsNoneNode<Node2D>(graphic);
            Assert.IsNoneNode<Jump>(jumpState);
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
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate(double delta)
    {
        if (player.IsOnFloor() && Mathf.IsZeroApprox(player.Velocity.Y))
        {
            float direction = Input.GetAxis("KeyLeft", "KeyRight");
            if (Mathf.IsZeroApprox(direction))
            {
                // Stop horizontal drift on landing when no input is held
                if (!Mathf.IsZeroApprox(player.Velocity.X))
                {
                    player.Velocity = new Vector2(0, player.Velocity.Y);
                }
                fsm.ChangeState(StateName.IDLE);
            }
            else
            {
                // If player is holding a direction, continue into Run state immediately on landing
                fsm.ChangeState(StateName.RUN);
            }
            return;
        }
        if ((player.IsOnWall() == true) && (player.IsOnFloor() == false) &&
            (handCheck.IsColliding() == true) && (footCheck.IsColliding() == true))
        {
            fsm.ChangeState(StateName.ONWALL);
            return;
        }
    }

    private void HandleGravity(double delta)
    {
        if (player.IsOnFloor() == false)
        {
            player.Velocity += new Vector2(0, ConstVar.GRAVITY * (float)delta);
        }
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

        HandleGravity(delta);
        if (animationPlayer.CurrentAnimation != AnimationName.FALL)
        {
            animationPlayer.Play(AnimationName.FALL);
        }
        player.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("KeyJump") && player.GetJumpCount() > 0)
        {
            // Allow air-jump only if we have remaining jumps
            // Give a vertical velocity boost for the jump, then switch to Jump state
            player.Velocity = new Vector2(player.Velocity.X, player.jumpVelocity);
            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
