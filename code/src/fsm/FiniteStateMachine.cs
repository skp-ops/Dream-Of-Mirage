using Godot;
using System;
using System.Collections.Generic;
using static Logger;

public partial class FiniteStateMachine : Node
{
    [Export]
    public NodePath mInitialStatePath;
    private State mCurrentState;

    private Dictionary<string, State> mStates = new Dictionary<string, State>();

    public override void _Ready()
    {
        // Init all states
        foreach (Node child in GetChildren())
        {
            LogInfo($"Found child node: {child.Name} of type {child.GetType().Name}");
            if (child is State s)
            {
                mStates[s.Name] = s;
                s.fsm = this;
                s.StateInit();
                s.StateExit(); // Ensure all states are exited initially
                LogInfo($"State {s.Name} initialized.");
            }
        }
        // Do current state
        mCurrentState = GetNode<State>(mInitialStatePath);
        if (mCurrentState != null)
        {
            LogInfo($"Initial state set to {mCurrentState.Name}.");
            mCurrentState.StateEnter();
        }
        else
        {
            LogError("Initial state is null. Please check the mInitialStatePath.");
        }
    }

    public override void _Process(double delta)
    {
        try
        {
            Assert.IsNoneNode<State>(mCurrentState);
        }
        catch (Exception ex)
        {
            LogError($"Error in StateUpdate of {mCurrentState?.Name}: {ex.Message}");
        }
        mCurrentState?.StateUpdate(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        try
        {
            Assert.IsNoneNode<State>(mCurrentState);
        }
        catch (Exception ex)
        {
            LogError($"Error in StatePhysicsUpdate of {mCurrentState?.Name}: {ex.Message}");
        }
        mCurrentState?.StatePhysicsUpdate(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        try
        {
            Assert.IsNoneNode<State>(mCurrentState);
        }
        catch (Exception ex)
        {
            LogError($"Error in StateHandleInput of {mCurrentState?.Name}: {ex.Message}");
        }
        mCurrentState?.StateHandleInput(@event);
    }

    public void ChangeState(string key)
    {
        // LogInfo(mCurrentState.Name + " changing to " + key);
        if (!mStates.ContainsKey(key))
        {
            LogError($"State change to {key} failed. State does not exist.");
            return;
        }

        if (mCurrentState == mStates[key])
        {
            LogWarning($"State change to {key} failed. State is already active.");
            return;
        }
        try
        {
            Assert.IsNoneNode<State>(mCurrentState);
        }
        catch (Exception ex)
        {
            LogError($"Error during state exit of {mCurrentState?.Name}: {ex.Message}");
        }
        mCurrentState?.StateExit();
        mCurrentState = mStates[key];
        try
        {
            Assert.IsNoneNode<State>(mCurrentState);
        }
        catch (Exception ex)
        {
            LogError($"Error during state enter of {mCurrentState?.Name}: {ex.Message}");
        }
        mCurrentState?.StateEnter();
    }
}