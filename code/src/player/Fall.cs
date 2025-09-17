using Godot;
using System;

public partial class Fall : State
{
    // player node
    private Player pNode;
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
        pNode = this.GetParent().GetParent() as Player; // Fall -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = pNode.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = pNode.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        handCheck = pNode.GetNode<RayCast2D>(PlayerNodeName.HAND_CHECK);
        footCheck = pNode.GetNode<RayCast2D>(PlayerNodeName.FOOT_CHECK);

        // Try to fetch sibling Jump state to manage remaining air jumps
        try
        {
            jumpState = this.GetParent().GetNode<Jump>(StateName.JUMP);
        }
        catch (Exception)
        {
            jumpState = null;
        }
        Logger.LogInfo("Query Player Node in [Fall] State done...");
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Player>(pNode);
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
        if (pNode.IsOnFloor() && Mathf.IsZeroApprox(pNode.Velocity.Y))
        {
            float direction = Input.GetAxis("KeyLeft", "KeyRight");
            // On landing, reset remaining air jumps to 2 so walking off ledge grants two jumps
            jumpState.jumpCount = 2;

            if (Mathf.IsZeroApprox(direction))
            {
                // Stop horizontal drift on landing when no input is held
                if (!Mathf.IsZeroApprox(pNode.Velocity.X))
                {
                    pNode.Velocity = new Vector2(0, pNode.Velocity.Y);
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
        if ((pNode.IsOnWall() == true) && (pNode.IsOnFloor() == false) &&
            (handCheck.IsColliding() == true) && (footCheck.IsColliding() == true))
        {
            fsm.ChangeState(StateName.ONWALL);
            return;
        }
    }
    
    private void HandleGravity(double delta)
    {
        if (pNode.IsOnFloor() == false)
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

        HandleGravity(delta);
        if (animationPlayer.CurrentAnimation != AnimationName.FALL)
        {
            animationPlayer.Play(AnimationName.FALL);
        }
        pNode.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("KeyJump") && jumpState.jumpCount > 0)
        {
            // Allow air-jump only if we have remaining jumps
            // Give a vertical velocity boost for the jump, then switch to Jump state
            pNode.Velocity = new Vector2(pNode.Velocity.X, pNode.jumpVelocity);
            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
