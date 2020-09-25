using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// <see cref="PlayerController"/> for the human player. Creates decision depending on player input.
    /// </summary>
    public class HumanPlayerController : PlayerController
    {

        #region Properties
        /// <summary>
        /// <see cref="Unit"/> selected in the UI that will perform next movements
        /// </summary>
        public Unit SelectedUnit { get; private set; }

        /// <summary>
        /// Tells if the <see cref="UIController"/> detected a mouse click. This class is responsible for turning it off when read.
        /// </summary>
        private bool mouseClicked { get; set; } = false;

        /// <summary>
        /// Tells if the <see cref="UIController"/> detected a click on the End Turn button for the human player
        /// </summary>
        private bool endTurnRequested { get; set; } = false;
        #endregion

        /// <summary>
        /// Message sent by the <see cref="UIController"/> when detecting a click
        /// </summary>
        public void OnClick()
        {
            mouseClicked = true;
        }

        /// <summary>
        /// Message sent by the <see cref="UIController"/> when detected a End Turn button press
        /// </summary>
        public void EndTurn()
        {
            endTurnRequested = true;
        }

        /// <summary>
        /// Handles all the logic for the human player input stages
        /// </summary>
        public override void Think()
        {
            if(endTurnRequested)
            {
                endTurnRequested = false;
                NextDecision = new Decision()
                {
                    Type = DecisionType.EndTurn
                };
            }
            if(mouseClicked)
            {
                mouseClicked = false;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Unit"))
                    {
                        var unit = hit.collider.gameObject.GetComponentInParent<Unit>();
                        if(unit.Faction == FactionManager.FactionIndex)
                        {
                            if( unit == SelectedUnit )
                            {
                                deselectUnit();
                            }
                            else
                            {
                                deselectUnit();
                                SelectedUnit = unit;
                                MatchManager.ActivateUIForUnit(SelectedUnit);
                                SelectedUnit.gameObject.AddComponent<SelectedUnit>();
                            }
                        }
                        else
                        {
                            if(SelectedUnit != null)
                            {
                                NextDecision = CreateAttackDecision(SelectedUnit, unit);
                            }
                        }
                    }
                    else if(hit.collider.CompareTag("Map") && SelectedUnit)
                    {
                        var cell = MatchManager.MapManager.GetCell(hit.point);
                        if(MapManager.IsFree(cell))
                        {
                            NextDecision = CreateMoveDecision(SelectedUnit, cell);
                        }
                    }
                }
                else
                {
                    deselectUnit();
                }
            }
        }

        /// <summary>
        /// Deselects an unit, removing the <see cref="SelectedUnit"/> component and turning it back to its default state
        /// </summary>
        private void deselectUnit()
        {
            if (SelectedUnit != null)
            {
                var selectedUnitComponent = SelectedUnit.GetComponent<SelectedUnit>();
                selectedUnitComponent.Deselect();
                Destroy(selectedUnitComponent);
                MatchManager.DeactivateUIForUnit(SelectedUnit);
                SelectedUnit = null;
            }

        }

    }

}