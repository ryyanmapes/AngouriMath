﻿using System;
using AngouriMath;
using AngouriMath.Extensions;
using static AngouriMath.MathS;
using static AngouriMath.Entity;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using static AngouriMath.Entity.Set;
using AngouriMath.Core;
using System.Linq;

Matrix m = "[[a, b], [c, d]]";

Console.WriteLine(m.TensorPower(2).ToString(true));