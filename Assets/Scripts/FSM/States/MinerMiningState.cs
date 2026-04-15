public class MinerMiningState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Mining");

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

        owner.StartMiningRoutine();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        owner.StopActiveRoutine();
    }
}