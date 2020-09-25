using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public class Unit : MonoBehaviour
    {
        #region Readonly Fields
        private const float DamageVFXShakeForce = 0.1f;
        private const float DamageVFXShakeDuration = 0.5f;
        #endregion

        #region Serialized Fields
        [SerializeField] private GameObject m_APBar = null;
        [SerializeField] private GameObject m_HPBar = null;
        [SerializeField] private Renderer m_cubeRenderer = null;
        [SerializeField] private GameObject m_bullet = null;
        #endregion

        #region Properties
        public int CurrentHP { get; set; }

        public int TotalHP { get { return unitSettings.MaxHealth; } }

        public int CurrentAP { get; private set; }

        public int TotalAP { get { return unitSettings.ActionPoints; } }

        /// <summary>
        /// Unit shared stats
        /// </summary>
        private UnitSettings unitSettings { get; set; }

        public int MoveCost { get { return unitSettings.MoveCost; } }

        public int AttackPower { get { return unitSettings.AttackPower; } }

        /// <summary>
        /// The color originally given to the unit. Saved because the color can change
        /// for VFX during the game, so we are able to reestablish it to the original one.
        /// </summary>
        private Color originalColor { get; set; }

        /// <summary>
        /// The position of this unit in the map
        /// </summary>
        public Vector2Int CellPosition { get; private set; } = new Vector2Int(0, 0);

        /// <summary>
        /// Faction index of this unit
        /// </summary>
        public int Faction { get; private set; }

        public int AttackRange { get { return unitSettings.AttackRange; } }

        /// <summary>
        /// Current color of this unit. Taken from the cube renderer.
        /// </summary>
        private Color Color
        {
            get
            {
                return m_cubeRenderer.material.color;
            }
            set
            {
                m_cubeRenderer.material.color = value;
            }
        }

        /// <summary>
        /// Number of available moves that this unit can do after or before spending AP for an attack
        /// </summary>
        public int AvailableMovesWithAttack
        {
            get
            {
                return Mathf.Max(0, (CurrentAP - unitSettings.AttackAPCost) / unitSettings.MoveCost);
            }
        }

        /// <summary>
        /// Number of available moves that this unit can do with full AP
        /// </summary>
        public int AvailableMoves
        {
            get
            {
                return TotalAP / unitSettings.MoveCost;
            }
        }

        /// <summary>
        /// Number of available moves that this unit can do currently
        /// </summary>
        public int CurrentAvailableMoves
        {
            get
            {
                return CurrentAP / unitSettings.MoveCost;
            }
        }

        /// <summary>
        /// Checks if the <see cref="CurrentHP"/> is greater than zero
        /// </summary>
        public bool IsDead
        {
            get
            {
                return CurrentHP <= 0;
            }
        }

        /// <summary>
        /// Information about this unit stats given in a human readable format
        /// </summary>
        public string StatsInfoString
        {
            get
            {
                return $"<align=center><size=40>{unitSettings.ClassName}</size>\n" +
                       $"HP: {CurrentHP}/{unitSettings.MaxHealth}\n" +
                       $"AP: {CurrentAP}/{unitSettings.ActionPoints}\n" +
                       $"Attack Power: {AttackPower}\n" +
                       $"Attack Range: {unitSettings.AttackRange}\n" +
                       $"Attack Cost: {unitSettings.AttackAPCost}\n" +
                       $"Move Cost: {unitSettings.MoveCost}</align>";
            }
        }

        public bool IsFullHP
        {
            get
            {
                return CurrentHP == unitSettings.MaxHealth;
            }
        }
        #endregion

        /// <summary>
        /// Checks if this unit has <paramref name="otherUnit"/> in range
        /// </summary>
        /// <param name="otherUnit"></param>
        /// <returns></returns>
        public bool CanAttack(Unit otherUnit)
        {
            var distance = Utils.ManhattanDistance(CellPosition, otherUnit.CellPosition);
            return distance <= AttackRange;
        }

        /// <summary>
        /// Initializes the <see cref="Unit"/> given the unit model settings, faction, color and position.
        /// </summary>
        /// <param name="unitData">The model that represents this unit class</param>
        /// <param name="faction">The faction index that this unit belongs to</param>
        /// <param name="color">The color of the unit</param>
        /// <param name="cell">The position where to place the unit</param>
        public void Initialize(UnitSettings unitData, int faction, Color color, Vector2Int cell)
        {
            var sizeFactor = unitData.SizeFactor;
            transform.localScale = new Vector3(sizeFactor, sizeFactor, sizeFactor);
            CellPosition = cell;
            unitSettings = unitData;
            Faction = faction;
            originalColor = color;
            m_cubeRenderer.material.color = color;
            CurrentHP = unitSettings.MaxHealth;
            m_bullet.SetActive(false);

            // fixing transparent render order issue
            m_cubeRenderer.material.renderQueue = 5000;
        }

        /// <summary>
        /// Set the unit partially invisible. Used to represent if this unit is in range
        /// when a <see cref="HumanPlayerController.SelectedUnit"/> is selected.
        /// </summary>
        public void SetInvisible()
        {
            m_cubeRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        }

        /// <summary>
        /// Turns off the partial invisibility of the unit.
        /// </summary>
        public void SetVisible()
        {
            m_cubeRenderer.material.color = originalColor;
        }

        /// <summary>
        /// VFX launched when the unit dies
        /// </summary>
        /// <returns></returns>
        private IEnumerator dieVFX()
        {
            yield return damageDealtVFX();
            var t = 0.0f;
            var initScale = transform.localScale.x;
            while(t < 1.0f)
            {
                var scale = Mathf.Lerp(initScale, 0.0f, t);
                t += Time.deltaTime * 4.0f;
                transform.localScale = new Vector3(scale, scale, scale);
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Sets the unit cell in the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PlaceAt(int x, int y)
        {
            CellPosition = new Vector2Int(x, y);
        }

        /// <summary>
        /// Sets the unit cell in the map
        /// </summary>
        /// <param name="newCell"></param>
        public void PlaceAt(Vector2Int newCell)
        {
            CellPosition = new Vector2Int(newCell.x, newCell.y);
        }

        /// <summary>
        /// Handles logic for this unit attacked by <paramref name="otherUnit"/> and launches proper VFX
        /// </summary>
        /// <param name="otherUnit"></param>
        /// <returns></returns>
        public IEnumerator AttackedBy(Unit otherUnit, bool ignoreAnimations = false)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - otherUnit.AttackPower);
            updateHPBar();
            if (!ignoreAnimations)
            {
                yield return bulletAnimation(otherUnit);
                if (IsDead)
                {
                    yield return dieVFX();
                }
                else
                {
                    yield return damageDealtVFX();
                }
            }
        }

        /// <summary>
        /// VFX for a this unit attacking <paramref name="otherUnit"/> with a bullet
        /// </summary>
        /// <param name="otherUnit"></param>
        /// <returns></returns>
        private IEnumerator bulletAnimation(Unit otherUnit)
        {
            m_bullet.SetActive(true);
            m_bullet.transform.position = transform.position;
            var t = 0.0f;
            var source = otherUnit.transform.position;
            var target = transform.position;
            var distance = Vector3.Distance(source, target);
            while(t < 1.0f)
            {
                m_bullet.transform.position = Vector3.Lerp(source, target, t);
                t += Time.deltaTime / distance * 4.0f;
                yield return new WaitForEndOfFrame();
            }
            m_bullet.SetActive(false);
        }

        /// <summary>
        /// VFX launched when this unit suffers damage
        /// </summary>
        /// <returns></returns>
        private IEnumerator damageDealtVFX()
        {
            var startColor = Color.white;
            startColor.a = 0.1f;
            var endColor = Color;
            var t = 0.0f;
            Color = startColor;
            var basePosition = transform.position;
            while(t < 1.0f)
            {
                Color = Color.Lerp(startColor, endColor, t);
                t += Time.deltaTime / DamageVFXShakeDuration;
                var oneMinusT = 1.0f - t;
                transform.position = basePosition +
                    new Vector3(Random.Range(-DamageVFXShakeForce, DamageVFXShakeForce) * oneMinusT,
                                Random.Range(-DamageVFXShakeForce, DamageVFXShakeForce) * oneMinusT,
                                Random.Range(-DamageVFXShakeForce, DamageVFXShakeForce) * oneMinusT);
                yield return new WaitForEndOfFrame();
            }
            transform.position = basePosition;
            Color = endColor;
        }

        /// <summary>
        /// Refills the AP bar. Called when the turn of a player starts.
        /// </summary>
        public void RegenerateAP()
        {
            CurrentAP = unitSettings.ActionPoints;
            updateAPBar();
        }

        /// <summary>
        /// Tries to spend an AP amount and notifies about the success of the operation
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool TryToSpend(int amount)
        {
            if (CurrentAP >= amount)
            {
                CurrentAP -= amount;
                updateAPBar();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to spend an AP amount for an attack
        /// </summary>
        /// <returns></returns>
        public bool TryToSpendAPToAttack()
        {
            return TryToSpend(unitSettings.AttackAPCost);
        }

        /// <summary>
        /// Generic method to update a bar (AP or HP)
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="newValue"></param>
        /// <param name="totalValue"></param>
        private void updateBar(GameObject bar, float newValue, float totalValue)
        {
            var scale = bar.transform.localScale;
            scale.x = newValue / totalValue;
            bar.transform.localScale = scale;
        }

        /// <summary>
        /// Updates the AP bar given the <see cref="CurrentAP"/>
        /// </summary>
        private void updateAPBar()
        {
            updateBar(m_APBar, CurrentAP, unitSettings.ActionPoints);
        }

        /// <summary>
        /// Updates the HP bar given the <see cref="CurrentHP"/>
        /// </summary>
        private void updateHPBar()
        {
            updateBar(m_HPBar, CurrentHP, unitSettings.MaxHealth);
        }

        /// <summary>
        /// Fix to make HP and AP bars work as billboards
        /// </summary>
        private void LateUpdate()
        {
            m_APBar.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
            m_HPBar.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
        }

    }

}