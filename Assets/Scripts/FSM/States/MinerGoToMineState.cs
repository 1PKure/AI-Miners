public class MinerGoToMineState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Going To Mine");

        if (owner.CurrentMine == null)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (!owner.CurrentMine.IsReservedBy(owner) || !owner.CurrentMine.HasGold)
        {
            owner.CurrentMine.Release(owner);
            owner.ClearAssignedMine();
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        owner.PathNodeAgent.SetDestination(owner.CurrentMine.InteractionPosition);
    }

    public override void Update()
    {
        if (owner.CurrentMine == null)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (!owner.CurrentMine.IsReservedBy(owner) || !owner.CurrentMine.HasGold)
        {
            owner.CurrentMine.Release(owner);
            owner.ClearAssignedMine();
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (owner.IsNearTarget(owner.CurrentMine.InteractionPosition))
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.ReachedMine);
        }
    }

    public override void Exit()
    {
    }
}