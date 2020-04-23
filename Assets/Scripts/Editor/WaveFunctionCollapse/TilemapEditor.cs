using Celeste.WaveFunctionCollapse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CelesteEditor.WaveFunctionCollapse
{
    [CustomEditor(typeof(Tilemap))]
    public class TilemapEditor : Editor
    {
        #region Properties and Fields

        private Tilemap Tilemap
        {
            get { return target as Tilemap; }
        }

        private bool useCustomStartPoint;
        private int customStartPointX;
        private int customStartPointY;
        
        private int collapseLocationX;
        private int collapseLocationY;

        private bool tileInfoSectionOpen = false;
        private List<bool> rowOpen = new List<bool>();
        private List<bool> columnOpen = new List<bool>();

        #endregion

        #region Editor Methods

        public void OnEnable()
        {
            rowOpen.Capacity = Tilemap.rows;
            columnOpen.Capacity = Tilemap.rows * Tilemap.columns;

            for (int i = 0; i < rowOpen.Capacity; ++i)
            {
                rowOpen.Add(false);
            }

            for (int i = 0; i < columnOpen.Capacity; ++i)
            {
                columnOpen.Add(false);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            useCustomStartPoint = EditorGUILayout.Toggle("Custom Start Point", useCustomStartPoint);

            if (useCustomStartPoint)
            {
                customStartPointX = EditorGUILayout.IntField(customStartPointX);
                customStartPointY = EditorGUILayout.IntField(customStartPointY);
            }

            if (GUILayout.Button("Create"))
            {
                if (useCustomStartPoint)
                {
                    Tilemap.Create(customStartPointX, customStartPointY);
                }
                else
                {
                    Tilemap.Create();
                }
            }

            if (GUILayout.Button("Reset"))
            {
                Tilemap.ResetSolution();
            }

            if (GUILayout.Button("Remove Invalid Possibilities"))
            {
                Tilemap.RemoveInvalidPossibilities();
            }

            if (GUILayout.Button("Collapse Location"))
            {
                collapseLocationX = EditorGUILayout.IntField(collapseLocationX);
                collapseLocationY = EditorGUILayout.IntField(collapseLocationY);

                Tilemap.CollapseLocation(collapseLocationX, collapseLocationY);
            }

            if (Tilemap.HasSolutionInfo)
            {
                tileInfoSectionOpen = EditorGUILayout.Foldout(tileInfoSectionOpen, "Tile Info");
                if (tileInfoSectionOpen)
                {
                    Tilemap tilemap = Tilemap;

                    for (int row = 0; row < tilemap.rows; ++row)
                    {
                        rowOpen[row] = EditorGUILayout.Foldout(rowOpen[row], "Row: " + row.ToString());

                        if (rowOpen[row])
                        {
                            List<TilePossibilities> rowPossibilities = tilemap.Solution[row];

                            for (int column = 0, nColumns = Math.Min(rowPossibilities.Count, tilemap.columns); column < nColumns; ++column)
                            {
                                int columnIndex = row * tilemap.columns + column;
                                columnOpen[columnIndex] = EditorGUILayout.Foldout(columnOpen[columnIndex], "Column: " + column.ToString());

                                if (columnOpen[columnIndex])
                                {
                                    foreach (Tile tile in rowPossibilities[column].possibleTiles)
                                    {
                                        EditorGUILayout.LabelField(tile.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
