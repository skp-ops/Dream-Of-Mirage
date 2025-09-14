using Godot;
using System;

public partial class State : Node
{
    public FiniteStateMachine fsm;

    // Called when the state is entered or exited
    public virtual void StateEnter() { }
    // Remove signal connections and other cleanup
    public virtual void StateExit() { }

    // Bind functions to FSM signals or other behaviors
    // Called during the node's lifecycle
    public virtual void StateReady() { }

    // _Process, _PhysicsProcess, _Input
    public virtual void StateUpdate(double delta) { }
    public virtual void StatePhysicsUpdate(double delta) { }
    public virtual void StateHandleInput(InputEvent @event) { }
}