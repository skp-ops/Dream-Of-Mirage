using Godot;
using System;

public partial class Run : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    // Reference to Jump state to manage remaining air jumps on walk-off
    private Jump jumpState;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // Run -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = pNode.GetNode<Sprite2D>("Sprite2D");
        jumpState = this.GetParent().GetNode<Jump>("Jump");
        Logger.LogInfo("Query Player Node in [Run] State done...");
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
        try
        {
            Assert.IsNoneNode<Jump>(jumpState);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("jumpState: " + ex.Message);
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
            fsm.ChangeState("Fall");
            return;
        }

        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if ((pNode.Velocity.X == 0) && (direction == 0))
        {
            fsm.ChangeState("Idle");
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
            sprite.FlipH = direction < 0;
            velocity.X = Mathf.MoveToward(velocity.X, direction * pNode.speed, pNode.acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, pNode.acceleration * (float)delta);
        }
        pNode.Velocity = velocity;
        if (animationPlayer.CurrentAnimation != "run")
        {
            animationPlayer.Play("run");
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
            fsm.ChangeState("Jump");
            return;
        }
    }
}
