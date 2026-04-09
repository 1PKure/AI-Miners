public class MinerUnloadState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Unloading");

        if (owner.HomeBase != null && owner.CarriedAmount > 0)
        {
            owner.HomeBase.Deposit(
                owner.CarriedResourceType,
                owner.CarriedAmount,
                owner.CarriedScorePerUnit
            );
        }

        owner.ClearCarriedResource();

        if (owner.CurrentResourceNode != null)
        {
            owner.CurrentResourceNode.Release(owner);
            owner.ClearAssignedResourceNode();
        }

        owner.SendEvent(MinerFsmEvents.UnloadComplete);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}