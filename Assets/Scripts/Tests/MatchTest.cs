using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CubeWars
{

    namespace Tests
    {
        public class MatchTest
        {

            [UnityTest]
            public IEnumerator MatchManagerTest()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("MatchTestScene");
                var matchManager = MonoBehaviour.FindObjectOfType<MatchManager>();

                int iterations = 1000;
                while (--iterations > 0)
                {
                    var width = Random.Range(20, 40);
                    var height = Random.Range(20, 40);
                    var freeSlots = width * height / 2;
                    var settings = ScriptableObject.CreateInstance<MatchSettings>();
                    settings.MapSize = new Vector2Int(width, height);
                    var spawnPos = new Vector2Int(0, 0);
                    var playerUnitsNumber = new int[3];
                    void advanceSpawnPos()
                    {
                        if (spawnPos.y == height - 1)
                        {
                            spawnPos.Set(0, spawnPos.y + 1);
                        }
                        else
                        {
                            spawnPos.x += 1;
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var playerConfig = new MatchSettings.FactionConfig();
                        playerConfig.PlayerType = i == 0 ? PlayerType.Human : PlayerType.Bot;
                        playerConfig.Units = new List<MatchSettings.UnitConfig>();
                        var playerUnits = Random.Range(1, freeSlots / 4);
                        playerUnitsNumber[i] = playerUnits;
                        freeSlots -= playerUnits;
                        while (playerUnits-- > 0)
                        {
                            playerConfig.Units.Add(new MatchSettings.UnitConfig()
                            {
                                SpawnPosition = spawnPos,
                                UnitType = Random.Range(0, 3)
                            });
                            advanceSpawnPos();
                        }
                        settings.PlayerConfigs.Add(playerConfig);
                    }

                    matchManager.CreateMatch(settings);
                    Assert.IsTrue(matchManager.Factions.Count == 3);
                    for (int i = 0; i < 3; i++)
                    {
                        var realCount = 0;
                        foreach (var unit in matchManager.EnemyUnits(matchManager.Factions[i].Units[0]))
                        {
                            realCount++;
                        }

                        var expectedCount = 0;
                        for (int j = 0; j < 3; j++)
                        {
                            if (j != i)
                            {
                                expectedCount += playerUnitsNumber[j];
                            }
                        }

                        Assert.IsTrue(realCount == expectedCount);
                    }
                }
            }

            [UnityTest]
            public IEnumerator GameControllerRandomWalkerTest()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("MatchTestScene");
                var gameController = MonoBehaviour.FindObjectOfType<GameController>();
                var matchManager = MonoBehaviour.FindObjectOfType<MatchManager>();

                int iterations = 10;
                Time.timeScale = 100.0f;
                while (--iterations > 0)
                {
                    var settings = MatchSettings.CreateTestMatchConfig(AIType.RandomWalker);
                    settings.PlayerConfigs[0].PlayerType = PlayerType.Bot;
                    gameController.InitMatch(settings);
                    while (!gameController.GameFinished() && gameController.CurrentTurnNumber < 20)
                    {
                        yield return gameController.MatchStep();
                    }

                    // All units should be alive and with full HP
                    for (int i = 0; i < matchManager.Factions.Count; i++)
                    {
                        Assert.IsTrue(matchManager.Factions[i].Units.Count == settings.PlayerConfigs[i].Units.Count);
                        foreach (var unit in matchManager.AllUnits())
                        {
                            Assert.IsTrue(unit.IsFullHP);
                        }
                    }
                }
            }

            [UnityTest]
            public IEnumerator GameControllerKillerTest()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("MatchTestScene");
                var gameController = MonoBehaviour.FindObjectOfType<GameController>();
                var matchManager = MonoBehaviour.FindObjectOfType<MatchManager>();

                Time.timeScale = 100.0f;
                int iterations = 5;
                while (--iterations > 0)
                {
                    var settings = MatchSettings.CreateRandomMatchConfig(10, 10, 3, new AIType[] { AIType.Aggressive, AIType.RandomWalker, AIType.RandomWalker });
                    settings.PlayerConfigs[0].PlayerType = PlayerType.Bot;
                    foreach(var unit in settings.PlayerConfigs[0].Units)
                    {
                        unit.UnitType = 1;
                    }
                    gameController.InitMatch(settings);
                    while (!gameController.GameFinished())
                    {
                        yield return gameController.MatchStep();
                    }

                    foreach(var unit in matchManager.Factions[0].Units)
                    {
                        Assert.IsTrue(unit.IsFullHP);
                    }
                    Assert.IsTrue(matchManager.Factions[1].IsDead);
                    Assert.IsTrue(matchManager.Factions[2].IsDead);
                }
            }

            [UnityTest]
            public IEnumerator ScriptedMatchTest()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("MatchTestScene");
                var gameController = MonoBehaviour.FindObjectOfType<GameController>();
                var matchManager = MonoBehaviour.FindObjectOfType<MatchManager>();

                var iterations = 100;
                while (iterations-- > 0)
                {
                    var settings = ScriptableObject.CreateInstance<MatchSettings>();
                    settings.MapSize = new Vector2Int(10, 10);
                    settings.PlayerConfigs = new List<MatchSettings.FactionConfig>();

                    var player0 = new MatchSettings.FactionConfig()
                    {
                        PlayerType = PlayerType.Scripted,
                        Units = new List<MatchSettings.UnitConfig>()
                    };

                    player0.Units.Add(new MatchSettings.UnitConfig()
                    {
                        SpawnPosition = new Vector2Int(0, 0),
                        UnitType = Random.Range(0, 3)
                    });

                    var player1 = new MatchSettings.FactionConfig()
                    {
                        PlayerType = PlayerType.Scripted,
                        Units = new List<MatchSettings.UnitConfig>()
                    };

                    player1.Units.Add(new MatchSettings.UnitConfig()
                    {
                        SpawnPosition = new Vector2Int(0, 1),
                        UnitType = Random.Range(0, 3)
                    });

                    settings.PlayerConfigs.Add(player0);
                    settings.PlayerConfigs.Add(player1);

                    gameController.InitMatch(settings);
                    Assert.IsTrue(gameController.ScriptedPlayerControllers != null);
                    Assert.IsTrue(gameController.ScriptedPlayerControllers.Count == 2);
                    var player0ctrl = gameController.ScriptedPlayerControllers[0];
                    var player1ctrl = gameController.ScriptedPlayerControllers[1];
                    var player0unit = player0ctrl.FactionManager.Units[0];
                    var player1unit = player1ctrl.FactionManager.Units[0];
                    player0ctrl.EnqueueAttackDecision(player0unit, player1unit);
                    Assert.IsTrue(gameController.CurrentTurn == 0);
                    yield return gameController.MatchStep();
                    Assert.IsTrue(gameController.CurrentTurn == 1);
                    var expectedHP = Mathf.Max(0, player1unit.TotalHP - player0unit.AttackPower);
                    if (expectedHP == 0)
                    {
                        Assert.IsTrue(matchManager.Factions[1].Units.Count == 0);
                    }
                    if (!gameController.GameFinished())
                    {
                        var unit1moves = Mathf.Min(player1unit.AvailableMoves, settings.MapSize.x - 1);
                        var unit1expectedpos = new Vector2Int(0, unit1moves);
                        player1ctrl.EnqueueMoveDecision(player1unit, unit1expectedpos);
                        yield return gameController.MatchStep();
                        Assert.IsTrue(gameController.CurrentTurn == 0);
                        if (player1unit.CellPosition != unit1expectedpos)
                        {
                            player1ctrl.EnqueueMoveDecision(player1unit, unit1expectedpos);
                            yield return gameController.MatchStep();
                        }
                        Assert.IsTrue(player1unit.CellPosition == unit1expectedpos);
                    }
                }
            }

        }
    }


}
