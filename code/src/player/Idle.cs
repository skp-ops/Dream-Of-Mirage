using Godot;
using System;

public partial class Idle : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // Idle -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = pNode.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = pNode.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        Logger.LogInfo("Query Player Node in [Idle] State done...");
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
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate(double delta)
    {
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if (direction != 0)
        {
            fsm.ChangeState(StateName.RUN);
            return;
        }
        if (pNode.IsOnFloor() == false)
        {
            fsm.ChangeState(StateName.FALL);
            return;
        }
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
        // Apply gravity when in air and decelerate horizontally to 0 while idling
        HandleGravity(delta);

        // smoothly play idle animation
        if (animationPlayer.CurrentAnimation != AnimationName.IDLE)
        {
            animationPlayer.Play(AnimationName.IDLE);
        }
        pNode.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        /*
        Input handling for jump action, the code below must be in this function,
        otherwise the jump input may be missed if put in StatePhysicsUpdate() or StateUpdate().
        */

        if (Input.IsActionJustPressed("KeyJump"))
        {
            pNode.Velocity = new Vector2(pNode.Velocity.X, pNode.jumpVelocity);
            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
