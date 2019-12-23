using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using TwodTypes;

namespace day13_pong
{
    public enum GamePiece
    {

        Tile = 0, //0 is an empty tile. No game object appears in this tile.
        Wall = 1, //1 is a wall tile. Walls are indestructible barriers.
        Block = 2, //2 is a block tile. Blocks can be broken by the ball.
        Paddle = 3, //3 is a horizontal paddle tile. The paddle is indestructible.
        Ball = 4 //4 is a ball tile. The ball moves diagonally and bounces off objects.

    }

    class Game
    {
        readonly string code;
        Dictionary<Location, GamePiece> board = new Dictionary<Location, GamePiece>();

        public Game(string _code)
        {
            code = _code;
        }

        public void Play()
        {
            IntCode.ParallelCodeRunner prog = new IntCode.ParallelCodeRunner();

            prog.Code = code;

            prog.ExternalOutput = prog.ExternalInput;

            // run it to completion, then look at the output
            prog.Run();

            if (prog.ExternalInput.Count % 3 != 0)
            {
                MessageBox.Show("Not a multip[le of 3: " + prog.ExternalInput.Count.ToString());
                Debugger.Break();
            }

            int totalCmds = prog.ExternalInput.Count / 3;
            int initCnt = prog.ExternalInput.Count;

            for (int i = 0; i < initCnt && prog.ExternalInput.Count > 0; i += 3)
            {
                string xStr = prog.ExternalInput.Take();
                string yStr = prog.ExternalInput.Take();
                string tileStr = prog.ExternalInput.Take();

                int x, y, tile;

                bool ok = false;
                ok = Int32.TryParse(xStr, out x);
                if (!ok)
                    Debugger.Break();

                ok = Int32.TryParse(yStr, out y);
                if (!ok)
                    Debugger.Break();

                ok = Int32.TryParse(tileStr, out tile);
                if (!ok)
                    Debugger.Break();

                Location l = new Location(x, y);

                board[l] = (GamePiece)(tile);

            }

            // everything is put on the board

        }

        public int Count(GamePiece tileType)
        {
            return board.Count(x => x.Value == tileType);
        }
    }
}
