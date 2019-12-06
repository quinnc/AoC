using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Day4.Calculators
{
    class PasswordGuesser
    {
        public void Guess(int bottom, int top)
        {
            Parallel.For(bottom, top + 1,
                currGuess =>
                {
                    string guessAsStr = currGuess.ToString();
                    if (IsIncreasing(guessAsStr) && HasDouble(guessAsStr))
                    {
                        matches.Add(currGuess);
                    }
                });

        }

        private bool HasDouble(string currGuess)
        {
            for (int i = 1; i < currGuess.Length; i++)
            {
                if (currGuess[i - 1] == currGuess[i])
                    return true;
            }

            return false;
        }

        private bool IsIncreasing(string currGuess)
        {
            for (int i = 1; i < currGuess.Length; i++)
            {
                // if the current one is less than the previous, then fail
                if (currGuess[i] < currGuess[i - 1])
                    return false;
            }

            return true;
        }

        List<int> matches = new List<int>();

        public IReadOnlyList<int> Matches => matches;

        public override string ToString()
        {
            string s = "";

            foreach (int m in matches)
            {
                s = m.ToString() + Environment.NewLine;
            }

            return s;

        }
    }
}
