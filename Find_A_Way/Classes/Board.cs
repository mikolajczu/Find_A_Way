using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Find_A_Way.Classes
{
    internal class Board
    {
        public Cell[][] map;

        public Board(int y, int x)
        {
            map = new Cell[y][];
            for (int i = 0; i < y; i++)
                map[i] = new Cell[x];
            for(int i = 0; i < y; i++)
                for(int j = 0; j < x; j++)
                    map[i][j] = new Cell(j, i);
        }

        public void Clear()
        {
            for (int i = 0; i < map.Length; i++)
                for (int j = 0; j < map[i].Length; j++)
                    map[i][j] = new Cell(j, i);
        }
    }
}
