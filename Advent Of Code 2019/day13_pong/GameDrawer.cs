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

        public delegate void SetTextCallback(string str);
        SetTextCallback scoreCb, screenCb;


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
            scoreCb = new SetTextCallback(SetScore);
            screenCb = new SetTextCallback(SetScreen);
        }

        private void SetScore( string str)
        {
            scorebox.Text = str;
        }
        private void SetScreen (string str)
        {
            screen.Text = str;
        }

        Threadify threadThis = new Threadify();

        bool IThreadible.StartMember()
        {
            while (!stop)
            {
                Thread.Yield();

                string t = _game.ToString();
                string s = _game.Score;

                //screen.Text = t;
                //screen.Invoke((MethodInvoker)delegate {
                //    // Running on the UI thread
                //    screen.Text = t;
                //});
                //scorebox.Text = s;

                screen.Dispatcher.Invoke(screenCb, t);
                scorebox.Dispatcher.Invoke(scoreCb, s);
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
