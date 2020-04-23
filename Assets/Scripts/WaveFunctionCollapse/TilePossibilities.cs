using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Celeste.WaveFunctionCollapse
{
    public class TilePossibilities
    {
        #region Serialized Fields

        public List<Tile> possibleTiles;

        #endregion

        #region Properties and Fields

        public bool HasPossibilities
        {
            get { return possibleTiles.Count > 0; }
        }

        public bool HasCollapsed
        {
            get { return collapsed; }
        }

        private int x;
        private int y;
        private bool collapsed = false;

        #endregion

        public TilePossibilities(int x, int y, List<Tile> tiles)
        {
            this.x = x;
            this.y = y;
            possibleTiles = new List<Tile>(tiles);
        }

        #region Possibility Functions

        public void Collapse()
        {
            // IMPROVEMENT: NEED TO USE WEIGHT

            Debug.Assert(HasPossibilities);
            Tile chosenTile = possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count)];

            possibleTiles.Clear();
            possibleTiles.Add(chosenTile);
            collapsed = true;
        }

        public void RemoveUnsupportedPossibilitiesFor(Tile other, Direction direction)
        {
            for (int i = possibleTiles.Count - 1; i >= 0; --i)
            {
                if (!possibleTiles[i].SupportsTile(other, direction))
                {
                    Debug.Log(string.Format("Removing possibility: {0} from {1}, {2} because of unsupported Tile {3} in Direction {4}", possibleTiles[i].name, x, y, other?.name, direction.ToDisplayString()));
                    possibleTiles.RemoveAt(i);
                }
            }
        }

        #endregion
    }
}