public class MinerReturnToBaseState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Returning To Base");

        if (owner.CarriedAmount <= 0)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (owner.HomeBase == null)
            return;

        owner.PathNodeAgent.SetDestination(owner.HomeBase.InteractionPosition);

        if (owner.PathNodeAgent.HasReachedDestination)
        {
            owner.SendEvent(MinerFsmEvents.ReachedBase);
        }
    }

    public override void Update()
    {
        if (owner.HomeBase == null)
            return;

        if (owner.PathNodeAgent.HasReachedDestination)
        {
            owner.SendEvent(MinerFsmEvents.ReachedBase);
        }
    }

    public override void Exit()
    {
        owner.PathNodeAgent.StopMoving();
    }
}