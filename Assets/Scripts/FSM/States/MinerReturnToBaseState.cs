public class MinerReturnToBaseState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Returning To Base");

        if (owner.HomeBase != null)
        {
            owner.PathNodeAgent.SetDestination(owner.HomeBase.transform.position);
        }
    }

    public override void Update()
    {
        if (owner.HomeBase == null)
            return;

        if (owner.IsNearTarget(owner.HomeBase.transform.position))
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.ReachedBase);
        }
    }

    public override void Exit()
    {
    }
}