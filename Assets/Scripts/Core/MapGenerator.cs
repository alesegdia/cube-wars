using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    /// <summary>
    /// Entity responsible for building the map and an overlay to change the map default's color
    /// for visual purposes in the game
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {

        /// <summary>
        /// Used to associate numbers to colors that will alternate in the grid
        /// </summary>
        [Serializable]
        public class ColorEntry
        {
            public Color ColorA;
            public Color ColorB;
        }

        #region Serialized Fields
        [SerializeField] private MeshFilter m_meshFilter = null;
        [SerializeField] private List<ColorEntry> m_cellColors = new List<ColorEntry>();
        #endregion

        #region Properties
        /// <summary>
        /// A Matrix2D where ints will be associated with the <see cref="m_cellColors"/>
        /// </summary>
        private Matrix2D<int> colorMap { get; set; }

        /// <summary>
        /// Color array to set in the mesh
        /// </summary>
        private List<Color> colors { get; set; } = new List<Color>();
        #endregion

        /// <summary>
        /// Clears the overlay <see cref="colorMap"/> layer to 0
        /// </summary>
        public void ClearOverlay()
        {
            colorMap.Fill(0);
            ApplyOverlay();
        }

        /// <summary>
        /// Sets a color cell to a different index
        /// </summary>
        /// <param name="x">X coordinate of the cell</param>
        /// <param name="y">Y coordinate of the cell</param>
        /// <param name="v">value to set in the cell</param>
        public void SetOverlayCell(int x, int y, int v)
        {
            colorMap.Set(x, y, v);
        }

        /// <summary>
        /// Applies the <see cref="colorMap"/> overlay with indexed colors at <see cref="m_cellColors"/>
        /// </summary>
        public void ApplyOverlay()
        {
            for (int i = 0; i < colorMap.Size.x; i++)
            {
                for (int j = 0; j < colorMap.Size.y; j++)
                {
                    var colorEntry = m_cellColors[colorMap.Get(i, j)];
                    setColor(i, j, (i + j) % 2 == 0 ? colorEntry.ColorA : colorEntry.ColorB);
                }
            }
            var mesh = m_meshFilter.mesh;
            mesh.SetColors(colors);
            m_meshFilter.mesh = mesh;
        }

        /// <summary>
        /// Sets the color of a quad in the <see cref="colors"/> array
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        private void setColor(int x, int y, Color c)
        {
            var startIndex = (colorMap.Size.y * x + y) * 4;
            colors[startIndex] = c;
            colors[startIndex + 1] = c;
            colors[startIndex + 2] = c;
            colors[startIndex + 3] = c;
        }

        /// <summary>
        /// Builds the map
        /// </summary>
        /// <param name="tileWidth">Width of the tiles</param>
        /// <param name="tileHeight">Height of the tiles</param>
        /// <param name="width">Width of the map in tiles</param>
        /// <param name="height">Height of the map in tiles</param>
        public void CreateMap(float tileWidth, float tileHeight, int width, int height)
        {
            var meshCollider = GetComponent<MeshCollider>();
            if(meshCollider)
            {
                Destroy(meshCollider);
            }

            colorMap = new Matrix2D<int>(width, height, 0);

            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            colors.Clear();

            var vIdx = 0;
            var cellIdx = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var i0 = vIdx; vIdx++;
                    var i1 = vIdx; vIdx++;
                    var i2 = vIdx; vIdx++;
                    var i3 = vIdx; vIdx++;
                    vertices.Add(new Vector3( i      * tileWidth, 0, j       * tileHeight));
                    vertices.Add(new Vector3((i + 1) * tileWidth, 0, j       * tileHeight));
                    vertices.Add(new Vector3( i      * tileWidth, 0, (j + 1) * tileHeight));
                    vertices.Add(new Vector3((i + 1) * tileWidth, 0, (j + 1) * tileHeight));
                    normals.Add(Vector3.up); normals.Add(Vector3.up); normals.Add(Vector3.up); normals.Add(Vector3.up);
                    triangles.Add(i0); triangles.Add(i2); triangles.Add(i1); // triangle 1
                    triangles.Add(i2); triangles.Add(i3); triangles.Add(i1); // triangle 2

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));

                    var color = Color.magenta;
                    colors.Add(color); colors.Add(color); colors.Add(color); colors.Add(color);
                    cellIdx++;
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.SetColors(colors);
            mesh.SetUVs(0, uvs);

            m_meshFilter.mesh = mesh;

            gameObject.AddComponent<MeshCollider>();
        }

    }

}
