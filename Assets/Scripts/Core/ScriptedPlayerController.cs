using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// This player controller is intended to be used for testing purposes. It will keep a queue
    /// of decisions that will be applied on each Think
    /// </summary>
    public class ScriptedPlayerController : PlayerController
    {
        private Queue<Decision> decisionQueue { get; set; } = new Queue<Decision>();

        public void EnqueueMoveDecision(Unit unit, Vector2Int targetCell)
        {
            decisionQueue.Enqueue(CreateMoveDecision(unit, targetCell));
        }

        public void EnqueueAttackDecision(Unit source, Unit target)
        {
            decisionQueue.Enqueue(CreateAttackDecision(source, target));
        }

        public void EnqueueEndTurnDecision()
        {
            decisionQueue.Enqueue(CreateEndTurnDecision());
        }

        public override void Think()
        {
            base.Think();
            if(decisionQueue.Count == 0)
            {
                NextDecision = CreateEndTurnDecision();
            }
            else
            {
                NextDecision = decisionQueue.Dequeue();
            }
        }
    }

}