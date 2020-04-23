using Celeste.WaveFunctionCollapse;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CelesteEditor.WaveFunctionCollapse
{
    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : Editor
    {
        #region Properties and Fields

        private TilemapCreator TilemapCreator
        {
            get { return target as TilemapCreator; }
        }

        private bool onlyInstantiateValidSolution = true;

        #endregion

        #region Editor Methods

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            onlyInstantiateValidSolution = EditorGUILayout.Toggle("Only Instantiate Valid Solution", onlyInstantiateValidSolution);

            if (GUILayout.Button("Instantiate"))
            {
                Clear();

                TilemapCreator.tilemap.InstantiateSolution(TilemapCreator.gameObject.transform);
            }

            if (GUILayout.Button("Create and Instantiate"))
            {
                Clear();

                if (TilemapCreator.tilemap.Create() || !onlyInstantiateValidSolution)
                {
                    TilemapCreator.tilemap.InstantiateSolution(TilemapCreator.gameObject.transform);
                }
            }
        }

        #endregion

        #region Management

        private void Clear()
        {
            Transform creatorTransform = TilemapCreator.gameObject.transform;

            for (int i = creatorTransform.childCount - 1; i >= 0; --i)
            {
#if UNITY_EDITOR
                DestroyImmediate(creatorTransform.GetChild(i).gameObject);
#else
                Destroy(creatorTransform.GetChild(i).gameObject);
#endif
            }
        }

        #endregion
    }
}
