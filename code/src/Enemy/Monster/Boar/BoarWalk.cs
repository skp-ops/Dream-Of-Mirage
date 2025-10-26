using Godot;
using System;

public partial class BoarWalk : State
{
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
        bNode = this.GetParent().GetParent() as Boar; // Walk -> FSM -> Boar
        animationPlayer = bNode.GetNode<AnimationPlayer>(EnemyNodeName.ANIMATION);
        sprite = bNode.GetNode<Sprite2D>(EnemyNodeName.SPRITE2D);
        graphic = bNode.GetNode<Node2D>(EnemyNodeName.GRAPHIC);
        wallCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.WALL_CHECK);
        floorCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.FLOOR_CHECK);
        playerCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.PLAYER_CHECK);
        Logger.LogInfo("Query Boar Node in [BoarWalk] State done...");
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
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate(double delta)
    {
        // if player is detected, switch to attack state
        if (playerCheck.IsColliding())
        {
            fsm.ChangeState(StateName.ATTACK);
            return;
        }
        floorCheck.ForceRaycastUpdate();
        wallCheck.ForceRaycastUpdate();
        // if wall is detected or if no floor ahead, just turn around
        if (wallCheck.IsColliding() || !floorCheck.IsColliding())
        {
            fsm.ChangeState(StateName.IDLE);
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
        bNode.Velocity = new Vector2(bNode.speed * graphic.Scale.X * -1, bNode.Velocity.Y);
        HandleGravity(delta);
        if (animationPlayer.CurrentAnimation != AnimationName.WALK)
        {
            animationPlayer.Play(AnimationName.WALK);
        }
        bNode.MoveAndSlide();
    }
}
