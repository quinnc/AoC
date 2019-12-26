using IntCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

    class Game : IThreadible
    {
        string code;
        ConcurrentDictionary<Location, GamePiece> board = new ConcurrentDictionary<Location, GamePiece>();
        ulong score = 0;
        BlockingCollection<string> gameOutput = new BlockingCollection<string>();
        private IntCode.ParallelCodeRunner prog;
        private bool _stop = false;
        private Threadify threadThis = new Threadify();

        
        public Game(string _code)
        {
            code = _code;
            score = 0;
        }

        public void Play()
        {
            prog = new IntCode.ParallelCodeRunner();

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
                    board[l] = (GamePiece)tile;
                }
            }

            // everything is put on the board
        }

        //  threaded implementation sot hat I can test my draw routine
        public void PlayThread()
        {
            prog = new IntCode.ParallelCodeRunner();

            threadThis.Instance = this;
            threadThis.RunInThread();

            prog = new IntCode.ParallelCodeRunner();

            prog.Code = code;
            prog.ExternalOutput = gameOutput;

            prog.RunInThread();
        }

        public void PlayUnlimited()
        {
            prog = new IntCode.ParallelCodeRunner();

            //start listener thread
            threadThis.Instance = this;
            threadThis.RunInThread();

            // set the program to play infinitely
            char[] array = code.ToCharArray();
            array[1] = '2';
            code = new string(array);

            prog.Code = code;
            prog.ExternalOutput = gameOutput;

            // start code
            prog.RunInThread();
        }

        public int Count(GamePiece tileType)
        {
            return board.Count(x => x.Value == tileType);
        }

        public void LeftPressed()
        {
            gameOutput.Add(((int)Move.Left).ToString());
        }

        public void StayPressed()
        {
            gameOutput.Add(((int)Move.Stay).ToString());
        }

        public void RightPressed()
        {
            gameOutput.Add(((int)Move.Right).ToString());
        }

        public bool Stop()
        {
            this._stop = true;
            return prog.EndThread() && threadThis.ThreadedResult();
        }

        public override string ToString()
        {
            List<List<char>> screen = new List<List<char>>();
            foreach (var tile in board)
            {
                if (screen[tile.Key.y] == null)
                    screen[tile.Key.y] = new List<char>();

                switch (tile.Value)
                {
                    case GamePiece.Ball:
                        screen[tile.Key.y][tile.Key.x] = 'o';
                        break;

                    case GamePiece.Block:
                        screen[tile.Key.y][tile.Key.x] = '#';
                        break;

                    case GamePiece.Paddle:
                        screen[tile.Key.y][tile.Key.x] = '-';
                        break;

                    case GamePiece.Tile:
                        screen[tile.Key.y][tile.Key.x] = ' ';
                        break;

                    case GamePiece.Wall:
                        screen[tile.Key.y][tile.Key.x] = '|';
                        break;
                }
            }

            int maxrow = screen.Count;
            int maxcol = 0;

            foreach (var row in screen)
            {
                if (row.Count > maxcol)
                    maxcol = row.Count;
            }

            string gamescreen = "";

            for (int i = maxrow - 1; i >= 0; i--)
            {
                string screenline = "";
                for (int j = 0; j < maxcol; j++)
                {
                    if (screen[i] == null)
                        break;

                    if (j > screen[i].Count)
                    {
                        screenline += ' ';
                        continue;
                    }
                    else
                    {
                        screenline += screen[i][j];
                    }
                }

                gamescreen += Environment.NewLine + screenline;

            }

            return gamescreen;
        }

        bool IThreadible.StartMember()
        {
            int mode = 0;
            string xStr = prog.ExternalInput.Take();
            string yStr = prog.ExternalInput.Take();
            string tileStr = prog.ExternalInput.Take();

            int x, y, tile;

            bool ok = false;

            while (!_stop)
            {
                x = -100;
                y = -100;
                tile = -100;

                switch (mode)
                {
                    case 0:
                    {
                        ok = Int32.TryParse(xStr, out x);
                        if (!ok)
                            Debugger.Break();
                        mode = 1;
                    }
                    break;

                    case 1:
                    {
                        ok = Int32.TryParse(yStr, out y);
                        if (!ok)
                            Debugger.Break();
                        mode = 2;
                    }
                    break;

                    case 2:
                    {
                        ok = Int32.TryParse(tileStr, out tile);
                        if (!ok)
                            Debugger.Break();
                        mode = 0;
                    }
                    break;
                }

                if (x == -100 || y == -100 || tile == -100)
                {
                    Debugger.Break();
                    return false;
                }

                // then just got the third value, so interpret the triad
                if (mode == 0)
                {
                    if (x == -1 && y == 0)
                    {
                        // score not location
                        score = (ulong)tile;
                    }
                    else
                    {
                        Location l = new Location(x, y);
                        board[l] = (GamePiece)tile;
                    }
                }

                Thread.Yield();
                Thread.Sleep(0);
            }

            return ok;
        }

        public string Score => score.ToString();
    }
}
