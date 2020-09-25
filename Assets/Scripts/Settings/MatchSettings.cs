using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Holds information about how the match must be set up. This is:
    /// <list type="bullet">
    /// <item>Map size</item>
    /// <item>Number of factions</item>
    /// <item>Units with class and position for each faction</item>
    /// </list>
    /// </summary>
    [CreateAssetMenu(menuName = "CubeWars/Match settings")]
    public class MatchSettings : ScriptableObject
    {

        [System.Serializable]
        public class UnitConfig
        {
            public int UnitType;
            public Vector2Int SpawnPosition;
        }

        [System.Serializable]
        public class FactionConfig
        {
            public PlayerType PlayerType;
            public AIType AIType;
            public List<UnitConfig> Units;
        }

        public Vector2Int MapSize;
        public List<FactionConfig> PlayerConfigs = new List<FactionConfig>();

        /// <summary>
        /// Creates a configuration with random positioning
        /// </summary>
        /// <returns></returns>
        public static MatchSettings CreateRandomMatchConfig(int width, int height, int unitsPerPlayer, AIType[] defaultAIType)
        {
            var matchConfig = CreateInstance<MatchSettings>();
            matchConfig.MapSize = new Vector2Int(width, height);

            Matrix2D<int> cells = new Matrix2D<int>(width, height, 0);

            for (int i = 0; i < 3; i++)
            {
                var factionConfig = new MatchSettings.FactionConfig
                {
                    PlayerType = i == 0 ? PlayerType.Human : PlayerType.Bot,
                    Units = new List<UnitConfig>(),
                    AIType = defaultAIType[i]
                };
                for (int j = 0; j < unitsPerPlayer; j++)
                {
                    var unitSpawnPosition = new Vector2Int();
                    do
                    {
                        unitSpawnPosition.x = Random.Range(0, matchConfig.MapSize.x);
                        unitSpawnPosition.y = Random.Range(0, matchConfig.MapSize.y);
                    }
                    while (cells.Get(unitSpawnPosition.x, unitSpawnPosition.y) != 0);
                    cells.Set(unitSpawnPosition.x, unitSpawnPosition.y, 1);
                    factionConfig.Units.Add(new UnitConfig()
                    {
                        UnitType = j % 3,
                        SpawnPosition = unitSpawnPosition,
                    });
                }
                matchConfig.PlayerConfigs.Add(factionConfig);
            }

            return matchConfig;
        }


        public static MatchSettings CreateRandomMatchConfig(int width, int height, int unitsPerPlayer, AIType defaultAIType)
        {
            return CreateRandomMatchConfig(width, height, unitsPerPlayer, new AIType[] { defaultAIType, defaultAIType, defaultAIType });
        }


        public static MatchSettings CreateInfiniteRandomMatchConfig()
        {
            return CreateRandomMatchConfig(10, 10, 3, AIType.Aggressive);
        }

        public static MatchSettings CreateTestMatchConfig(AIType defaultAIType)
        {
            var width = Random.Range(20, 40);
            var height = Random.Range(20, 40);
            var playerUnits = Random.Range(1, 5);
            return CreateRandomMatchConfig(width, height, playerUnits, defaultAIType);
        }

        public static MatchSettings CreateTestMatchConfig(AIType[] defaultAIType)
        {
            var width = Random.Range(20, 40);
            var height = Random.Range(20, 40);
            var playerUnits = Random.Range(1, width * height / 6);
            return CreateRandomMatchConfig(width, height, playerUnits, defaultAIType);
        }

        /// <summary>
        /// Checks that there's only one human, that the units don't overlap and that they don't fall outside the bounds of the map
        /// </summary>
        public void OnValidate()
        {
            int numHumans = 0;
            List<Vector2Int> tookPositions = new List<Vector2Int>();
            foreach(var playerConfig in PlayerConfigs)
            {
                numHumans += playerConfig.PlayerType == PlayerType.Human ? 1 : 0;
                foreach(var unitConfig in playerConfig.Units)
                {
                    var spawnPos = unitConfig.SpawnPosition;
                    Debug.Assert(!tookPositions.Contains(spawnPos), "An unit is in the place of another unit!");
                    var insideBounds = spawnPos.x >= 0 && spawnPos.x < MapSize.x && spawnPos.y >= 0 && spawnPos.y < MapSize.y;
                    Debug.Assert(insideBounds, "The unit is outside the bounds of the map!");
                    tookPositions.Add(unitConfig.SpawnPosition);
                }
            }
            Debug.Assert(numHumans <= 1, "Number of human player types MUST BE ONE OR NONE");
        }

    }

}