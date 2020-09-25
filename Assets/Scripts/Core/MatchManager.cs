using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Entity in charge of building the units of each faction and the map,
    /// as well as being a bridge for several processes/queries and an accessor for
    /// the <see cref="MapManager"/> and the different <see cref="FactionManager"/> in
    /// the game
    /// </summary>
    public class MatchManager : MonoBehaviour
    {

        #region Serialized Fields
        [SerializeField] private UnitFactory m_unitFactory = null;
        [SerializeField] private MapManager m_mapManager = null;
        #endregion

        #region Properties

        public MapManager MapManager { get { return m_mapManager; } }

        public List<FactionManager> Factions { get; private set; }
        
        public FactionManager HumanFaction { get; private set; }
        
        public List<FactionManager> BotFactions { get; private set; }
        
        public bool HumanWon
        {
            get
            {
                return !HumanFaction.IsDead;
            }
        }
        public bool BotMatch
        {
            get
            {
                return BotFactions.Count == Factions.Count;
            }
        }
        #endregion

        /// <summary>
        /// Boots the MatchManager given a specific <see cref="MatchSettings"/>
        /// </summary>
        /// <param name="matchConfig"></param>
        public void CreateMatch(MatchSettings matchConfig)
        {
            MapManager.Initialize(matchConfig.MapSize.x, matchConfig.MapSize.y);

            if(Factions != null)
            {
                foreach (var faction in Factions)
                {
                    faction.Clean();
                }
            }

            Factions = new List<FactionManager>();
            BotFactions = new List<FactionManager>();

            int factionIndex = 0;
            foreach(var playerConfig in matchConfig.PlayerConfigs)
            {
                var factionManager = new FactionManager(factionIndex, playerConfig.PlayerType);
                if(playerConfig.PlayerType == PlayerType.Human)
                {
                    HumanFaction = factionManager;
                }
                else
                {
                    BotFactions.Add(factionManager);
                }
                foreach(var unitConfig in playerConfig.Units)
                {
                    var unit = m_unitFactory.CreateUnit(unitConfig.UnitType, factionIndex, unitConfig.SpawnPosition);
                    MapManager.PlaceUnit(unitConfig.SpawnPosition, unit);
                    factionManager.AddUnit(unit);
                    unit.transform.parent = MapManager.transform;
                }
                Factions.Add(factionManager);
                factionIndex++;
            }
        }

        /// <summary>
        /// Sets the overlay and turns invisible the unreachable-for-attack units
        /// </summary>
        /// <param name="unit"></param>
        public void ActivateUIForUnit(Unit unit)
        {
            MapManager.SetOverlayForUnitMoveReach(unit);
            foreach(var otherUnit in EnemyUnits(unit))
            {
                if (!unit.CanAttack(otherUnit))
                {
                    otherUnit.SetInvisible();
                }
            }
        }

        /// <summary>
        /// Removes overlay and turn all units visible again
        /// </summary>
        /// <param name="unit"></param>
        public void DeactivateUIForUnit(Unit unit)
        {
            MapManager.ClearOverlay();
            foreach(var otherUnit in EnemyUnits(unit))
            {
                otherUnit.SetVisible();
            }
        }

        /// <summary>
        /// Iterator to get all enemy units
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public IEnumerable<Unit> EnemyUnits(Unit unit)
        {
            foreach (var faction in Factions)
            {
                if (faction.FactionIndex != unit.Faction)
                {
                    foreach (var otherUnit in faction.Units)
                    {
                        yield return otherUnit;
                    }
                }
            }
        }

        /// <summary>
        /// Iterator to get all units
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public IEnumerable<Unit> AllUnits()
        {
            foreach (var faction in Factions)
            {
                foreach (var unit in faction.Units)
                {
                    yield return unit;
                }
            }
        }

        /// <summary>
        /// Checks if the player lost or won
        /// </summary>
        /// <returns></returns>
        public bool GameFinished()
        {
            var botsDead = false;
            var aliveBots = 0;
            foreach (var faction in BotFactions)
            {
                if(!faction.IsDead) aliveBots++;
                botsDead = botsDead && faction.IsDead;
            }
            if(!BotMatch)
            {
                return
                     botsDead && !HumanFaction.IsDead ||
                    !botsDead && HumanFaction.IsDead;
            }
            else
            {
                return aliveBots == 1;
            }
        }

        /// <summary>
        /// Called when an unit has died
        /// </summary>
        /// <param name="unit"></param>
        public void NotifyDeath(Unit unit)
        {
            Factions[unit.Faction].NotifyDeath(unit);
            m_mapManager.RemoveUnit(unit.CellPosition);
        }

        /// <summary>
        /// Forces the human lose for debugging purposes
        /// </summary>
        public void ForceHumanLose()
        {
            foreach (var faction in Factions)
            {
                if (faction.IsHuman)
                {
                    eradicateFaction(faction);
                }
            }
        }

        /// <summary>
        /// Forces the human win for debugging purposes
        /// </summary>
        public void ForceHumanWin()
        {
            foreach (var faction in Factions)
            {
                if (!faction.IsHuman)
                {
                    eradicateFaction(faction);
                }
            }
        }

        private void eradicateFaction(FactionManager faction)
        {
            foreach(var unit in faction.Units)
            {
                Destroy(unit.gameObject);
                m_mapManager.RemoveUnit(unit.CellPosition);
            }
            faction.Units.Clear();
        }
    }

} 