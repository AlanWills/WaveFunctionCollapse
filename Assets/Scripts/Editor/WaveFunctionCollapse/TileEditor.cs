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
    [CustomEditor(typeof(Tile))]
    public class TileEditor : Editor
    {
        #region Properties and Fields

        private Tile Tile
        {
            get { return target as Tile; }
        }

        private Tile other;
        private Direction direction;
        private string supportedText;

        #endregion

        #region Editor Methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            other = EditorGUILayout.ObjectField(other, typeof(Tile), false) as Tile;
            direction = (Direction)EditorGUILayout.EnumPopup(direction);

            if (GUILayout.Button("Check Supported"))
            {
                supportedText = Tile.SupportsTile(other, direction) ? "Supported" : "Not Supported";
            }

            EditorGUILayout.LabelField(supportedText);
        }

        #endregion

    }
}
