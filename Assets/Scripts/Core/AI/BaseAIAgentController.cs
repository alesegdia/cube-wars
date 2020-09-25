using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace CubeWars
{

    /// <summary>
    /// Base class for all AIAgents with common functionality
    /// </summary>
    public class BaseAIAgentController : PlayerController
    {

        /// <summary>
        /// Used to represent the action of an unit moving first and attacking next
        /// </summary>
        protected class MoveAndAttackAction
        {
            public Vector2Int MoveTo;
            public Unit AttackTo;
        }

        /// <summary>
        /// Checks for a random enemy in range if the unit was in another position
        /// </summary>
        /// <param name="unit">The unit from where to look from</param>
        /// <param name="fromX">The X coordinate of the supposed position</param>
        /// <param name="fromY">The Y coordinate of the supposed position</param>
        /// <returns>A random unit in range or null if there are no units in range</returns>
        protected Unit enemyInRange(Unit unit, int fromX, int fromY)
        {
            foreach (var enemyUnit in MatchManager.EnemyUnits(unit))
            {
                if (Utils.MahnattanDistance(fromX, fromY, enemyUnit.CellPosition.x, enemyUnit.CellPosition.y) <= unit.AttackRange)
                {
                    return enemyUnit;
                }
            }
            return null;
        }

        protected Unit enemyInRange(Unit unit, Vector2Int from)
        {
            return enemyInRange(unit, from.x, from.y);
        }

        /// <summary>
        /// Tries to build a <see cref="MoveAndAttackAction"/> if possible
        /// </summary>
        /// <param name="unit">The unit that is trying to move and attack</param>
        /// <param name="pf">The <see cref="PathFinder"/> for the unit</param>
        /// <returns>A <see cref="MoveAndAttackAction"/> if possible, or null if not</returns>
        protected MoveAndAttackAction canMoveAndAttack(Unit unit, PathFinder pf)
        {
            int availableMovesWithAttack = unit.AvailableMovesWithAttack;
            List<MoveAndAttackAction> actions = new List<MoveAndAttackAction>();

            foreach(var node in pf.ReachableNodes())
            {
                if (node.Cost <= availableMovesWithAttack)
                {
                    var enemy = enemyInRange(unit, node.Position);
                    if (enemy != null)
                    {
                        actions.Add(new MoveAndAttackAction()
                        {
                            MoveTo = node.Position,
                            AttackTo = enemy
                        });
                    }
                }
            }

            if (actions.Count > 0)
            {
                actions.OrderBy(a => a.AttackTo.CurrentHP);
                return actions[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a decision that wanders randomly in the map, maximizing the distance
        /// with a random component
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> to perform the action</param>
        /// <param name="pf">The <see cref="PathFinder"/> of the unit</param>
        /// <returns></returns>
        protected Decision CreateRandomWanderMoveMaximizingDistance(Unit unit, PathFinder pf)
        {
            var furtherCellCost = 0;
            var furtherCell = new Vector2Int(0, 0);
            foreach (var cell in pf.ReachableNodes())
            {
                if (unit.CurrentAvailableMoves <= cell.Cost)
                {
                    if (cell.Cost > furtherCellCost)
                    {
                        furtherCellCost = cell.Cost;
                        furtherCell = cell.Position;
                    }
                }
            }
            return new Decision()
            {
                Cell0 = unit.CellPosition,
                Cell1 = furtherCell,
                SourceUnit = unit
            };
        }

        protected Decision CreateRandomWanderMove(Unit unit, PathFinder pf)
        {
            var availableCells = new List<PathFinder.Cell>();
            foreach (var cell in pf.ReachableNodes())
            {
                availableCells.Add(cell);
            }
            return new Decision()
            {
                Cell0 = unit.CellPosition,
                Cell1 = availableCells[Random.Range(0, availableCells.Count)].Position,
                SourceUnit = unit
            };
        }



    }

}