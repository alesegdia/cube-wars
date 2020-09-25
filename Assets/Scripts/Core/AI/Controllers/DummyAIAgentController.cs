using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// An AI that does NOTHING. Yes, NOTHING!!!!!!!11
    /// </summary>
    public class DummyAIAgentController : PlayerController
    {

        public override void Think()
        {
            NextDecision = CreateEndTurnDecision();
        }

    }

}