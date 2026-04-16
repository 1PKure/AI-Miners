using UnityEngine;

public class MinerFleeState : FsmState<MinerAgentController>
{
    private float repathTimer;
    private const float repathInterval = 0.35f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Fleeing");
        owner.StopActiveRoutine();

        if (owner.WasDamagedRecently && owner.CurrentResourceNode != null)
        {
            owner.MarkResourceNodeAsDangerous(owner.CurrentResourceNode, 4f);
            owner.AbandonCurrentResourceNode();
        }

        repathTimer = 0f;
        UpdateFleePath();
    }

    public override void Update()
    {
        if (owner.IsDead)
            return;

        if (owner.CurrentThreat == null)
        {
            owner.SendEvent(MinerFsmEvents.SafeReached);
            return;
        }

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            UpdateFleePath();
        }

        float distance = Vector3.Distance(
            owner.transform.position,
            owner.CurrentThreat.transform.position
        );

        bool farEnough = distance >= owner.SafeDistance;
        bool cooldownFinished = !owner.IsFearCooldownActive;

        if (farEnough && cooldownFinished)
        {
            owner.ClearThreat();
            owner.SendEvent(MinerFsmEvents.SafeReached);
        }
    }

    public override void Exit()
    {
        owner.PathNodeAgent.StopMoving();
    }

    private void UpdateFleePath()
    {
        Vector3 fleeTarget = owner.GetFleePosition();
        owner.PathNodeAgent.SetDestination(fleeTarget, true);
    }
}