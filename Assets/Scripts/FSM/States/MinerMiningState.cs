using UnityEngine;

public class MinerMiningState : FsmState<MinerAgentController>
{
    private float miningTimer;

    public override void Enter()
    {
        owner.SetCurrentStateName("Mining");
        miningTimer = 0f;

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
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        miningTimer -= Time.deltaTime;

        if (miningTimer > 0f)
            return;

        miningTimer = owner.CurrentResourceNode.MineInterval;

        int extractedAmount = owner.CurrentResourceNode.Extract(owner.MiningPerTick);

        if (extractedAmount > 0)
        {
            owner.AddResource(
                owner.CurrentResourceNode.ResourceType,
                extractedAmount,
                owner.CurrentResourceNode.ScorePerUnit
            );

            if (owner.IsInventoryFull())
            {
                owner.SendEvent(MinerFsmEvents.InventoryFull);
                return;
            }
        }
        else
        {
            owner.SendEvent(MinerFsmEvents.MineEmpty);
        }
    }

    public override void Exit()
    {
    }
}