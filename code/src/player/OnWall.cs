using Godot;
using System;
using System.Reflection.Metadata;

public partial class OnWall : State
{
    // player node
    private Player pNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;
    private Jump jumpState;
    // Track last wall direction used for a wall jump to prevent infinite jumping on the same wall
    private int lastWallDirX = 0;

    public override void _Ready()
    {
        pNode = this.GetParent().GetParent() as Player; // OnWall -> FSM -> Player
        animationPlayer = pNode.GetNode<AnimationPlayer>(PlayerNodeName.ANIMATION);
        sprite = pNode.GetNode<Sprite2D>(PlayerNodeName.SPRITE2D);
        graphic = pNode.GetNode<Node2D>(PlayerNodeName.GRAPHIC);
        jumpState = this.GetParent().GetNode<Jump>(StateName.JUMP);
        Logger.LogInfo("Query Player Node in [OnWall] State done...");
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
        // Only grant a wall jump when switching to a different wall side
        int currentWallDirX = Math.Sign((float)pNode.GetWallNormal().X);
        if (currentWallDirX != lastWallDirX)
        {
            jumpState.jumpCount = 1;
        }
    }

    public override void StateExit()
    {
        // Reset the last wall direction when grounded to allow fresh wall jumps later
        if (pNode.IsOnFloor())
        {
            lastWallDirX = 0;
        }
    }

    public override void StateUpdate(double delta)
    {
        if ((pNode.IsOnWall() == false) || (pNode.IsOnFloor()))
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
        if (pNode.IsOnWall() && !pNode.IsOnFloor())
        {
            // Apply wall slide effect
            pNode.Velocity += new Vector2(0, ConstVar.GRAVITY * 0.4f * (float)delta);
        }
        return;
    }

    public override void StatePhysicsUpdate(double delta)
    {
        Vector2 norm = pNode.GetWallNormal();
        graphic.Scale = new Vector2(norm.X < 0 ? 1 : -1, 1);

        HandleWallSlide(delta);
        if (animationPlayer.CurrentAnimation != AnimationName.ONWALL)
        {
            animationPlayer.Play(AnimationName.ONWALL);
        }
        pNode.MoveAndSlide();
    }

    public override void StateHandleInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("KeyJump") && (jumpState.jumpCount > 0))
        {
            Vector2 wallNormal = pNode.GetWallNormal();
            int dirX = Math.Sign((float)wallNormal.X);
            // Remember which wall side we jumped from
            lastWallDirX = dirX;
            // Apply horizontal velocity away from the wall
            pNode.Velocity = new Vector2(pNode.speed * wallNormal.X, pNode.jumpVelocity);
            jumpState.jumpCount--;

            Input.ActionRelease("KeyJump");
            fsm.ChangeState(StateName.JUMP);
            return;
        }
    }
}
