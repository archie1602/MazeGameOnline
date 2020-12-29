using System;

namespace client
{
    class Player
    {
        (int, int) currPlayerPos;
        (int, int) prevPlayerPos;
        (int, int) mapPos;
        char marker;
        ConsoleColor color;

        public (int, int) CurrPlayerPos
        {
            get { return currPlayerPos; }
            set
            {
                prevPlayerPos = currPlayerPos;
                currPlayerPos = value;
            }
        }

        public Player((int, int) _currPlayerPos, (int, int) _mapPos, char _marker, ConsoleColor _color)
        {
            currPlayerPos = _currPlayerPos;
            prevPlayerPos = _currPlayerPos;
            mapPos = _mapPos;

            marker = _marker;
            color = _color;
        }

        public void Draw()
        {
            int shiftX = mapPos.Item1 + 1;
            int shiftY = mapPos.Item2 + 1;

            Console.ResetColor();

            // Erase player from old position
            Console.SetCursorPosition(shiftX + prevPlayerPos.Item1, shiftY + prevPlayerPos.Item2);
            Console.Write(" ");

            Console.ForegroundColor = color;

            // Draw player in a new position
            Console.SetCursorPosition(shiftX + currPlayerPos.Item1, shiftY + currPlayerPos.Item2);
            Console.Write(marker);
        }

    }
}