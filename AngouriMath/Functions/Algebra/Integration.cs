
/* Copyright (c) 2019-2020 Angourisoft
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */



﻿using AngouriMath.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngouriMath
{
    public abstract partial class Entity
    {
        /// <summary>
        /// Returns a value of a definite integral of a function. Only works for one-variable functions
        /// </summary>
        /// <param name="x">
        /// Variable to integrate over
        /// </param>
        /// <param name="from">
        /// The down bound for integrating
        /// </param>
        /// <param name="to">
        /// The up bound for integrating
        /// </param>
        /// <returns></returns>
        public Number DefiniteIntegral(VariableEntity x, Number from, Number to)
        {
            return Integration.Integrate(this, x, from, to, 100);
        }

        /// <summary>
        /// Returns a value of a definite integral of a function. Only works for one-variable functions
        /// </summary>
        /// <param name="x">
        /// Variable to integrate over
        /// </param>
        /// <param name="from">
        /// The down bound for integrating
        /// </param>
        /// <param name="to">
        /// The up bound for integrating
        /// </param>
        /// <param name="stepCount">
        /// Accuracy (initially, amount of iterations)
        /// </param>
        /// <returns></returns>
        public Number DefiniteIntegral(VariableEntity x, Number from, Number to, int stepCount)
        {
            return Integration.Integrate(this, x, from, to, stepCount);
        }
    }
    public static class Integration
    {
        public static Number Integrate(Entity func, VariableEntity x, Number from, Number to, int stepCount)
        {
            double ReFrom = from.Re;
            double ImFrom = from.Im;
            double ReTo = to.Re;
            double ImTo = to.Im;
            var res = new Number(0, 0);
            var cfunc = func.Compile(x);
            for(int i = 0; i <= stepCount; i++)
            {
                var share = ((double)i) / stepCount;
                var tmp = new Number(ReFrom * share + ReTo * (1 - share), ImFrom * share + ImTo * (1 - share));
                res += cfunc.Substitute(tmp);
            }
            return res / (stepCount + 1) * (to - from);
        }
    }
}
