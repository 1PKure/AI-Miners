using System.Collections.Generic;
using UnityEngine;

public class MinerFleeState : FsmState<MinerAgentController>
{
    private float repathTimer;
    private const float repathInterval = 0.25f;

    private PathNode lastChosenNode;

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
        lastChosenNode = null;

        ChooseNextFleeNode();
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

        float distanceToThreat = Vector3.Distance(
            owner.transform.position,
            owner.CurrentThreat.transform.position
        );

        bool farEnough = distanceToThreat >= owner.SafeDistance;
        bool cooldownFinished = !owner.IsFearCooldownActive;

        if (farEnough && cooldownFinished)
        {
            owner.ClearThreat();
            owner.SendEvent(MinerFsmEvents.SafeReached);
            return;
        }

        repathTimer -= Time.deltaTime;

        bool needsNewDecision =
            repathTimer <= 0f ||
            !owner.PathNodeAgent.IsMoving ||
            !owner.PathNodeAgent.HasValidPath ||
            owner.PathNodeAgent.HasReachedDestination;

        if (needsNewDecision)
        {
            repathTimer = repathInterval;
            ChooseNextFleeNode();
        }
    }

    public override void Exit()
    {
        owner.PathNodeAgent.StopMoving();
    }

    private void ChooseNextFleeNode()
    {
        if (owner.CurrentThreat == null)
            return;

        PathNodeGenerator generator = owner.PathNodeAgent.NodeGenerator;

        if (generator == null)
        {
            owner.PathNodeAgent.StopMoving();
            return;
        }

        PathNode currentNode = generator.GetClosestNode(owner.transform.position);

        if (currentNode == null)
        {
            owner.PathNodeAgent.StopMoving();
            return;
        }

        PathNode bestNode = FindBestEscapeNode(currentNode);

        if (bestNode == null)
        {
            owner.PathNodeAgent.StopMoving();
            return;
        }

        lastChosenNode = bestNode;
        owner.PathNodeAgent.SetDestination(bestNode.WorldPosition, true);
    }

    private PathNode FindBestEscapeNode(PathNode currentNode)
    {
        if (owner.CurrentThreat == null)
            return null;

        Vector3 threatPos = owner.CurrentThreat.transform.position;
        float currentThreatDistance = Vector3.Distance(currentNode.WorldPosition, threatPos);

        PathNode bestNode = null;
        float bestScore = float.MinValue;

        List<PathNode> candidates = new List<PathNode>();

        if (currentNode.Neighbors != null)
        {
            for (int i = 0; i < currentNode.Neighbors.Count; i++)
            {
                PathNode neighbor = currentNode.Neighbors[i];

                if (neighbor == null)
                    continue;

                candidates.Add(neighbor);

                if (neighbor.Neighbors != null)
                {
                    for (int j = 0; j < neighbor.Neighbors.Count; j++)
                    {
                        PathNode secondRing = neighbor.Neighbors[j];

                        if (secondRing == null || secondRing == currentNode)
                            continue;

                        if (!candidates.Contains(secondRing))
                            candidates.Add(secondRing);
                    }
                }
            }
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            PathNode candidate = candidates[i];

            if (candidate == null)
                continue;

            float distanceToThreat = Vector3.Distance(candidate.WorldPosition, threatPos);
            int neighborCount = GetValidNeighborCount(candidate);
            if (neighborCount <= 1)
                continue;

            float score = 0f;

            score += distanceToThreat * 10f;

            if (distanceToThreat <= currentThreatDistance)
                score -= 10f;

            if (candidate == lastChosenNode)
                score -= 25f;

            score += neighborCount * 2f;

            score -= candidate.NodeCost * 1.5f;

            if (score > bestScore)
            {
                bestScore = score;
                bestNode = candidate;
            }
        }

        return bestNode;
    }

    private int GetValidNeighborCount(PathNode node)
    {
        if (node == null || node.Neighbors == null)
            return 0;

        int count = 0;

        for (int i = 0; i < node.Neighbors.Count; i++)
        {
            if (node.Neighbors[i] != null)
                count++;
        }

        return count;
    }
}