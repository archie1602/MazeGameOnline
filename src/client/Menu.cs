using System;
using Figgle;
using Pastel;

namespace client
{
    class Menu
    {
        string[] options;
        int selectedIndex;
        int startPositionY;
        Type type;
        // console width
        int cw = Console.WindowWidth;

        // console height
        int ch = Console.WindowHeight;

        public enum Type
        {
            Main,
            CreateRoom,
            EnterRoom,
            WaitingRoomVisitor,
            WaitingRoomOwner
        }

        public Menu(string[] _options, Type _type, int _startPositionY)
        {
            options = _options;
            type = _type;
            selectedIndex = 0;
            startPositionY = _startPositionY;
        }

        void DisplayLabel((int, int) labelPosition, string labelName, bool isSelected = false)
        {
            Console.SetCursorPosition(labelPosition.Item1, labelPosition.Item2);

            if (isSelected)
                Console.BackgroundColor = ConsoleColor.Red;

            Console.Write(labelName);
            Console.ResetColor();
        }

        void EraseLabel((int, int) labelPosition, int labelSize)
        {
            Console.SetCursorPosition(labelPosition.Item1, labelPosition.Item2);

            for (int i = 0; i < labelSize; i++)
                Console.Write(" ");
        }

        void Display()
        {
            int padding = 2;

            if ((type == Type.Main) || (type == Type.CreateRoom) || (type == Type.EnterRoom))
            {
                for (int i = 0; i < options.Length; i++)
                    DisplayLabel((((cw - options[i].Length) / 2), startPositionY + padding * i), options[i], i == selectedIndex);
            }
            else if (type == Type.WaitingRoomVisitor)
            {
                DisplayLabel((0, startPositionY), options[0], true);
            }
            else if (type == Type.WaitingRoomOwner)
            {
                DisplayLabel((0, startPositionY), options[0], true);
                DisplayLabel((cw - options[1].Length, startPositionY), options[1]);
            }
        }

