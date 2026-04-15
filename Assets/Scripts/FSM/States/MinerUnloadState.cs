public class MinerUnloadState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Unloading");

        if (owner.CarriedAmount <= 0)
        {
            owner.SendEvent(MinerFsmEvents.UnloadComplete);
            return;
        }

        owner.StartUnloadRoutine();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        owner.StopActiveRoutine();
    }
}