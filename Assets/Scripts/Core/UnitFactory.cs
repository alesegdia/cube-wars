using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Creates new units based on the faction and the unit settings index
    /// </summary>
    public class UnitFactory : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private UnitSettingsDatabase m_unitSettingsDatabase = null;
        [SerializeField] private GameObject m_baseUnitPrefab = null;
        [SerializeField] private FactionSettingsDatabase m_factionSettingsDatabase = null;
        #endregion

        /// <summary>
        /// Spawns a new unit
        /// </summary>
        /// <param name="unitDataIndex">The index for the class in the <see cref="m_unitDatas"/> array</param>
        /// <param name="faction">The faction index to assign to this unit</param>
        /// <param name="gridPosition">The position where to spawn this unit</param>
        /// <returns></returns>
        public Unit CreateUnit(int unitDataIndex, int faction, Vector2Int gridPosition)
        {
            var unitData = m_unitSettingsDatabase.GetUnitSettings(unitDataIndex);
            var unitGO = Instantiate(m_baseUnitPrefab);
            var unit = unitGO.GetComponent<Unit>();
            unitGO.transform.position = new Vector3(gridPosition.x + 0.5f, unitData.SizeFactor / 2.0f, gridPosition.y + 0.5f);
            unit.Initialize(unitData, faction, m_factionSettingsDatabase.GetFactionSettings(faction).Color, gridPosition);
            return unit;
        }
    }

}