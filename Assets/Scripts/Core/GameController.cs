using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Responsible for booting the match, creating the player controllers and handling the game flow
    /// </summary>
    public class GameController : MonoBehaviour
    {
        #region Readonly Fields
        private static readonly float MoveTimePerCell = 0.25f;
        #endregion

        #region Serialized Fields
        [Header("General")]
        [SerializeField] private MatchManager m_matchManager = null;
        [SerializeField] private MatchSettings m_singleMatchSettings = null;
        [SerializeField] private FactionSettingsDatabase m_factionSettingsDatabase = null;

        [Header("UI")]
        [SerializeField] private UIController m_uiController = null;
        #endregion

        #region Properties
        
        private List<PlayerController> playerControllers { get; set; }

        public int CurrentTurn { get; set; } = 0;

        private HumanPlayerController humanPlayer { get; set; }

        private PlayerController currentPlayer
        {
            get
            {
                return playerControllers[CurrentTurn];
            }
        }

        private MapManager mapManager
        {
            get
            {
                return m_matchManager.MapManager;
            }
        }

        public List<ScriptedPlayerController> ScriptedPlayerControllers { get; private set; } = new List<ScriptedPlayerController>();

        public int CurrentTurnNumber { get; private set; } = 0;
        #endregion

        /// <summary>
        /// Creates the <see cref="PlayerController"/> of a specific type depending on <see cref="PlayerType"/> and <see cref="AIType"/>
        /// </summary>
        /// <param name="faction"></param>
        /// <param name="aiType"></param>
        /// <returns></returns>
        private GameObject createPlayerControllerForFaction(FactionManager faction, AIType aiType)
        {
            var go = new GameObject()
            {
                name = "PlayerController" + faction.FactionIndex
            };
            switch(faction.PlayerType)
            {
                case PlayerType.Human:
                    humanPlayer = go.AddComponent<HumanPlayerController>();
                    break;
                case PlayerType.Bot:
                    switch (aiType)
                    {
                        case AIType.Dummy:
                            go.AddComponent<DummyAIAgentController>();
                            break;
                        case AIType.Aggressive:
                            go.AddComponent<AgressiveAIAgentController>();
                            break;
                        case AIType.RandomWalker:
                            go.AddComponent<RandomWalkerAIAgentController>();
                            break;
                    }
                    break;
                case PlayerType.Scripted:
                    ScriptedPlayerControllers.Add(go.AddComponent<ScriptedPlayerController>());
                    break;
            }
            return go;
        }
        
        /// <summary>
        /// Create the player controllers and initializes the <see cref="MatchManager"/>
        /// </summary>
        /// <param name="matchSettings">The settings to use to set up the match</param>
        public void InitMatch(MatchSettings matchSettings)
        {
            StopAllCoroutines();
            CurrentTurn = 0;
            ScriptedPlayerControllers = new List<ScriptedPlayerController>();
            m_matchManager.CreateMatch(matchSettings);
            playerControllers = new List<PlayerController>();
            foreach (var faction in m_matchManager.Factions)
            {
                var controllerGO = createPlayerControllerForFaction(faction, matchSettings.PlayerConfigs[faction.FactionIndex].AIType);
                var controller = controllerGO.GetComponent<PlayerController>();
                controller.Initialize(m_matchManager, faction);
                playerControllers.Add(controller);
            }
            CurrentTurnNumber = 0;
            m_uiController?.Initialize(humanPlayer);
        }

        /// <summary>
        /// Handles all the match flow from the beginning until some player kills other factions. This can be
        /// interrupted by debug menu messages like <see cref="UIController.ForceLose"/> and <see cref="UIController.ForceWin"/>
        /// </summary>
        /// <returns></returns>
        private IEnumerator runMatch()
        {
            while (!m_matchManager.GameFinished())
            {
                yield return MatchStep();
            }
            m_uiController?.ShowMatchResults();
        }

        /// <summary>
        /// Executes the turn of a single player and advances to the next
        /// </summary>
        /// <returns></returns>
        public IEnumerator MatchStep()
        {
            var turnEnded = false;
            var attackedThisTurn = false;
            m_uiController?.NewTurn(currentPlayer.FactionManager.FactionIndex);
            currentPlayer.TurnStart();
            while (!turnEnded && !m_matchManager.GameFinished() && !m_matchManager.Factions[CurrentTurn].IsDead)
            {
                while (currentPlayer.NextDecision == null)
                {
                    currentPlayer.Think();
                    yield return new WaitForEndOfFrame();
                }
                var playerDecision = currentPlayer.NextDecision;
                turnEnded = playerDecision.Type == DecisionType.EndTurn;

                if (playerDecision.Type == DecisionType.Attack && attackedThisTurn)
                {
                    Log(UIController.MessageType.Info, "Attack already done during this turn!");
                }
                else if (validateDecision(currentPlayer.FactionManager, playerDecision))
                {
                    yield return applyDecision(playerDecision);
                    attackedThisTurn = attackedThisTurn || playerDecision.Type == DecisionType.Attack;
                }
                else
                {
                    Debug.LogWarning("Decision couldn't be applied.");
                }
                currentPlayer.NextDecision = null;
            }
            Log(UIController.MessageType.Info, "Turn finished");
            advanceTurn();
            CurrentTurnNumber++;
        }

        /// <summary>
        /// Advances the turn to the next player
        /// </summary>
        private void advanceTurn()
        {
            CurrentTurn = (CurrentTurn + 1) % playerControllers.Count;
        }

        /// <summary>
        /// Applies an already validated decision
        /// </summary>
        /// <param name="decision">An already validated decision</param>
        /// <returns></returns>
        private IEnumerator applyDecision(Decision decision)
        {
            if(decision.Type != DecisionType.EndTurn && currentPlayer.FactionManager.IsHuman)
            {
                m_matchManager.DeactivateUIForUnit(decision.SourceUnit);
            }
            if (decision.Type == DecisionType.Move)
            {
                yield return applyMove(decision);
            }
            if (decision.Type == DecisionType.Attack)
            {
                yield return applyAttack(decision);
            }
            if (decision.Type != DecisionType.EndTurn && currentPlayer.FactionManager.IsHuman)
            {
                m_matchManager.ActivateUIForUnit(decision.SourceUnit);
            }
        }

        /// <summary>
        /// Applies a decision of <seealso cref="DecisionType.Move"/> kind that was already validated
        /// </summary>
        /// <param name="decision">An already validated decision</param>
        /// <returns></returns>
        private IEnumerator applyMove(Decision decision)
        {
            var path = decision.PathFinder.GetPathTo(decision.Cell1);
            if(path != null)
            {
                var sourceUnit = decision.SourceUnit;
                for (int i = 0; i < path.Count; i++)
                {
                    var nextNode = path[path.Count - i - 1];
                    var sourcePos = sourceUnit.transform.position;
                    var targetPos = mapManager.GetCellPosition(nextNode);
                    targetPos.y = sourcePos.y;
                    float t = 0.0f;
                    while (t < 1.0f)
                    {
                        t += Time.deltaTime / MoveTimePerCell;
                        if (t > 1.0f) t = 1.0f;
                        var pos = Vector3.Lerp(sourcePos, targetPos, t);
                        sourceUnit.transform.position = pos;
                        yield return new WaitForEndOfFrame();
                    }
                }
                mapManager.MoveUnit(sourceUnit, decision.Cell1);
                Log(UIController.MessageType.Info, "Unit moved from " + decision.Cell0 + " to " + decision.Cell1);
            }
        }

        /// <summary>
        /// Applies a decision of <seealso cref="DecisionType.Attack"/> kind that was already validated
        /// </summary>
        /// <param name="decision">An already validated decision</param>
        /// <returns></returns>
        private IEnumerator applyAttack(Decision decision)
        {
            var sourceUnit = decision.SourceUnit;
            var targetUnit = decision.TargetUnit;
            yield return targetUnit.AttackedBy(sourceUnit);
            if(targetUnit.IsDead)
            {
                m_matchManager.NotifyDeath(targetUnit);
            }
            Log(UIController.MessageType.Info, "Unit at " + sourceUnit.CellPosition + " attacked unit at " + targetUnit.CellPosition);
        }

        /// <summary>
        /// Validates a decision.
        /// <list type="bullet">
        /// <item>Checks if the <see cref="Decision"/> is feasible given the <see cref="Unit.CurrentAP"/> and the costs</item>
        /// <item>Checks if the <see cref="Decision"/> is feasible given the position of the <see cref="Unit"/> and the reach</item>
        /// <item>Checks if the <see cref="Decision"/> is feasible given the <see cref="Unit"/> factions if it's <see cref="DecisionType.Attack"/></item>
        /// </list>
        /// </summary>
        /// <param name="factionManager"><see cref="FactionManager"/> that executed the decision</param>
        /// <param name="decision">The unvalidated Decision</param>
        /// <returns>Whether the decision was correctly validated or not</returns>
        private bool validateDecision(FactionManager factionManager, Decision decision)
        {
            if(decision.Type == DecisionType.EndTurn)
            {
                return true;
            }

            if (decision.SourceUnit.Faction != factionManager.FactionIndex)
            {
                Log(UIController.MessageType.Warning, "Trying to control an unowned unit");
                return false;
            }

            if (decision.Type == DecisionType.Attack)
            {
                if (decision.TargetUnit.Faction == factionManager.FactionIndex)
                {
                    Log(UIController.MessageType.Warning, "You can not attack an unit of your own!");
                    return false;
                }
                if (!decision.SourceUnit.CanAttack(decision.TargetUnit))
                {
                    Log(UIController.MessageType.Warning, "Enemy unit out of range!");
                    return false;
                }
                if (decision.TargetUnit == null)
                {
                    Log(UIController.MessageType.Warning, "You can't attack the VOID!");
                    return false;
                }
                if (decision.SourceUnit == null)
                {
                    Log(UIController.MessageType.Warning, "Trying to attack with no unit? Hmmm, smells like bug spirits...");
                    return false;
                }
                if (!decision.SourceUnit.TryToSpendAPToAttack())
                {
                    Log(UIController.MessageType.Warning, "Not enough AP to attack!");
                    return false;
                }
            }
            else if (decision.Type == DecisionType.Move)
            {
                decision.Cell1 = new Vector2Int(Mathf.Max(Mathf.Min(decision.Cell1.x, mapManager.Size.x - 1), 0), Mathf.Max(Mathf.Min(decision.Cell1.y, mapManager.Size.y - 1), 0));
                if (!m_matchManager.MapManager.IsFree(decision.Cell1))
                {
                    Log(UIController.MessageType.Warning, "The target unit is not free!");
                    return false;
                }

                decision.PathFinder = mapManager.GetPathFinderFor(decision.SourceUnit);
                decision.Moves = decision.PathFinder.GetCell(decision.Cell1).Cost;
                if (!decision.SourceUnit.TryToSpend(decision.Moves * decision.SourceUnit.MoveCost))
                {
                    Log(UIController.MessageType.Warning, "Not enough AP to move!");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Stops previous match and creates a new one depending on <see cref="AppController.GameMode"/>
        /// </summary>
        public void StartMatch()
        {
            if(Singleton<AppController>.Instance.GameMode == GameMode.SingleMatch)
            {
                InitMatch(m_singleMatchSettings);
            }
            else
            {
                InitMatch(MatchSettings.CreateInfiniteRandomMatchConfig());
            }
            StartCoroutine(runMatch());
        }

        public bool GameFinished()
        {
            return m_matchManager.GameFinished();
        }

        /// <summary>
        /// Log a message to the UIController, only if the message is <see cref="UIController.MessageType.Info"/> or 
        /// if it's a human player's <see cref="UIController.MessageType.Warning"/>
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="message">The message itself to be logged</param>
        private void Log(UIController.MessageType messageType, string message)
        {
            if(m_matchManager.Factions[CurrentTurn].IsHuman || messageType == UIController.MessageType.Info)
            {
                var color = m_factionSettingsDatabase.GetFactionSettings(CurrentTurn).Color;
                m_uiController?.Log(message, color);
            }
            if(messageType == UIController.MessageType.Warning)
            {
                Debug.LogWarning(CurrentTurn + ": " + message);
            }
        }

    }

}