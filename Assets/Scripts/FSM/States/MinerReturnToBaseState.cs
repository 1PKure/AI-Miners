public class MinerReturnToBaseState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Returning To Base");

        if (owner.CarriedGold <= 0)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (owner.HomeBase != null)
        {
            owner.PathNodeAgent.SetDestination(owner.HomeBase.InteractionPosition);
        }
    }

    public override void Update()
    {
        if (owner.HomeBase == null)
            return;

        if (owner.IsNearTarget(owner.HomeBase.InteractionPosition))
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.ReachedBase);
        }
    }

    public override void Exit()
    {
    }
}