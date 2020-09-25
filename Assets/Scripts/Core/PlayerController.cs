using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Entity responsible for controlling the processes of a player's turn
    /// </summary>
    public abstract class PlayerController : MonoBehaviour
    {
        #region Properties
        protected MatchManager MatchManager { get; set; }

        /// <summary>
        /// The <see cref="Decision"/> to be applied by the <see cref="GameController"/>. Set up at <see cref="Think"/>.
        /// </summary>
        public Decision NextDecision { get; set; }
        
        public FactionManager FactionManager { get; private set; }

        protected MapManager MapManager
        {
            get
            {
                return MatchManager.MapManager;
            }
        }

        protected List<Unit> MyUnits
        {
            get
            {
                return FactionManager.Units;
            }
        }
        #endregion

        public virtual void Initialize(MatchManager matchController, FactionManager factionManager)
        {
            MatchManager = matchController;
            FactionManager = factionManager;
        }

        /// <summary>
        /// Called by <see cref="GameController"/> when turn starts
        /// </summary>
        public virtual void TurnStart()
        {
            FactionManager.RegenerateActionPoints();
        }

        /// <summary>
        /// Helper to create a <see cref="DecisionType.EndTurn"/> <see cref="Decision"/>
        /// </summary>
        /// <returns></returns>
        protected Decision CreateEndTurnDecision()
        {
            return new Decision()
            {
                Type = DecisionType.EndTurn
            };
        }

        /// <summary>
        /// Helper to create a <see cref="DecisionType.Move"/> <see cref="Decision"/>
        /// </summary>
        /// <returns></returns>
        protected Decision CreateMoveDecision(Unit unit, Vector2Int targetCell)
        {
            return new Decision()
            {
                SourceUnit = unit,
                Cell0 = unit.CellPosition,
                Cell1 = targetCell,
                Type = DecisionType.Move
            };
        }

        /// <summary>
        /// Helper to create a <see cref="DecisionType.Attack"/> <see cref="Decision"/>
        /// </summary>
        /// <returns></returns>
        protected Decision CreateAttackDecision(Unit attacker, Unit attacked)
        {
            return new Decision()
            {
                SourceUnit = attacker,
                TargetUnit = attacked,
                Cell0 = attacker.CellPosition,
                Cell1 = attacked.CellPosition,
                Type = DecisionType.Attack
            };
        }

        /// <summary>
        /// Called by <see cref="GameController"/> to let the <see cref="PlayerController"/> decide about the next <see cref="Decision"/>
        /// </summary>
        public virtual void Think()
        {

        }

    }

}