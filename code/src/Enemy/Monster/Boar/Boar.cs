using Godot;
using System;

public partial class Boar : Monster
{
    private FiniteStateMachine fsm;

    public override void _Ready()
    {
        MaxHP = 50;
        Speed = 2.0f;

        base._Ready();

        // Get FSM reference
        fsm = GetNodeOrNull<FiniteStateMachine>("FSM");
        if (fsm == null)
        {
            Logger.LogError($"FSM not found for {Name}");
        }
    }

    protected override void OnHurt(HitBox hitBox)
    {
        TakeDamage(5);
        GD.Print($"Boar was hit! HP: {CurHP}/{MaxHP}");

        if (CurHP <= 0)
        {
            // Switch to Die state when HP reaches 0
            if (fsm != null)
            {
                fsm.ChangeState(StateName.DIE);
            }
        }
        else
        {
            // Switch to Hit state when damaged (only if not already in Hit state)
            if (fsm != null && fsm.GetCurrentStateName() != StateName.HIT)
            {
                fsm.ChangeState(StateName.HIT);
            }
        }
    }
}
