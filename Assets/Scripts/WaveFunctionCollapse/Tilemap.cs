using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celeste.WaveFunctionCollapse
{
    [CreateAssetMenu(fileName = "Tilemap", menuName = "Celeste/Wave Function Collapse/Tilemap")]
    public class Tilemap : ScriptableObject
    {
        #region Serialized Fields

        public List<Tile> tiles = new List<Tile>();

        public int rows = 10;
        public int columns = 10;
        public float tileWidth = 1;
        public float tileLength = 1;

        #endregion

        #region Properties and Fields

        public bool HasSolutionInfo
        {
            get { return solution.Count != 0; }
        }

        public List<List<TilePossibilities>> Solution
        {
            get { return solution; }
        }

        private List<List<TilePossibilities>> solution = new List<List<TilePossibilities>>();

        #endregion

        #region Creation Functions

        public bool Create()
        {
            List<int> startingPositions = new List<int>();
            for (int i = 0; i < rows * columns; ++i)
            {
                startingPositions.Add(i);
            }

            // IMPROVEMENT: Use entropy to order the starting positions from lowest to highest

            while (startingPositions.Count > 0)
            {
                int startingPositionIndex = Random.Range(0, startingPositions.Count);
                int startingPosition = startingPositions[startingPositionIndex];
                startingPositions.RemoveAt(startingPositionIndex);

                if (TryCreate(startingPosition % columns, startingPosition / columns))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Create(int startingPositionX, int startingPositionY)
        {
            bool result = TryCreate(startingPositionX, startingPositionY);

            Debug.Assert(result, "No valid configurations found");
            return result;
        }

        public void ResetSolution()
        {
            solution.Clear();

            for (int row = 0; row < rows; ++row)
            {
                List<TilePossibilities> rowPossibilities = new List<TilePossibilities>();
                solution.Add(rowPossibilities);

                for (int column = 0; column < columns; ++column)
                {
                    rowPossibilities.Add(new TilePossibilities(column, row, tiles));
                }
            }
        }

        public void InstantiateSolution(Transform parent)
        {
            for (int row = 0; row < rows; ++row)
            {
                List<TilePossibilities> rowPossibilities = solution[row];

                for (int column = 0; column < columns; ++column)
                {
                    if (rowPossibilities[column].HasCollapsed)
                    {
                        Tile tile = rowPossibilities[column].possibleTiles[0];
                        GameObject tileGameObject = Instantiate(tile.gameObject, parent);
                        tileGameObject.transform.position = new Vector3(column * tileWidth, 0, row * tileLength);
                    }
                }
            }
        }

        private bool TryCreate(int startingLocationX, int startingLocationY)
        {
            Debug.Log(string.Format("Starting at {0}, {1}", startingLocationX, startingLocationY));

            ResetSolution();
            //RemoveInvalidPossibilities();

            while (CollapsedLocationCount() < rows * columns)
            {
                Debug.Log(string.Format("Starting to collapse {0}, {1}", startingLocationX, startingLocationY));
                if (!CollapseLocation(startingLocationX, startingLocationY))
                {
                    // Our algorithm has failed
                    // OPTIMIZATION: Possibility to backtrack here and collapse to a different choice
                    return false;
                }

                Vector2 location = NextLocation();
                startingLocationX = (int)location.x;
                startingLocationY = (int)location.y;
            }

            return true;
        }

        public void RemoveInvalidPossibilities()
        {
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    UpdateFromNeighbours(column, row);
                }
            }
        }

        public bool CollapseLocation(int x, int y)
        {
            Debug.Assert(y < solution.Count);
            List<TilePossibilities> row = solution[y];
            
            Debug.Assert(x < row.Count);
            TilePossibilities location = row[x];

            UpdateFromNeighbours(x, y);

            if (!location.HasPossibilities)
            {
                return false;
            }

            location.Collapse();

            Debug.Assert(location.HasCollapsed, "Invalid number of possible tiles after collapse");
            Debug.Log(string.Format("Location {0}, {1} collapsed to {2}", x, y, location.possibleTiles[0]));

            UpdateNeighbours(x, y, location.possibleTiles[0]);

            // If any of them end up with no possibilities this was a bad choice
            // OPTIMIZATION: Possibility to backtrack here and do another choice
            return DoAllNeighboursHavePossibilities(x, y);
        }

        #endregion

        #region Entropy Functions

        private int CollapsedLocationCount()
        {
            int count = 0;

            for (int row = 0; row < rows; ++row)
            {
                List<TilePossibilities> rowPossibilities = solution[row];

                for (int column = 0; column < columns; ++column)
                {
                    TilePossibilities location = rowPossibilities[column];

                    if (location.HasCollapsed)
                    {
                        ++count;
                    }
                }
            }

            return count;
        }

        private Vector2 NextLocation()
        {
            int x = 0, y = 0;
            int currentPossibilityCount = int.MaxValue;

            for (int row = 0; row < rows; ++row)
            {
                List<TilePossibilities> rowPossibilities = solution[row];

                for (int column = 0; column < columns; ++column)
                {
                    TilePossibilities location = rowPossibilities[column];

                    if (!location.HasCollapsed && location.HasPossibilities && location.possibleTiles.Count < currentPossibilityCount)
                    {
                        x = column;
                        y = row;
                        currentPossibilityCount = location.possibleTiles.Count;
                    }
                }
            }

            return new Vector2(x, y);
        }

        #endregion

        #region Neighbour Functions

        private void UpdateFromNeighbours(int x, int y)
        {
            Debug.Assert(y < solution.Count);
            List<TilePossibilities> row = solution[y];

            Debug.Assert(x < row.Count);
            TilePossibilities location = row[x];

            // Check left
            {
                if (x == 0)
                {
                    location.RemoveUnsupportedPossibilitiesFor(null, Direction.Left);
                }
                else if (row[x - 1].HasCollapsed)
                {
                    location.RemoveUnsupportedPossibilitiesFor(row[x - 1].possibleTiles[0], Direction.Left);
                }
            }

            // Check Right
            {
                if (x == row.Count - 1)
                {
                    location.RemoveUnsupportedPossibilitiesFor(null, Direction.Right);
                }
                else if (row[x + 1].HasCollapsed)
                {
                    location.RemoveUnsupportedPossibilitiesFor(row[x + 1].possibleTiles[0], Direction.Right);
                }
            }

            // Check Bottom
            {
                if (y == 0)
                {
                    location.RemoveUnsupportedPossibilitiesFor(null, Direction.Bottom);
                }
                else if (solution[y - 1][x].HasCollapsed)
                {
                    location.RemoveUnsupportedPossibilitiesFor(solution[y - 1][x].possibleTiles[0], Direction.Bottom);
                }
            }

            // Check Top
            {
                if (y == solution.Count - 1)
                {
                    location.RemoveUnsupportedPossibilitiesFor(null, Direction.Top);
                }
                else if (solution[y + 1][x].HasCollapsed)
                {
                    location.RemoveUnsupportedPossibilitiesFor(solution[y + 1][x].possibleTiles[0], Direction.Top);
                }
            }
        }

        private void UpdateNeighbours(int x, int y, Tile collapsedTile)
        {
            // IMPROVEMENT: Allow holes - not necessarily all neighbours need a gap.  Or make a null tile which you have to specify rules for too?

            if (x != 0)
            {
                solution[y][x - 1].RemoveUnsupportedPossibilitiesFor(collapsedTile, Direction.Right);
            }

            if (x < columns - 1)
            {
                solution[y][x + 1].RemoveUnsupportedPossibilitiesFor(collapsedTile, Direction.Left);
            }

            if (y != 0)
            {
                solution[y - 1][x].RemoveUnsupportedPossibilitiesFor(collapsedTile, Direction.Top);
            }

            if (y < rows - 1)
            {
                solution[y + 1][x].RemoveUnsupportedPossibilitiesFor(collapsedTile, Direction.Bottom);
            }
        }

        private bool DoAllNeighboursHavePossibilities(int x, int y)
        {
            if (x != 0 && !solution[y][x - 1].HasPossibilities)
            {
                return false;
            }

            if (x < columns - 1 && !solution[y][x + 1].HasPossibilities)
            {
                return false;
            }

            if (y != 0 && !solution[y - 1][x].HasPossibilities)
            {
                return false;
            }

            if (y < rows - 1 && !solution[y + 1][x].HasPossibilities)
            {
                return false;
            }

            return true;
        }

        private void AddUncollapsedNeighbours(int x, int y, HashSet<int> collapsedNeighbours, Queue<Vector2> tilesToCollapse)
        {
            if (x != 0 && !collapsedNeighbours.Contains(y * columns + x - 1))
            {
                tilesToCollapse.Enqueue(new Vector2(x - 1, y));
            }

            if (x < columns - 1 && !collapsedNeighbours.Contains(y * columns + x + 1))
            {
                tilesToCollapse.Enqueue(new Vector2(x + 1, y));
            }

            if (y != 0 && !collapsedNeighbours.Contains((y - 1) * columns + x))
            {
                tilesToCollapse.Enqueue(new Vector2(x, y - 1));
            }

            if (y < rows - 1 && !collapsedNeighbours.Contains((y + 1) * columns + x))
            {
                tilesToCollapse.Enqueue(new Vector2(x, y + 1));
            }
        }

        #endregion
    }
}