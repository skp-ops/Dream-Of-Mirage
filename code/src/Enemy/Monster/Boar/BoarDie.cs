using Godot;
using System;

public partial class BoarDie : State
{
    private Boar bNode;
    private AnimationPlayer animationPlayer;
    private HitBox hitBox;
    private HurtBox hurtBox;
    private bool isAnimFinishedConnected = false;

    public override void _Ready()
    {
        bNode = GetParent().GetParent() as Boar; // Die -> FSM -> Boar
        animationPlayer = bNode.GetNodeOrNull<AnimationPlayer>(EnemyNodeName.ANIMATION);
        hitBox = bNode.GetNodeOrNull<HitBox>(EnemyNodeName.HITBOX);
        hurtBox = bNode.GetNodeOrNull<HurtBox>(EnemyNodeName.HURTBOX);

        if (animationPlayer == null)
        {
            Logger.LogError($"AnimationPlayer not found for {bNode.Name}");
        }

        Logger.LogInfo("BoarDie state initialized");
    }

    public override void StateInit()
    {
        try
        {
            Assert.IsNoneNode<Boar>(bNode);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Node is null: {ex.Message}");
        }
    }

    public override void StateEnter()
    {
        GD.Print($"{bNode.Name} entered Die state");

        // Stop all movement
        bNode.Velocity = Vector2.Zero;

        // Disable HurtBox to stop taking damage (use deferred)
        if (hurtBox != null)
        {
            hurtBox.CallDeferred("set_monitoring", false);
            hurtBox.CallDeferred("set_monitorable", false);
        }

        // Disable HitBox to stop attacking (use deferred)
        if (hitBox != null)
        {
            hitBox.CallDeferred("set_monitoring", false);
        }

        // Play death animation
        if (animationPlayer != null && animationPlayer.HasAnimation(AnimationName.DIE))
        {
            animationPlayer.Play(AnimationName.DIE);
            GD.Print($"Playing death animation, length: {animationPlayer.CurrentAnimationLength}s");

            if (!isAnimFinishedConnected)
            {
                animationPlayer.AnimationFinished += OnDieAnimationFinished;
                isAnimFinishedConnected = true;
            }
        }
        else
        {
            Logger.LogWarning($"Death animation not found, destroying immediately");
            bNode.QueueFree();
        }
    }

    public override void StateExit()
    {
        // Unsubscribe from animation finished
        if (isAnimFinishedConnected && animationPlayer != null)
        {
            animationPlayer.AnimationFinished -= OnDieAnimationFinished;
            isAnimFinishedConnected = false;
        }
    }

    public override void StateUpdate(double delta)
    {
        // Nothing to update during death
    }

    public override void StatePhysicsUpdate(double delta)
    {
        // Keep gravity and ground collision during death animation
        if (!bNode.IsOnFloor())
        {
            bNode.Velocity += new Vector2(0, ConstVar.GRAVITY * (float)delta);
        }
        bNode.MoveAndSlide();
    }

    private void OnDieAnimationFinished(StringName animName)
    {
        if (animName == AnimationName.DIE)
        {
            GD.Print($"{bNode.Name} death animation complete, destroying node");
            bNode.QueueFree();
        }
    }
}
