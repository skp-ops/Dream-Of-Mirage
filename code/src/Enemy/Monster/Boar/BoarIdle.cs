using Godot;
using System;

public partial class BoarIdle : State
{
    private float idleTimer = 0f;
    // monster node
    private Boar bNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private Node2D graphic;
    private RayCast2D wallCheck;
    private RayCast2D floorCheck;
    private RayCast2D playerCheck;

    public override void _Ready()
    {
        bNode = this.GetParent().GetParent() as Boar; // Idle -> FSM -> Boar
        animationPlayer = bNode.GetNode<AnimationPlayer>(EnemyNodeName.ANIMATION);
        sprite = bNode.GetNode<Sprite2D>(EnemyNodeName.SPRITE2D);
        graphic = bNode.GetNode<Node2D>(EnemyNodeName.GRAPHIC);
        wallCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.WALL_CHECK);
        floorCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.FLOOR_CHECK);
        playerCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.PLAYER_CHECK);
        Logger.LogInfo("Query Boar Node in [BoarIdle] State done...");
    }

    public override void StateReady()
    {
        try
        {
            Assert.IsNoneNode<Boar>(bNode);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
            Assert.IsNoneNode<Node2D>(graphic);
            Assert.IsNoneNode<RayCast2D>(wallCheck);
            Assert.IsNoneNode<RayCast2D>(floorCheck);
            Assert.IsNoneNode<RayCast2D>(playerCheck);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }
    }

    public override void StateEnter()
    {
        bNode.Velocity = new Vector2(0, bNode.Velocity.Y);
        // if wall is detected or if no floor ahead, just turn around
        if (wallCheck.IsColliding() || !floorCheck.IsColliding())
        {
            graphic.Scale = new Vector2(-graphic.Scale.X, graphic.Scale.Y);
        }
    }

    public override void StateExit()
    {
        idleTimer = 0f;
    }

    public override void StateUpdate(double delta)
    {
        idleTimer += (float)delta;
        // if player is detected, switch to attack state
        if (playerCheck.IsColliding())
        {
            fsm.ChangeState(StateName.ATTACK);
            return;
        }
        // after 1 seconds, switch to Walk state
        if (idleTimer >= 1.0f)
        {
            fsm.ChangeState(StateName.WALK);
            return;
        }
    }

    private void HandleGravity(double delta)
    {
        if (!bNode.IsOnFloor())
        {
            bNode.Velocity += new Vector2(0, ConstVar.GRAVITY * (float)delta);
        }
    }

    public override void StatePhysicsUpdate(double delta)
    {
        HandleGravity(delta);
        // smoothly play idle animation
        if (animationPlayer.CurrentAnimation != AnimationName.IDLE)
        {
            animationPlayer.Play(AnimationName.IDLE);
        }
        bNode.MoveAndSlide();
    }
}
