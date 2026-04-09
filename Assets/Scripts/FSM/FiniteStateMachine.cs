using System;
using System.Collections.Generic;

public class FiniteStateMachine<T>
{
    private readonly T owner;
    private readonly Dictionary<Type, FsmState<T>> states = new Dictionary<Type, FsmState<T>>();
    private readonly DoubleEntryTable<Type, string, Type> transitions = new DoubleEntryTable<Type, string, Type>();

    private FsmState<T> currentState;

    public FsmState<T> CurrentState => currentState;

    public FiniteStateMachine(T owner)
    {
        this.owner = owner;
    }

    public void AddState(FsmState<T> state)
    {
        state.Initialize(owner, this);
        states[state.GetType()] = state;
    }

    public void AddTransition<TFrom, TTo>(string trigger)
        where TFrom : FsmState<T>
        where TTo : FsmState<T>
    {
        transitions.Add(typeof(TFrom), trigger, typeof(TTo));
    }

    public void SetInitialState<TState>() where TState : FsmState<T>
    {
        if (!states.TryGetValue(typeof(TState), out currentState))
            return;

        currentState.Enter();
    }

    public void SendEvent(string trigger)
    {
        if (currentState == null)
            return;

        Type currentType = currentState.GetType();

        if (transitions.TryGetValue(currentType, trigger, out Type nextType))
        {
            ChangeState(nextType);
        }
    }

    public void Update()
    {
        currentState?.Update();
    }

    private void ChangeState(Type nextStateType)
    {
        if (!states.TryGetValue(nextStateType, out FsmState<T> nextState))
            return;

        if (nextState == currentState)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }
}