using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Settings for each faction
    /// </summary>
    [CreateAssetMenu(menuName = "CubeWars/Faction settings")]
    public class FactionSettings : ScriptableObject
    {

        [SerializeField] private Color m_color = Color.magenta;

        public Color Color { get { return m_color; } }

    }

}