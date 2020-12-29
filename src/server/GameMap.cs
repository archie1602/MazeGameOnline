using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace server
{
    class GameMap
    {
        string[,] map;
        (int, int) size; // width and height

        public (int, int) Size => size;

        public GameMap(string path)
        {
            string[] lines = File.ReadAllLines(path);

            int width = lines[0].Length;
            int height = lines.Length;

            size = (width, height);

            map = new string[height, width];

            string currentLine = string.Empty;

            for (int i = 0; i < height; i++)
            {
                currentLine = lines[i];

                for (int j = 0; j < width; j++)
                {
                    char c = currentLine[j];

                    if (c == '-')
                        map[i, j] = "═";
                    else if (c == '|')
                        map[i, j] = "║";
                    else if (c == '+')
                    {
                        // Need to understand which one

                        if (i == 0 && j == 0)
                            map[i, j] = "╔";
                        else if (i == 0 && j == width - 1)
                            map[i, j] = "╗";
                        else if (i == height - 1 && j == 0)
                            map[i, j] = "╚";
                        else if (i == height - 1 && j == width - 1)
                            map[i, j] = "╝";
                        else if (lines[i][j + 1] == '-' && lines[i + 1][j] == '|')
                            map[i, j] = "╔";
                        else if (lines[i][j - 1] == '-' && lines[i + 1][j] == '|')
                            map[i, j] = "╗";
                        else if (lines[i][j + 1] == '-' && lines[i - 1][j] == '|')
                            map[i, j] = "╚";
                        else if (lines[i][j - 1] == '-' && lines[i - 1][j] == '|')
                            map[i, j] = "╝";
                    }
                    else
                        map[i, j] = " ";
                }
            }
        }

        public bool isPositionWalkable(int x, int y)
        {
            x += 1;
            y += 1;

            if (x <= 0 || y <= 0 || x >= size.Item1 || y >= size.Item2)
                return false;

            return (map[y, x] == " ");
        }
    }
}