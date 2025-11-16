using Godot;
using System;

public partial class Attack2 : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private bool isAnimFinishedConnected;

    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // Attack2 -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        Logger.LogInfo("Query Player Node in [Attack_2] State done...");
    }

    public override void StateInit()
    {
        try
        {
            Assert.IsNoneNode<Player>(player);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }
    }

    public override void StateEnter()
    {
        animationPlayer.Play(AnimationSpecialName.PLAYER_ATTACK_2);
        // Subscribe to animation finished to decide post-attack fallback
        if (!isAnimFinishedConnected)
        {
            animationPlayer.AnimationFinished += OnAttack2Finished;
            isAnimFinishedConnected = true;
        }
    }

    public override void StateExit()
    {
        // Unsubscribe to avoid duplicate handlers
        if (isAnimFinishedConnected)
        {
            animationPlayer.AnimationFinished -= OnAttack2Finished;
            isAnimFinishedConnected = false;
        }
    }

    public override void StateUpdate(double delta)
    {
        // Combo chaining while holding attack during movement
        if (player.isComboEnabled && Input.IsActionPressed("KeyAttack"))
        {
            Input.ActionRelease("KeyAttack");
            fsm.ChangeState(StateName.ATTACK_3);
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
        // 地面攻击时停止水平移动，空中攻击保留水平速度
        if (player.IsOnFloor())
        {
            player.Velocity = new Vector2(0, player.Velocity.Y);
        }
        player.EnableTurnDirection();
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

        // combo chaining is handled in StateUpdate to work with held inputs reliably
    }

    private void OnAttack2Finished(StringName animName)
    {
        if (animName.ToString() != AnimationSpecialName.PLAYER_ATTACK_2)
        {
            return;
        }

        if (!player.IsOnFloor())
        {
            fsm.ChangeState(StateName.FALL);
            return;
        }

        float direction = Input.GetAxis("KeyLeft", "KeyRight");
        if (direction != 0)
        {
            fsm.ChangeState(StateName.RUN);
        }
        else
        {
            fsm.ChangeState(StateName.IDLE);
        }
    }
}
