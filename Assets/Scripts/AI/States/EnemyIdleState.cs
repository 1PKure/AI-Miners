public class EnemyIdleState : FsmState<EnemyAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Idle");
        owner.PathNodeAgent.StopMoving();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}