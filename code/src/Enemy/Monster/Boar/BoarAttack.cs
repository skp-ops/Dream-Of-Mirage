using Godot;
using System;

public partial class BoarAttack : State
{
    private float cooldownTimer;
    private float originalSpeed;
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
        bNode = this.GetParent().GetParent() as Boar; // Attack -> FSM -> Boar
        animationPlayer = bNode.GetNode<AnimationPlayer>(EnemyNodeName.ANIMATION);
        sprite = bNode.GetNode<Sprite2D>(EnemyNodeName.SPRITE2D);
        graphic = bNode.GetNode<Node2D>(EnemyNodeName.GRAPHIC);
        wallCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.WALL_CHECK);
        floorCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.FLOOR_CHECK);
        playerCheck = bNode.GetNode<RayCast2D>(EnemyNodeName.PLAYER_CHECK);
        Logger.LogInfo("Query Boar Node in [BoarAttack] State done...");
    }

    public override void StateInit()
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
        originalSpeed = bNode.Speed;
        bNode.Speed = 30f;
        cooldownTimer = 5f;
    }

    public override void StateExit()
    {
        cooldownTimer = 5f;
        bNode.Speed = originalSpeed;
    }

    public override void StateUpdate(double delta)
    {
        cooldownTimer -= (float)delta;
        if (cooldownTimer <= 0f)
        {
            fsm.ChangeState(StateName.WALK);
            return;
        }
        if (playerCheck.IsColliding())
        {
            var collider = playerCheck.GetCollider();
            // 检测到玩家，重置攻击冷却时间
            if (collider is Player player)
            {
                cooldownTimer = 5f;
                return;
            }
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
        bNode.Velocity = new Vector2(bNode.Speed * graphic.Scale.X * -1, bNode.Velocity.Y);
        HandleGravity(delta);
        // if wall is detected or if no floor ahead, just turn around
        if (wallCheck.IsColliding() || !floorCheck.IsColliding())
        {
            graphic.Scale = new Vector2(-graphic.Scale.X, graphic.Scale.Y);
        }
        bNode.MoveAndSlide();
        if (animationPlayer.CurrentAnimation != AnimationName.ATTACK)
        {
            animationPlayer.Play(AnimationName.ATTACK);
        }
    }

}
