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
        SetTextCallback screenCb; //  scoreCb, 


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
           // screenCb = new SetTextCallback(SetScreen);
        }

        private void SetScore( string str, string scoreStr)
        {
            screen.Text = str;
            scorebox.Text = scoreStr;

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
                Thread.Sleep(100);

                string t = _game.ToString();
                string s = _game.Score;

                screen.Dispatcher.Invoke(screenCb, new object []{ t, s });
                //scorebox.Dispatcher.Invoke(scoreCb, s);
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
