using Godot;
using System;

public partial class BoarHit : State
{
    // monster node
    private Boar bNode;
    // AnimationPlayer and Sprite2D
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private HitBox hitBox;
    private Node2D graphic;
    private bool isAnimFinishedConnected = false;

    // Knockback parameters
    private float knockbackForce = 100f;
    private float knockbackDecay = 5f;
    private float currentKnockbackSpeed = 0f;

    public override void _Ready()
    {
        bNode = this.GetParent().GetParent() as Boar; // Hit -> FSM -> Boar
        animationPlayer = bNode.GetNode<AnimationPlayer>(EnemyNodeName.ANIMATION);
        sprite = bNode.GetNode<Sprite2D>(EnemyNodeName.SPRITE2D);
        hitBox = bNode.GetNode<HitBox>(EnemyNodeName.HITBOX);
        graphic = bNode.GetNode<Node2D>(EnemyNodeName.GRAPHIC);

        Logger.LogInfo("Query Boar Node in [BoarHit] State done...");
    }

    public override void StateInit()
    {
        try
        {
            Assert.IsNoneNode<Boar>(bNode);
            Assert.IsNoneNode<AnimationPlayer>(animationPlayer);
            Assert.IsNoneNode<Sprite2D>(sprite);
            Assert.IsNoneNode<HitBox>(hitBox);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError("Node is null: " + ex.Message);
        }
    }

    public override void StateEnter()
    {
        // Stop movement and apply knockback
        bNode.Velocity = Vector2.Zero;
        currentKnockbackSpeed = knockbackForce;

        // Disable HitBox so Boar can't damage player while being hit
        if (hitBox != null)
        {
            hitBox.Monitoring = false;
        }
        else
        {
            Logger.LogWarning($"HitBox is null when entering Hit state for {bNode.Name}");
        }

        // Play hit animation
        animationPlayer.Play(AnimationName.HIT);

        // Subscribe to animation finished
        if (!isAnimFinishedConnected)
        {
            animationPlayer.AnimationFinished += OnHitAnimationFinished;
            isAnimFinishedConnected = true;
        }
    }

    public override void StateExit()
    {
        // Re-enable HitBox when exiting hit state
        if (hitBox != null)
        {
            hitBox.Monitoring = true;
        }
        else
        {
            Logger.LogWarning($"HitBox is null when exiting Hit state for {bNode.Name}");
        }

        // Unsubscribe from animation finished
        if (isAnimFinishedConnected)
        {
            animationPlayer.AnimationFinished -= OnHitAnimationFinished;
            isAnimFinishedConnected = false;
        }
    }

    public override void StateUpdate(double delta)
    {
        // Nothing to do during hit animation
    }

    private void OnHitAnimationFinished(StringName animName)
    {
        if (animName.ToString() != AnimationName.HIT)
        {
            return;
        }

        // After hit animation, immediately switch to attack state
        fsm.ChangeState(StateName.ATTACK);
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
        // Apply knockback (in opposite direction of facing)
        if (currentKnockbackSpeed > 0)
        {
            // Knockback direction is opposite to facing direction
            float knockbackDirection = graphic.Scale.X; // If facing left (Scale.X = 1), knockback right; if facing right (Scale.X = -1), knockback left
            bNode.Velocity = new Vector2(knockbackDirection * currentKnockbackSpeed, bNode.Velocity.Y);
            currentKnockbackSpeed -= knockbackDecay * (float)delta * 100f; // Decay knockback speed
        }
        else
        {
            // Stop horizontal movement after knockback ends
            bNode.Velocity = new Vector2(0, bNode.Velocity.Y);
        }

        HandleGravity(delta);
        bNode.MoveAndSlide();
    }
}
