public class MinerGoToMineState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Going To Mine");

        if (owner.CurrentResourceNode == null)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (!owner.CurrentResourceNode.IsReservedBy(owner) || !owner.CurrentResourceNode.HasResources)
        {
            owner.CurrentResourceNode.Release(owner);
            owner.ClearAssignedResourceNode();
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        owner.PathNodeAgent.SetDestination(owner.CurrentResourceNode.InteractionPosition);

        if (owner.PathNodeAgent.HasReachedDestination)
        {
            owner.SendEvent(MinerFsmEvents.ReachedMine);
        }
    }

    public override void Update()
    {
        if (owner.CurrentResourceNode == null)
        {
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (!owner.CurrentResourceNode.IsReservedBy(owner) || !owner.CurrentResourceNode.HasResources)
        {
            owner.CurrentResourceNode.Release(owner);
            owner.ClearAssignedResourceNode();
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        if (owner.PathNodeAgent.HasReachedDestination)
        {
            owner.SendEvent(MinerFsmEvents.ReachedMine);
        }
    }

    public override void Exit()
    {
        owner.PathNodeAgent.StopMoving();
    }
}