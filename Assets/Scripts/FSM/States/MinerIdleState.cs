public class MinerIdleState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Idle");
        owner.PathNodeAgent.StopMoving();

        GoldMine mine = owner.FindAvailableMine();

        if (mine != null)
        {
            owner.AssignMine(mine);
            owner.SendEvent(MinerFsmEvents.MineAssigned);
        }
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}