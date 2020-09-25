using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Manages all the units of this faction and respond to basic queries about the state of the Faction
    /// </summary>
    public class FactionManager
    {
        public FactionManager(int factionIndex, PlayerType playerType)
        {
            FactionIndex = factionIndex;
            PlayerType = playerType;
        }

        #region Properties
        /// <summary>
        /// Whether the <see cref="FactionManager"/> is the human or not
        /// </summary>
        public bool IsHuman
        {
            get
            {
                return PlayerType == PlayerType.Human;
            }
        }

        /// <summary>
        /// Checks if there are no alive units in this <see cref="FactionManager"/>
        /// </summary>
        public bool IsDead
        {
            get
            {
                return Units.Count == 0;
            }
        }

        /// <summary>
        /// The index of this faction
        /// </summary>
        public int FactionIndex { get; private set; } = -1;

        /// <summary>
        /// Is this a Bot or a Human?
        /// </summary>
        public PlayerType PlayerType { get; private set; }

        /// <summary>
        /// List of <see cref="Unit"/> of this faction
        /// </summary>
        public List<Unit> Units { get; } = new List<Unit>();
        #endregion

        public void AddUnit(Unit unit)
        {
            Units.Add(unit);
        }

        /// <summary>
        /// Destroy all units
        /// </summary>
        public void Clean()
        {
            foreach(var unit in Units)
            {
                Object.Destroy(unit.gameObject);
            }
        }

        /// <summary>
        /// Regenerates the AP for all units. Called at the beginning of the turn.
        /// </summary>
        public void RegenerateActionPoints()
        {
            foreach(var unit in Units)
            {
                unit.RegenerateAP();
            }
        }

        /// <summary>
        /// Gets notified about the death of an <see cref="Unit"/>
        /// </summary>
        /// <param name="unit"></param>
        public void NotifyDeath(Unit unit)
        {
            Debug.Assert(Units.Contains(unit));
            Units.Remove(unit);
        }

    }

}