using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{
    public enum DecisionType
    {
        Move, Attack, EndTurn
    }

    /// <summary>
    /// Represents a possible decision of a <see cref="PlayerController"/> in game. This class has several
    /// fields that can be interpreted differently or ignored completly depending on the <see cref="DecisionType"/>
    /// </summary>
    public class Decision
    {

        public DecisionType Type { get; set; }
        public Unit SourceUnit { get; set; }
        public Unit TargetUnit { get; set; }
        public Vector2Int Cell0 { get; set; }
        public Vector2Int Cell1 { get; set; }
        public int Moves { get; set; }
        public PathFinder PathFinder { get; set; }
    }

}