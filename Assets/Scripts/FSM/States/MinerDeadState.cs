using UnityEngine;

public class MinerDeadState : FsmState<MinerAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Dead");

        owner.StopActiveRoutine();
        owner.PathNodeAgent.StopMoving();

        // liberar recurso si estaba ocupado
        if (owner.CurrentResourceNode != null)
        {
            owner.CurrentResourceNode.Release(owner);
            owner.ClearAssignedResourceNode();
        }

        // desactivar visualmente (simple)
        owner.gameObject.SetActive(false);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}