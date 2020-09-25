using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Holds the information for all the <see cref="FactionSettings"/> of all factions
    /// </summary>
    [CreateAssetMenu(menuName = "CubeWars/Unit settings database")]
    public class UnitSettingsDatabase : ScriptableObject
    {
        [SerializeField] private List<UnitSettings> m_unitSettings = new List<UnitSettings>();

        public UnitSettings GetUnitSettings(int unitIndex)
        {
            return m_unitSettings[unitIndex];
        }

    }

}