using System;
using System.IO;

namespace client
{
    class Map
    {
        string[,] map;
        int width;
        int height;
        int x0;
        int y0;

        public Map(string path)
        {
            string[] lines = File.ReadAllLines(path);

            width = lines[0].Length;
            height = lines.Length;

            map = new string[height, width];

            string currentLine = string.Empty;

            for (int i = 0; i < height; i++)
            {
                currentLine = lines[i];

                for (int j = 0; j < width; j++)
                {
                    //map[i, j] = currentLine[j].ToString();

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

            x0 = (Console.WindowWidth - width) / 2;
            y0 = (Console.WindowHeight - height) / 2;

            // 17 pixels from right terminal is ignore
        }

        public int X0 => x0;
        public int Y0 => y0;

        public bool isPosWalkable(int x, int y)
        {
            x += 1;
            y += 1;

            if (x <= 0 || y <= 0 || x >= width || y >= height)
                return false;

            return (map[y, x] == " ");
        }

        public void Draw()
        {
            Console.Clear();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.SetCursorPosition(x0 + j, y0 + i);
                    Console.Write(map[i, j]);
                }
            }

            //Console.ReadKey();
        }
    }
}