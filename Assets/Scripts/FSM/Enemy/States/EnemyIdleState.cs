public class EnemyIdleState : FsmState<EnemyAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Idle");
        owner.PathNodeAgent.StopMoving();
    }

    public override void Update()
    {
        MinerAgentController miner = owner.FindClosestMiner();

        if (miner != null)
        {
            owner.AssignTarget(miner);
            miner.AssignThreat(owner);
            owner.SendEvent(EnemyFsmEvents.MinerDetected);
        }
    }

    public override void Exit()
    {
    }
}