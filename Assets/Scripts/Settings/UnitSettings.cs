using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{
    
    /// <summary>
    /// Holds information about a <see cref="Unit"/> class
    /// </summary>
    [CreateAssetMenu(menuName = "CubeWars/Unit settings")]
    public class UnitSettings : ScriptableObject
    {

        #region Serialized Fields
        [SerializeField] private string m_className = "<class-name>";
        
        [SerializeField] private int m_maxHealth = 0;
        [SerializeField] private int m_actionPoints = 0;

        [Space()]
        [SerializeField] private int m_moveCost = 0;

        [Space()]
        [SerializeField] private int m_attackPower = 0;
        [SerializeField] private int m_attackRange = 0;
        [SerializeField] private int m_attackAPCost = 0;

        [Space()]
        [SerializeField] private float m_sizeFactor = 1.0f;
        #endregion

        #region Properties
        /// <summary>
        /// The name of the class to show when needed on UI elements
        /// </summary>
        public string ClassName { get { return m_className; } }

        /// <summary>
        /// Action Points (AP) that the unit will have at the beginning of a <see cref="PlayerController"/> turn
        /// </summary>
        public int ActionPoints { get { return m_actionPoints; } }

        /// <summary>
        /// Starting HP of the unit
        /// </summary>
        public int MaxHealth { get { return m_maxHealth; } }
        
        /// <summary>
        /// Amount of damage that the unit will deal to another unit on a successful attack
        /// </summary>
        public int AttackPower { get { return m_attackPower; } }

        /// <summary>
        /// AP spent when moving the unit a single step
        /// </summary>
        public int MoveCost { get { return m_moveCost; } }

        /// <summary>
        /// Maximum (manhattan) distance at which the unit can attack
        /// </summary>
        public int AttackRange { get { return m_attackRange; } }
        
        /// <summary>
        /// AP spent when performing an attack
        /// </summary>
        public int AttackAPCost { get { return m_attackAPCost; } }

        /// <summary>
        /// The size factor of the unit. Used to scale the cube that will represent the unit.
        /// </summary>
        public float SizeFactor { get { return m_sizeFactor; } }
        #endregion

    }

}