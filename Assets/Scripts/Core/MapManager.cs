using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CubeWars
{

    /// <summary>
    /// Holds all the information about the map
    /// </summary>
    [RequireComponent(typeof(MapGenerator))]
    public class MapManager : MonoBehaviour
    {

        #region Readonly Fields
        private static readonly float TileWidth = 1.0f;
        private static readonly float TileHeight = 1.0f;
        #endregion

        #region Properties
        /// <summary>
        /// Map with positioned units
        /// </summary>
        private Matrix2D<Unit> cellMap { get; set; }

        /// <summary>
        /// Map with obstacles
        /// </summary>
        private Matrix2D<int> obstacleMap { get; set; }

        /// <summary>
        /// Map generator to create the map
        /// </summary>        
        private MapGenerator mapGenerator { get; set; }

        public Vector2Int Size
        {
            get
            {
                return cellMap.Size;
            }
        }
        #endregion

        /// <summary>
        /// Builds the map
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public void Initialize(int mapWidth, int mapHeight)
        {
            cellMap = new Matrix2D<Unit>(mapWidth, mapHeight, null);
            obstacleMap = new Matrix2D<int>(mapWidth, mapHeight, 0);
            mapGenerator = GetComponent<MapGenerator>();
            mapGenerator.CreateMap(TileWidth, TileHeight, mapWidth, mapHeight);
            mapGenerator.ApplyOverlay();
        }

        /// <summary>
        /// Creates a <see cref="PathFinder"/> for an unit in this map
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public PathFinder GetPathFinderFor(Unit unit)
        {
            return new PathFinder(unit.CellPosition, unit.CurrentAvailableMoves, obstacleMap);
        }

        /// <summary>
        /// Sets up the color map to show the unit movement reach
        /// </summary>
        /// <param name="unit"></param>
        public void SetOverlayForUnitMoveReach(Unit unit)
        {
            PathFinder pf = new PathFinder(unit.CellPosition, unit.CurrentAvailableMoves, obstacleMap);
            for(int i = 0; i < cellMap.Size.x; i++)
            {
                for(int j = 0; j < cellMap.Size.y; j++)
                {
                    var target = new Vector2Int(i, j);
                    if (target == unit.CellPosition) continue;
                    if(pf.ReachabilityMap.Get(target) == 1)
                    {
                        mapGenerator.SetOverlayCell(i, j, 1);
                    }
                }
            }
            mapGenerator.ApplyOverlay();
        }

        /// <summary>
        /// Clears the overlay color map
        /// </summary>
        public void ClearOverlay()
        {
            mapGenerator.ClearOverlay();
        }

        /// <summary>
        /// Checks if a cell position is free of obstacle in the map
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsFree(Vector2Int position)
        {
            return IsFree(position.x, position.y);
        }

        public bool IsFree(int x, int y)
        {
            return cellMap.Get(x, y) == null;
        }

        /// <summary>
        /// Sets an unit in a cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="unit"></param>
        public void PlaceUnit(int x, int y, Unit unit)
        {
            cellMap.Set(x, y, unit);
            obstacleMap.Set(x, y, 1);
        }

        public void PlaceUnit(Vector2Int pos, Unit unit)
        {
            PlaceUnit(pos.x, pos.y, unit);
        }

        /// <summary>
        /// Removes an unit from a cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void RemoveUnit(int x, int y)
        {
            cellMap.Set(x, y, null);
            obstacleMap.Set(x, y, 0);
        }

        public void RemoveUnit(Vector2Int pos)
        {
            RemoveUnit(pos.x, pos.y);
        }

        /// <summary>
        /// Returns a cell given a world position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector2Int GetCell(Vector3 worldPos)
        {
            var inPoint = transform.TransformPoint(worldPos);
            return new Vector2Int((int)(inPoint.x / TileWidth), (int)(inPoint.z / TileHeight));
        }

        public Vector3 GetCellPosition(Vector2Int cell)
        {
            return new Vector3(cell.x * TileWidth + TileWidth / 2.0f, 0, cell.y * TileHeight + TileHeight / 2.0f);
        }

        internal void MoveUnit(Unit sourceUnit, Vector2Int cell)
        {
            RemoveUnit(sourceUnit.CellPosition);
            PlaceUnit(cell, sourceUnit);
            sourceUnit.PlaceAt(cell);
        }

    }

}