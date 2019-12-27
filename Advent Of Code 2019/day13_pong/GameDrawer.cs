using IntCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace day13_pong
{

    class GameDraw : IThreadible
    {

        private TextBox screen, scorebox;
        private Game _game;
        bool stop = false;

        public delegate void SetTextCallback(string str, string scoreStr);
        SetTextCallback screenCb; 

        public bool Stop()
        {
            stop = true;
            return threadThis.ThreadedResult();
        }

        public GameDraw(ref TextBox output, ref TextBox score, ref Game game)
        {
            screen = output;
            scorebox = score;
            _game = game;
            screenCb = new SetTextCallback(SetScore);
        }

        private void SetScore( string str, string scoreStr)
        {
            screen.Text = str;
            if (scoreStr != "0")
                scorebox.Text = scoreStr;

        }
        private void SetScreen (string str)
        {
            screen.Text = str;
        }

        public int Joystick { get; set; } = -100;

        Threadify threadThis = new Threadify();

        bool IThreadible.StartMember()
        {
            while (!stop)
            {
                Thread.Sleep(1);

                string t = _game.ToString();
                string s = _game.Score;

                screen.Dispatcher.BeginInvoke(screenCb, new object []{ t, s });
            }
            return true;
        }

        public void Start()
        {
            threadThis.Instance = this;
            threadThis.RunInThread();
        }
    }
}
