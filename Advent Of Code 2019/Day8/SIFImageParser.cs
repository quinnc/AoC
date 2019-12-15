using System;
using System.Collections.Generic;

namespace Day8
{
    internal class SIFImageParser
    {
        private string text;
        private readonly List<string> layers = new List<string>();

        public SIFImageParser(string text)
        {
            this.text = text;
        }

        internal void Decode()
        {
            int skip = 25 * 6;
            for (int i = 0; i < text.Length; i += skip)
            {
                layers.Add(text.Substring(i, skip));
            }

        }

        internal int Checkcode()
        {
            int maxZeros = 25 * 6 + 1; ;
            int maxZerosIndex = 0;

            //foreach (var l in layers)
            for (int layerIdx = 0; layerIdx < layers.Count; layerIdx++)
            {
                int currZeros = 0;

                foreach (var c in layers[layerIdx])
                {
                    if (c == '0')
                        currZeros++;
                }

                if (currZeros <= maxZeros)
                {
                    maxZerosIndex = layerIdx;
                }

            }

            int currOnes = 0;
            int currTwos = 0;

            foreach (var c in layers[maxZerosIndex])
            {
                if (c == '1')
                    currOnes++;
                else if (c == '2')
                    currTwos++;
            }

            return currOnes * currTwos;
        }
    }
}