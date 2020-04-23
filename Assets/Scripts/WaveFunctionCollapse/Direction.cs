using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.WaveFunctionCollapse
{
    public enum Direction
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public static class DirectionExtensions
    {
        public static string ToDisplayString(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return "Left";

                case Direction.Top:
                    return "Top";

                case Direction.Right:
                    return "Right";

                case Direction.Bottom:
                    return "Bottom";

                default:
                    return "";
            }
        }
    }
}
