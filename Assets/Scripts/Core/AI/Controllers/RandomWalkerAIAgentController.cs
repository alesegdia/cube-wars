using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// An AIAgent that walks randomly in the map
    /// </summary>
    public class RandomWalkerAIAgentController : PlayerController
    {
        private int m_currentUnitIndex = 0;

        public override void TurnStart()
        {
            base.TurnStart();
            m_currentUnitIndex = 0;
        }

        public override void Think()
        {
            if(m_currentUnitIndex == MyUnits.Count)
            {
                NextDecision = CreateEndTurnDecision();
            }
            else
            {
                var unit = MyUnits[m_currentUnitIndex % MyUnits.Count];
                var halfAvailableMoves = unit.CurrentAvailableMoves / 2;

                var nextPos = unit.CellPosition +
                    new Vector2Int(Random.Range(-halfAvailableMoves, halfAvailableMoves + 1),
                                   Random.Range(-halfAvailableMoves, halfAvailableMoves + 1));

                NextDecision = CreateMoveDecision(unit, nextPos);
                m_currentUnitIndex++;
            }
        }

    }

}