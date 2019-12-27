using IntCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            array[0] = '2';
            code = new string(array);
            //prog.InitRAM(0, 2);

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
            prog.ExternalInput.Add(((int)Move.Left).ToString());
        }

        public void StayPressed()
        {
            prog.ExternalInput.Add(((int)Move.Stay).ToString());
        }

        public void RightPressed()
        {
            prog.ExternalInput.Add(((int)Move.Right).ToString());
        }

        public bool Stop()
        {
            _stop = true;
            gameOutput.Add("game over");
            return prog.EndThread() && threadThis.ThreadedResult();
        }

        public override string ToString()
        {
            List<List<char>> screen = new List<List<char>>();
            foreach (var tile in board)
            {
                if (screen.Count <= tile.Key.y)
                {
                    for (int ynew = screen.Count; ynew <= tile.Key.y; ynew++)
                    {
                        screen.Add(new List<char>());
                    }
                }

                if (screen[tile.Key.y].Count <= tile.Key.x)
                {

                    for (int xnew = screen[tile.Key.y].Count; xnew <= tile.Key.x; xnew++)
                    {
                        screen[tile.Key.y].Add(' ');
                    }
                }

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

                    if (j >= screen[i].Count)
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
            string str;

            int x, y, tile;

            x = -100;
            y = -100;
            tile = -100;

            bool ok = false;

            int ballLastX = -1;
            int paddleLastX = -1;

            while (!_stop)
            {
                str = gameOutput.Take();
                switch (mode)
                {
                    case 0:
                    {
                        ok = Int32.TryParse(str, out x);
                        if (!ok)
                            break;
                        mode = 1;
                    }
                    break;

                    case 1:
                    {
                        ok = Int32.TryParse(str, out y);
                        if (!ok)
                            break;
                        mode = 2;
                    }
                    break;

                    case 2:
                    {
                        ok = Int32.TryParse(str, out tile);
                        if (!ok)
                            break;
                        mode = 0;
                    }
                    break;
                }

                if (!ok || (x == -100 && y == -100 && tile == -100))
                    break;

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
                        if (x >= 0 && y >= 0)
                        {
                            GamePiece tileGp = (GamePiece)tile;

                            Location l = new Location(x, y);
                            board[l] = tileGp;

                            if (tileGp == GamePiece.Ball)
                            {
                                ballLastX = x;
                            }
                            else if (tileGp == GamePiece.Paddle)
                            {

                                paddleLastX = x;
                            }

                            if ((paddleLastX != -1) && (ballLastX != -1))
                            {

                                // Empty the input queue in case the ball reversed direction on us
                                while (prog.ExternalInput.TryTake(out var _)) { }
                                if (ballLastX < paddleLastX)
                                {
                                    prog.ExternalInput.Add("-1");
                                }
                                else if (ballLastX == paddleLastX)
                                {
                                    prog.ExternalInput.Add("0");
                                }
                                else if (ballLastX > paddleLastX)
                                {
                                    prog.ExternalInput.Add("1");
                                }
                            }
                        }
                    }

                    x = -100;
                    y = -100;
                    tile = -100;
                }

                Thread.Yield();
                //Thread.Sleep(0);
            }

            return ok;
        }

        public string Score => score.ToString();
    }
}
