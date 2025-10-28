using Godot;
using System;

public partial class Run : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;


    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // Run -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = player.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        Logger.LogInfo("Query Player Node in [Run] State done...");
    }

    public override void StateInit()
    {
        try
        {
            Assert.IsNoneNode<Player>(player);
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
        player.SetJumpCount(2);
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate(double delta)
    {
        if (player.IsOnFloor() == false)
        {
            fsm.ChangeState(StateName.FALL);
            return;
        }

        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if ((player.Velocity.X == 0) && (direction == 0))
        {
            fsm.ChangeState(StateName.IDLE);
            return;
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        Vector2 velocity = player.Velocity;
        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        // horizontal movement
        if (direction != 0)
        {
            graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
            velocity.X = Mathf.MoveToward(velocity.X, direction * player.speed, player.acceleration * (float)delta);
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, player.acceleration * (float)delta);
        }
        player.Velocity = velocity;
        if (animationPlayer.CurrentAnimation != AnimationName.RUN)
        {
            animationPlayer.Play(AnimationName.RUN);
        }
        player.MoveAndSlide();
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
            player.Velocity = new Vector2(player.Velocity.X, player.jumpVelocity);
            // Prevent the same-frame buffered press from triggering another jump in Jump state
            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
        if (Input.IsActionPressed("KeyAttack"))
        {
            Input.ActionRelease("KeyAttack");
            fsm.ChangeState(StateName.ATTACK_1);
            return;
        }
    }
}
