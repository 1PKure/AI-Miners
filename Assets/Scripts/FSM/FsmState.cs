public abstract class FsmState<T>
{
    protected T owner;
    protected FiniteStateMachine<T> stateMachine;

    public void Initialize(T owner, FiniteStateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}