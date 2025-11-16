using Godot;
using System;

public partial class Boar : Monster
{
    private FiniteStateMachine fsm;
    private AnimationPlayer animationPlayer;
    private bool isDying = false;

    public override void _Ready()
    {
        MaxHP = 150;
        Speed = 10.0f;

        base._Ready();

        // Get FSM and AnimationPlayer references
        fsm = GetNodeOrNull<FiniteStateMachine>("FSM");
        animationPlayer = GetNodeOrNull<AnimationPlayer>(EnemyNodeName.ANIMATION);

        if (fsm == null)
        {
            Logger.LogError($"FSM not found for {Name}");
        }
        if (animationPlayer == null)
        {
            Logger.LogError($"AnimationPlayer not found at path: {EnemyNodeName.ANIMATION} for {Name}");
        }
    }

    protected override void OnHurt(HitBox hitBox)
    {
        // Don't process hurt if already dying
        if (isDying)
        {
            return;
        }

        TakeDamage(5);
        GD.Print($"Boar was hit! HP: {CurHP}/{MaxHP}");

        // Switch to Hit state when damaged (only if not already in Hit state and still alive)
        if (CurHP > 0 && fsm != null && fsm.GetCurrentStateName() != StateName.HIT)
        {
            fsm.ChangeState(StateName.HIT);
        }
    }

    protected override void Die()
    {
        if (isDying)
        {
            return; // Already dying, prevent duplicate calls
        }
        isDying = true;

        GD.Print($"{Name} is dying! Disconnecting HurtBox...");

        // Stop FSM to prevent state interference
        if (fsm != null)
        {
            fsm.SetProcess(false);
            fsm.SetPhysicsProcess(false);
            GD.Print("FSM disabled");
        }

        // Stop all movement
        Velocity = Vector2.Zero;

        // Disconnect HurtBox to stop taking damage (use deferred to avoid physics query conflicts)
        var hurtBox = FindChildByType<HurtBox>(this);
        if (hurtBox != null)
        {
            hurtBox.CallDeferred("set_monitoring", false);
            hurtBox.CallDeferred("set_monitorable", false);
        }

        // Disable HitBox (use deferred to avoid physics query conflicts)
        var hitBox = FindChildByType<HitBox>(this);
        if (hitBox != null)
        {
            hitBox.CallDeferred("set_monitoring", false);
        }

        // Play death animation before destroying
        if (animationPlayer != null)
        {
            GD.Print($"Playing death animation: {AnimationName.DIE}");

            // Check if die animation exists
            if (animationPlayer.HasAnimation(AnimationName.DIE))
            {
                // Force stop any current animation
                animationPlayer.Stop();
                animationPlayer.Play(AnimationName.DIE);
                GD.Print($"Death animation started, length: {animationPlayer.CurrentAnimationLength}s");

                // Wait for animation to finish before destroying
                animationPlayer.AnimationFinished += OnDieAnimationFinished;
            }
            else
            {
                GD.Print($"Death animation '{AnimationName.DIE}' not found! Available animations:");
                foreach (var anim in animationPlayer.GetAnimationList())
                {
                    GD.Print($"  - {anim}");
                }
                QueueFree();
            }
        }
        else
        {
            GD.Print($"{Name} has no AnimationPlayer, destroying immediately");
            QueueFree();
        }
    }

    private void OnDieAnimationFinished(StringName animName)
    {
        GD.Print($"Animation finished: {animName}");
        if (animName == AnimationName.DIE)
        {
            GD.Print($"{Name} death animation complete, destroying node");
            QueueFree();
        }
    }
}
