using UnityEngine;

public class MinerIdleState : FsmState<MinerAgentController>
{
    private float retryTimer;
    private const float retryInterval = 0.5f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Idle");
        owner.PathNodeAgent.StopMoving();
        retryTimer = 0f;

        TryAssignMine();
    }

    public override void Update()
    {
        retryTimer -= Time.deltaTime;

        if (retryTimer <= 0f)
        {
            retryTimer = retryInterval;
            TryAssignMine();
        }
    }

    public override void Exit()
    {
    }

    private void TryAssignMine()
    {
        GoldMine mine = owner.FindAvailableMine();

        if (mine != null)
        {
            owner.AssignMine(mine);
            owner.SendEvent(MinerFsmEvents.MineAssigned);
        }
    }
}