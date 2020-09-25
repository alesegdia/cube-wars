using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CubeWars
{

    /// <summary>
    /// This AIAgent tries to move and attack with one unit if possible, and moves randomly maximizing the distance if not.
    /// </summary>
    public class AgressiveAIAgentController : BaseAIAgentController
    {
        private Queue<Decision> decisionQueue { get; set; } = new Queue<Decision>();
        private List<Unit> nonAttackingUnits { get; set; } = new List<Unit>();
        private int nonAttackingUnitTurn { get; set; } = 0;
        private bool attackThisTurn { get; set; } = false;

        public override void Initialize(MatchManager matchController, FactionManager factionManager)
        {
            base.Initialize(matchController, factionManager);
            FactionManager.Units.OrderBy(u => u.AttackPower).Reverse();
        }

        public override void TurnStart()
        {
            base.TurnStart();
            nonAttackingUnits.Clear();
            nonAttackingUnitTurn = 0;
            attackThisTurn = false;
            foreach (var unit in FactionManager.Units)
            {
                var pf = MatchManager.MapManager.GetPathFinderFor(unit);
                var action = canMoveAndAttack(unit, pf);
                if (action != null && !attackThisTurn)
                {
                    decisionQueue.Enqueue(CreateMoveDecision(unit, action.MoveTo));
                    decisionQueue.Enqueue(CreateAttackDecision(unit, action.AttackTo));
                    attackThisTurn = true;
                }
                else
                {
                    nonAttackingUnits.Add(unit);
                }
            }
        }

        public override void Think()
        {
            // wait for attack to be completed so we build the pathfinder with moved unit
            if(attackThisTurn && decisionQueue.Count > 0)
            {
                NextDecision = decisionQueue.Dequeue();
                return;
            }
            attackThisTurn = false;

            // then apply movements for the rest of the units
            if(nonAttackingUnitTurn < nonAttackingUnits.Count)
            {
                var unit = nonAttackingUnits[nonAttackingUnitTurn];
                var pf = MatchManager.MapManager.GetPathFinderFor(unit);
                decisionQueue.Enqueue(CreateRandomWanderMove(unit, pf));
            }
            else
            {
                decisionQueue.Enqueue(CreateEndTurnDecision());
            }
            NextDecision = decisionQueue.Dequeue();
            nonAttackingUnitTurn++;
        }

    }

}