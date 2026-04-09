using UnityEngine;

public class MinerMiningState : FsmState<MinerAgentController>
{
    private float miningTimer;
    private const float miningInterval = 0.5f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Mining");
        miningTimer = 0f;

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
        }
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
            owner.SendEvent(MinerFsmEvents.MineLost);
            return;
        }

        miningTimer -= Time.deltaTime;

        if (miningTimer > 0f)
            return;

        miningTimer = miningInterval;

        int extractedGold = owner.CurrentMine.ExtractGold(owner.MiningPerTick);

        if (extractedGold > 0)
        {
            owner.AddGold(extractedGold);

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