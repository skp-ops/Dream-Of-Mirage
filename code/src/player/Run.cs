using Godot;
using System;

public partial class Run : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;
    // Reference to Jump state to manage remaining air jumps on walk-off
    private Jump jumpState;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // Run -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = pNode.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = pNode.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        jumpState = this.GetParent().GetNode<Jump>(StateName.JUMP);
        Logger.LogInfo("Query Player Node in [Run] State done...");
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
        if (pNode.IsOnFloor() == false)
        {
            jumpState.jumpCount = 2;
            fsm.ChangeState(StateName.FALL);
            return;
        }

        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if ((pNode.Velocity.X == 0) && (direction == 0))
        {
            fsm.ChangeState(StateName.IDLE);
            return;
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        Vector2 velocity = pNode.Velocity;
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        // horizontal movement
        if (direction != 0)
        {
            graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
            velocity.X = Mathf.MoveToward(velocity.X, direction * pNode.speed, pNode.acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, pNode.acceleration * (float)delta);
        }
        pNode.Velocity = velocity;
        if (animationPlayer.CurrentAnimation != AnimationName.RUN)
        {
            animationPlayer.Play(AnimationName.RUN);
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
            // Give a vertical velocity boost for the jump, then switch to Jump state
            pNode.Velocity = new Vector2(pNode.Velocity.X, pNode.jumpVelocity);
            // Prevent the same-frame buffered press from triggering another jump in Jump state
            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
