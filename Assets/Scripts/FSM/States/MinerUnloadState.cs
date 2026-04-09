public class MinerUnloadState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Unloading");

        if (owner.HomeBase != null && owner.CarriedGold > 0)
        {
            owner.HomeBase.DepositGold(owner.CarriedGold);
        }

        owner.ClearGold();

        if (owner.CurrentMine != null)
        {
            owner.CurrentMine.Release();
            owner.ClearAssignedMine();
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