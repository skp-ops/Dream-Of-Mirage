using Godot;
using System;
using System.Reflection.Metadata;

public partial class OnWall : State
{
    // player node
    private Player player;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;
    // Track last wall direction used for a wall jump to prevent infinite jumping on the same wall
    private int lastWallDirX = 0;

    public override void _Ready()
    {
        player = this.GetParent().GetParent() as Player; // OnWall -> FSM -> Player
        animationPlayer = player.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = player.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = player.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        Logger.LogInfo("Query Player Node in [OnWall] State done...");
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
        // Only grant a wall jump when switching to a different wall side
        int currentWallDirX = Math.Sign((float)player.GetWallNormal().X);
        if (currentWallDirX != lastWallDirX)
        {
            player.SetJumpCount(1);
        }
    }

    public override void StateExit()
    {
        // Reset the last wall direction when grounded to allow fresh wall jumps later
        if (player.IsOnFloor())
        {
            lastWallDirX = 0;
        }
    }

    public override void StateUpdate(double delta)
    {
        if ((player.IsOnWall() == false) || (player.IsOnFloor()))
        {
            // when player is on floor, state cannot change to Idle
            // since when Idle state cannot reset jump count
            // change to Fall state first, then Fall state will change to Idle
            fsm.ChangeState(StateName.FALL);
            return;
        }
    }

    private void HandleWallSlide(double delta)
    {
        if (player.IsOnWall() && !player.IsOnFloor())
        {
            // Apply wall slide effect
            player.Velocity += new Vector2(0, ConstVar.GRAVITY * 0.4f * (float)delta);
        }
        return;
    }

    public override void StatePhysicsUpdate(double delta)
    {
        Vector2 norm = player.GetWallNormal();
        graphic.Scale = new Vector2(norm.X < 0 ? 1 : -1, 1);

        HandleWallSlide(delta);
        if (animationPlayer.CurrentAnimation != AnimationName.ONWALL)
        {
            animationPlayer.Play(AnimationName.ONWALL);
        }
        player.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("KeyJump") && (player.GetJumpCount() > 0))
        {
            Vector2 wallNormal = player.GetWallNormal();
            int dirX = Math.Sign((float)wallNormal.X);
            // Remember which wall side we jumped from
            lastWallDirX = dirX;
            // Apply horizontal velocity away from the wall
            player.Velocity = new Vector2(player.speed * wallNormal.X, player.jumpVelocity);
            player.SetJumpCount(player.GetJumpCount() - 1);

            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
