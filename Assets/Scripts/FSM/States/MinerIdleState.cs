using UnityEngine;

public class MinerIdleState : FsmState<MinerAgentController>
{
    private float retryTimer;
    private const float retryInterval = 0.5f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Idle");
        owner.PathNodeAgent.StopMoving();
        owner.ClearRecentDamageFlag();
        retryTimer = 0f;

        TryAssignResourceNode();
    }

    public override void Update()
    {
        retryTimer -= Time.deltaTime;

        if (retryTimer <= 0f)
        {
            retryTimer = retryInterval;
            TryAssignResourceNode();
        }
    }

    public override void Exit()
    {
    }

    private void TryAssignResourceNode()
    {
        ResourceNode node = owner.FindAvailableResourceNode();

        if (node != null)
        {
            owner.AssignResourceNode(node);
            owner.SendEvent(MinerFsmEvents.MineAssigned);
        }
    }
}