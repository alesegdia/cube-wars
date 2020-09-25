using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CubeWars
{

    namespace Tests
    {
        public class UnitFactoryTest
        {

            [UnityTest]
            public IEnumerator UnitDamageDealingTest()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("UnitTestScene");
                var unitFactory = MonoBehaviour.FindObjectOfType<UnitFactory>();

                var iterations = 100;
                while (--iterations > 0)
                {
                    var unit1 = unitFactory.CreateUnit(Random.Range(0, 3), 0, new Vector2Int(0, 0));
                    var unit2 = unitFactory.CreateUnit(Random.Range(0, 3), 1, new Vector2Int(0, 0));
                    Assert.IsTrue(unit1 != null);
                    Assert.IsTrue(unit2 != null);
                    yield return unit1.AttackedBy(unit2, true);
                    var expectedHP = Mathf.Max(0, unit1.TotalHP - unit2.AttackPower);
                    Assert.IsTrue(unit1.CurrentHP == expectedHP);
                    Object.Destroy(unit1.gameObject);
                    Object.Destroy(unit2.gameObject);
                }
            }

            [UnityTest]
            public IEnumerator UnitKill()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("UnitTestScene");
                var unitFactory = MonoBehaviour.FindObjectOfType<UnitFactory>();

                var iterations = 100;
                while (--iterations > 0)
                {
                    var unit1 = unitFactory.CreateUnit(Random.Range(0, 3), 0, new Vector2Int(0, 0));
                    var unit2 = unitFactory.CreateUnit(Random.Range(0, 3), 1, new Vector2Int(0, 0));
                    var attacksToKill = Mathf.CeilToInt(((float)unit1.TotalHP) / ((float)unit2.AttackPower));
                    while (attacksToKill > 0)
                    {
                        yield return unit1.AttackedBy(unit2, true);
                        if (attacksToKill == 1)
                        {
                            Assert.IsTrue(unit1.CurrentHP <= unit2.AttackPower);
                        }
                        attacksToKill = attacksToKill - 1;
                    }
                    Object.Destroy(unit1.gameObject);
                    Object.Destroy(unit2.gameObject);
                }
            }

            [UnityTest]
            public IEnumerator SpendActionPoints()
            {
                yield return TestUtils.WaitUntilSceneIsLoaded("UnitTestScene");
                var unitFactory = MonoBehaviour.FindObjectOfType<UnitFactory>();

                var iterations = 100;
                while (--iterations > 0)
                {
                    var unit = unitFactory.CreateUnit(Random.Range(0, 3), 0, new Vector2Int(0, 0));
                    unit.RegenerateAP();
                    var decrement = Random.Range(1, 5);
                    var neededDecrements = unit.TotalAP / decrement;
                    var module = unit.TotalAP % decrement;

                    while (neededDecrements > 0)
                    {
                        Assert.IsTrue(unit.TryToSpend(decrement));
                        neededDecrements--;
                    }

                    if (module > 0)
                    {
                        Assert.IsTrue(unit.TryToSpend(module));
                    }
                    else
                    {
                        Assert.IsFalse(unit.TryToSpend(1));
                    }
                    Assert.IsTrue(unit.CurrentAP == 0);
                }
            }

        }
    }


}
