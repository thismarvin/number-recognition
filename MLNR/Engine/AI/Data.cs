﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNR.Engine.AI
{
    class Data
    {
        public double[] Input { get; private set; }
        public double[] Output { get; private set; }

        public Data(double[] input, double[] output)
        {
            Input = input;
            Output = output;
        }
    }
}
