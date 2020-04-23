using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Celeste.WaveFunctionCollapse
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Celeste/Wave Function Collapse/Tile")]
    public class Tile : ScriptableObject
    {
        #region Serialized Fields

        public float weight = 1;
        public List<Rule> rules = new List<Rule>();
        public GameObject gameObject;

        #endregion

        #region Rules Functions
        
        public bool SupportsTile(Tile tile, Direction direction)
        {
            foreach (Rule rule in rules)
            {
                if (rule.otherTile == tile && rule.direction == direction)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
