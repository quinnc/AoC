using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public enum Move
    {
        Left = -1,
        Stay = 0,
        Right = 1,
    }

    class Game : INotifyPropertyChanged
    {
        string code;
        Dictionary<Location, GamePiece> board = new Dictionary<Location, GamePiece>();
        ulong score = 0;
        BlockingCollection<string> gameOutput = new BlockingCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Game(string _code)
        {
            code = _code;
            score = 0;
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
                MessageBox.Show("Not a multiple of 3: " + prog.ExternalInput.Count.ToString());
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

                if (x == -1 && y == 0)
                {
                    // score not location
                    score = (ulong)tile;
                }
                else
                {
                    Location l = new Location(x, y);

                    board[l] = (GamePiece)(tile);
                }

            }

            // everything is put on the board

        }


        public void PlayUnlimited()
        {

            //start listener thread

            // start code
            IntCode.ParallelCodeRunner prog = new IntCode.ParallelCodeRunner();

            // set the program to play infinitely
            char[] array = code.ToCharArray();
            array[1] = '2';
            code = new string(array);

            prog.Code = code;
            
            prog.ExternalOutput = gameOutput;

            // run it to completion, then look at the output
            prog.Run();

        }

        public int Count(GamePiece tileType)
        {
            return board.Count(x => x.Value == tileType);
        }
    }
}
