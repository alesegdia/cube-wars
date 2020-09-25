using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Holds the information for all the <see cref="FactionSettings"/> of all factions
    /// </summary>
    [CreateAssetMenu(menuName = "CubeWars/Faction settings database")]
    public class FactionSettingsDatabase : ScriptableObject
    {
        [SerializeField] private List<FactionSettings> m_factionSettings = new List<FactionSettings>();

        public FactionSettings GetFactionSettings(int faction)
        {
            return m_factionSettings[faction];
        }

    }

}