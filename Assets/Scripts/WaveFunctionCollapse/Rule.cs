using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Celeste.WaveFunctionCollapse
{
    [CreateAssetMenu(fileName = "Rule", menuName = "Celeste/Wave Function Collapse/Rule")]
    public class Rule : ScriptableObject
    {
        #region Serialized Fields

        public Tile otherTile;
        public Direction direction;

        #endregion
    }
}

// IMPROVEMENT: Have a rule which allows empty tiles?