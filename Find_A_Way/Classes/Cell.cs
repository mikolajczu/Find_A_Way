using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Find_A_Way.Classes
{
    public enum State
    {
        START,
        FINISH,
        WALL,
        NOTVISITED
    }

    internal class Cell
    {
        public State state;
        public bool visited;
        public float globalDistance;
        public float localDistance;
        public int x, y;
        public List<Cell> neighbors;
        public Cell? parent;

        public Cell(int x, int y)
        {
            state = State.NOTVISITED;
            visited = false;
            globalDistance = float.PositiveInfinity;
            localDistance = float.PositiveInfinity;
            this.x = x;
            this.y = y;
            neighbors = new List<Cell>();
            parent = null;
        }
    }
}
