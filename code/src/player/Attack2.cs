using Godot;
using System;

public partial class Attack2 : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;

    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // Attack2 -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = player.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        Logger.LogInfo("Query Player Node in [Attack_2] State done...");
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
        animationPlayer.Play(AnimationSpecialName.PLAYER_ATTACK_2);
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
        if ((player.Velocity.X == 0) && (direction == 0) && (animationPlayer.CurrentAnimation != AnimationSpecialName.PLAYER_ATTACK_2))
        {
            fsm.ChangeState(StateName.IDLE);
            return;
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        player.MoveAndSlide();
        player.HandleGravity(delta);
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

        /*
        combo attack input handling
        */

        if (Input.IsActionJustPressed("KeyAttack") && player.isComboEnabled)
        {
            Input.ActionRelease("KeyAttack");
            fsm.ChangeState(StateName.ATTACK_3);
            return;
        }

        /*
        Change direction while attacking
        */

        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if (direction != 0)
        {
            graphic.Scale = new Vector2(direction < 0 ? -1 : 1, 1);
        }

        // 长按方向键移动
        if (Input.IsActionPressed("KeyLeft") || Input.IsActionPressed("KeyRight"))
        {
            fsm.ChangeState(StateName.RUN);
            return;
        }
    }
}
