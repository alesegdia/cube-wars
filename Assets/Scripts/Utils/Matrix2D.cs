using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeWars
{

    public class Matrix2D<T>
    {
        private T[] m_cells;
        private Vector2Int m_size;

        public Vector2Int Size
        {
            get
            {
                return m_size;
            }
        }

        public Matrix2D(int width, int height, T fill)
        {
            m_cells = new T[width * height];
            m_size.Set(width, height);
            Fill(fill);
        }

        public void Fill(T v)
        {
            for (int i = 0; i < m_size.x * m_size.y; i++)
            {
                m_cells[i] = v;
            }
        }

        public T Get(int x, int y)
        {
            return m_cells[coordToIndex(x, y)];
        }

        public T Get(Vector2Int c)
        {
            return m_cells[coordToIndex(c.x, c.y)];
        }

        public void Set(int x, int y, T c)
        {
            m_cells[coordToIndex(x, y)] = c;
        }

        public void Set(Vector2Int cell, T v)
        {
            Set(cell.x, cell.y, v);
        }

        public void Set(T[] values)
        {
            Debug.Assert(values.Length == Size.x * Size.y);
            m_cells = values;
        }

        private int coordToIndex(int x, int y)
        {
            return y * m_size.x + x;
        }
    }

}