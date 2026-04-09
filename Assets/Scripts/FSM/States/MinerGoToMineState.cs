public class MinerGoToMineState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Going To Mine");

        if (owner.CurrentMine != null)
        {
            owner.PathNodeAgent.SetDestination(owner.CurrentMine.transform.position);
        }
    }

    public override void Update()
    {
        if (owner.CurrentMine == null)
            return;

        if (owner.IsNearTarget(owner.CurrentMine.transform.position))
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.ReachedMine);
        }
    }

    public override void Exit()
    {
    }
}