        public (int selectedIndex, char marker, string color, string ip, int port, int maxPlayers, string roomCode)? Run()
        {
            Display();

            if (type == Type.Main)
            {
                ConsoleKey keyPressed;

                int padding = 2;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    keyPressed = keyInfo.Key;

                    int prevIndex = selectedIndex;

                    if (keyPressed == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    else if (keyPressed == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % options.Length;

                    if (prevIndex != selectedIndex)
                    {
                        (int, int) prevLabelPosition = (((cw - options[prevIndex].Length) / 2), startPositionY + padding * prevIndex);
                        (int, int) currentLabelPosition = (((cw - options[selectedIndex].Length) / 2), startPositionY + padding * selectedIndex);

                        EraseLabel(prevLabelPosition, options[prevIndex].Length);
                        DisplayLabel(prevLabelPosition, options[prevIndex]);
                        DisplayLabel(currentLabelPosition, options[selectedIndex], true);
                    }

                } while (keyPressed != ConsoleKey.Enter);

                return (selectedIndex, '\0', null, null, 0, 0, null);
            }
            else if (type == Type.CreateRoom)
            {
                string ip = null;
                string color = null;
                char marker = '\0';
                int port = 11000;
                int maxPlayers = 1;

                ConsoleKey keyPressed;

                int padding = 2;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    keyPressed = keyInfo.Key;

                    int prevIndex = selectedIndex;

                    if (keyPressed == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    else if (keyPressed == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % options.Length;
                    else if (keyPressed == ConsoleKey.Enter)
                    {
                        if ((options.Length - selectedIndex) > 2)
                        {
                            Console.Write(" ");
                            Console.CursorVisible = true;
                            (int, int) shift = (((cw - options[selectedIndex].Length) / 2) + options[selectedIndex].Length + 1, startPositionY + padding * selectedIndex);

                            Console.SetCursorPosition(shift.Item1, shift.Item2);
                            EraseLabel(shift, cw - shift.Item1);

                            Console.SetCursorPosition(shift.Item1, shift.Item2);

                            if (selectedIndex == 0)
                                marker = Char.Parse(Console.ReadLine());
                            else if (selectedIndex == 1)
                                color = Console.ReadLine();
                            else if (selectedIndex == 2)
                                ip = Console.ReadLine();
                            else if (selectedIndex == 3)
                                port = Int32.Parse(Console.ReadLine());
                            else if (selectedIndex == 4)
                                maxPlayers = Int32.Parse(Console.ReadLine());

                            Console.CursorVisible = false;
                        }
                    }

                    if (prevIndex != selectedIndex)
                    {
                        (int, int) prevLabelPosition = (((cw - options[prevIndex].Length) / 2), startPositionY + padding * prevIndex);
                        (int, int) currentLabelPosition = (((cw - options[selectedIndex].Length) / 2), startPositionY + padding * selectedIndex);

                        EraseLabel(prevLabelPosition, options[prevIndex].Length);
                        DisplayLabel(prevLabelPosition, options[prevIndex]);
                        DisplayLabel(currentLabelPosition, options[selectedIndex], true);
                    }

                } while (keyPressed != ConsoleKey.Enter || ((options.Length - selectedIndex) > 2));

                return (selectedIndex, marker, color, ip, port, maxPlayers, null);
            }
            else if (type == Type.EnterRoom)
            {
                string ip = null;
                string color = null;
                string roomCode = null;
                char marker = '\0';
                int port = 11000;

                ConsoleKey keyPressed;

                int padding = 2;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    keyPressed = keyInfo.Key;

                    int prevIndex = selectedIndex;

                    if (keyPressed == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    else if (keyPressed == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % options.Length;
                    else if (keyPressed == ConsoleKey.Enter)
                    {
                        if ((options.Length - selectedIndex) > 2)
                        {
                            Console.Write(" ");
                            Console.CursorVisible = true;
                            (int, int) shift = (((cw - options[selectedIndex].Length) / 2) + options[selectedIndex].Length + 1, startPositionY + padding * selectedIndex);

                            Console.SetCursorPosition(shift.Item1, shift.Item2);
                            EraseLabel(shift, cw - shift.Item1);

                            Console.SetCursorPosition(shift.Item1, shift.Item2);

                            if (selectedIndex == 0)
                                marker = Char.Parse(Console.ReadLine());
                            else if (selectedIndex == 1)
                                color = Console.ReadLine();
                            else if (selectedIndex == 2)
                                ip = Console.ReadLine();
                            else if (selectedIndex == 3)
                                port = Int32.Parse(Console.ReadLine());
                            else if (selectedIndex == 4)
                                roomCode = Console.ReadLine();

                            Console.CursorVisible = false;
                        }
                    }

                    if (prevIndex != selectedIndex)
                    {
                        (int, int) prevLabelPosition = (((cw - options[prevIndex].Length) / 2), startPositionY + padding * prevIndex);
                        (int, int) currentLabelPosition = (((cw - options[selectedIndex].Length) / 2), startPositionY + padding * selectedIndex);

                        EraseLabel(prevLabelPosition, options[prevIndex].Length);
                        DisplayLabel(prevLabelPosition, options[prevIndex]);
                        DisplayLabel(currentLabelPosition, options[selectedIndex], true);
                    }

                } while (keyPressed != ConsoleKey.Enter || ((options.Length - selectedIndex) > 2));

                return (selectedIndex, marker, color, ip, port, 0, roomCode);
            }
            else if (type == Type.WaitingRoomVisitor)
            {
                ConsoleKey keyPressed;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    keyPressed = keyInfo.Key;

                    (int, int) currentLabelPosition = (0, startPositionY);

                    DisplayLabel(currentLabelPosition, options[selectedIndex], true);

                } while (keyPressed != ConsoleKey.Enter);

                return (selectedIndex, '\0', null, null, 0, 0, null);
            }
            else if (type == Type.WaitingRoomOwner)
            {
                ConsoleKey keyPressed;

                do
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    keyPressed = keyInfo.Key;

                    int prevIndex = selectedIndex;

                    if (keyPressed == ConsoleKey.LeftArrow)
                        selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
                    else if (keyPressed == ConsoleKey.RightArrow)
                        selectedIndex = (selectedIndex + 1) % options.Length;

                    if (prevIndex != selectedIndex)
                    {
                        (int, int) prevLabelPosition = ((selectedIndex == 0) ? (cw - options[1].Length) : 0, startPositionY);
                        (int, int) currentLabelPosition = ((selectedIndex == 0) ? 0 : (cw - options[1].Length), startPositionY);

                        EraseLabel(prevLabelPosition, options[prevIndex].Length);
                        DisplayLabel(prevLabelPosition, options[prevIndex]);
                        DisplayLabel(currentLabelPosition, options[selectedIndex], true);
                    }

                } while (keyPressed != ConsoleKey.Enter);

                return (selectedIndex, '\0', null, null, 0, 0, null);
            }

            return null;
        }
    }
}