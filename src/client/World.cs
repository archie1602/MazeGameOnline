using System;

namespace client
{
    class World
    {
        // xWorldStart
        int xws;

        // yWorldStart
        int yws;
        
        public World(int _xws, int _yws)
        {
            xws = _xws;
            yws = _yws;
        }

        /*
        // Maze map
        string[,] map;

        // Maze width
        int width;

        // Maze height
        int height;

        // Constructor for building the world
        public World(string[,] _map)
        {
            map = _map;
            height = map.GetLength(0);
            width = map.GetLength(1);
        }

        // Method for drawing world
        public void Draw()
        {
            // Console width
            int wWidth = Console.WindowWidth;

            // Console heiht
            int wHeight = Console.WindowHeight;

            // Coordintaes of the new axis OX'Y'
            int x = (wWidth - width) / 2;
            int y = (wHeight - height) / 2;

            // Draw a map
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.SetCursorPosition(x + i, y + j);
                    Console.Write(map[i, j]);
                }
            }

        }
        */
    }
}