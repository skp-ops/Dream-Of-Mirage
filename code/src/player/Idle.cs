using Godot;
using System;

public partial class Idle : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // Idle -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = pNode.GetNode<Sprite2D>("Sprite2D");
        Logger.LogInfo("Query Player Node in [Idle] State done...");
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Player>(pNode);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("pNode: " + ex.Message);
        }
        try
        {
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("animationPlayer: " + ex.Message);
        }
        try
        {
            Assert.IsNoneNode<Sprite2D>(sprite);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("sprite2D: " + ex.Message);
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
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if (direction != 0)
        {
            fsm.ChangeState("Run");
            return;
        }
        if (pNode.IsOnFloor() == false)
        {
            fsm.ChangeState("Jump");
            return;
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        // Apply gravity when in air and decelerate horizontally to 0 while idling
        var velocity = pNode.Velocity;
        if (!pNode.IsOnFloor())
        {
            velocity.Y += ConstVar.GRAVITY * (float)delta;
        }
        velocity.X = Mathf.MoveToward(velocity.X, 0, pNode.acceleration * (float)delta);
        pNode.Velocity = velocity;

        if (animationPlayer.CurrentAnimation != "idle")
        {
            animationPlayer.Play("idle");
        }
        pNode.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("KeyJump"))
        {
            pNode.Velocity = new Vector2(pNode.Velocity.X, pNode.jumpVelocity);
            fsm.ChangeState("Jump");
            return;
        }
    }
}
