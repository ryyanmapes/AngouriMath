﻿/* 
 * Copyright (c) 2019-2021 Angouri.
 * AngouriMath is licensed under MIT. 
 * Details: https://github.com/asc-community/AngouriMath/blob/master/LICENSE.md.
 * Website: https://am.angouri.org.
 */
using AngouriMath.Core;
using AngouriMath.Core.Exceptions;
using PeterO.Numbers;
using FieldCacheNamespace;
using System.Linq;

namespace AngouriMath
{
    partial record Entity
    {
        partial record Number
        {
#pragma warning disable SealedOrAbstract // The only few exceptions: Complex, Real, Rational
            public partial record Complex : Number
#pragma warning restore SealedOrAbstract // AMAnalyzer
            {
                /// <summary>
                /// Constructor does not downcast automatically. Use <see cref="Create(Real, Real)"/> for automatic downcasting
                /// </summary>
                private protected Complex(Real? real, Real? imaginary) =>
                    (this.real, this.imaginary) = (real, imaginary);
                private readonly Real? real;
                private readonly Real? imaginary;

                /// <summary>
                /// Real part of a complex number
                /// </summary>
                public virtual Real RealPart => real ?? Integer.Zero;

                /// <summary>
                /// Imaginary part of a complex number
                /// </summary>
                public Real ImaginaryPart => imaginary ?? Integer.Zero;
                
                /// <summary>
                /// Conjugate of a complex number. Given this = a + ib, Conjugate = a - ib
                /// </summary>
                public Complex Conjugate => conjugate.GetValue(static @this => Create(@this.RealPart, -@this.ImaginaryPart), this);
                private FieldCache<Complex> conjugate;

                internal override Priority Priority =>
                    (RealPart, ImaginaryPart) switch
                    {
                        ({ IsZero: false }, { IsZero: false }) => Priority.Sum,
                        ({ IsZero: true }, Integer(1)) => Priority.Leaf,
                        _ => Priority.Mul
                    };

                /// <summary>
                /// An imaginary one. You can use it to avoid allocations
                /// </summary>
                [ConstantField] public static readonly Complex ImaginaryOne = new Complex(0, 1);

                /// <summary>
                /// An imaginary minus one. You can use it to avoid allocations
                /// </summary>
                [ConstantField] public static readonly Complex MinusImaginaryOne = new Complex(0, -1);

                /// <inheritdoc/>
                protected override bool ThisIsFinite => RealPart.EDecimal.IsFinite && ImaginaryPart.EDecimal.IsFinite;
                /// <inheritdoc/>
                public override bool IsExact => RealPart.IsExact && ImaginaryPart.IsExact;

                /// <summary>
                /// Checks if both parts equal 0
                /// </summary>
                public new bool IsZero => RealPart.EDecimal.IsZero && ImaginaryPart.EDecimal.IsZero;

                /// <summary>
                /// Checks whether the given number is undefined
                /// </summary>
                public bool IsNaN => this == Real.NaN;

                /// <summary>
                /// Creates an instance of Complex
                /// </summary>
                public static Complex Create(Real real, Real imaginary) =>
                    Create(real.EDecimal, imaginary.EDecimal);

                /// <summary>
                /// Creates an instance of Complex
                /// </summary>
                public static Complex Create(EDecimal real, EDecimal imaginary)
                {
                    if (!MathS.Settings.DowncastingEnabled)
                        return new Complex(real, imaginary);
                    if (real.IsNaN() || imaginary.IsNaN())
                        return Real.NaN;
                    if (imaginary.IsFinite && imaginary.Abs().LessThan(MathS.Settings.PrecisionErrorZeroRange))
                        return Real.Create(real);
                    else
                        return new Complex(Real.Create(real), Real.Create(imaginary));
                }

                /// <summary>
                /// Deconstructs as record
                /// </summary>
                public void Deconstruct(out Real realPart, out Real imaginaryPart) =>
                    (realPart, imaginaryPart) = (RealPart, ImaginaryPart);

                /// <summary>The magnitude of this <see cref="Complex"/>.</summary>
                /// <returns>
                /// Returns the absolute value of this complex number, to be precise,
                /// if this = a + ib, this.Abs() -> sqrt(a^2 + b^2)
                /// </returns>
                public new virtual Real Abs() =>
                    (Real)Sqrt(RealPart.EDecimal * RealPart.EDecimal + ImaginaryPart.EDecimal * ImaginaryPart.EDecimal);

                /// <summary>
                /// The phase of a complex number (aka angle)
                /// </summary>
                public Real Phase() => ImaginaryPart.EDecimal.Arctan2(RealPart.EDecimal, MathS.Settings.DecimalPrecisionContext);

                /// <summary>
                /// Creates a normal complex from its polar representation
                /// </summary>
                public static Complex CreatePolar(EDecimal magnitude, EDecimal phase)
                {
                    var context = MathS.Settings.DecimalPrecisionContext;
                    return Create(magnitude.Multiply(phase.Cos(context), context), magnitude.Multiply(phase.Sin(context), context));
                }

                /// <summary>-oo + -ooi</summary>
                [ConstantField] public static readonly Complex NegNegInfinity =
                    new Complex(Real.NegativeInfinity, Real.NegativeInfinity);

                /// <summary>-oo + +ooi</summary>
                [ConstantField] public static readonly Complex NegPosInfinity =
                    new Complex(Real.NegativeInfinity, Real.PositiveInfinity);

                /// <summary>+oo + -ooi</summary>
                [ConstantField] public static readonly Complex PosNegInfinity =
                    new Complex(Real.PositiveInfinity, Real.NegativeInfinity);

                /// <summary>+oo + +ooi</summary>
                [ConstantField] public static readonly Complex PosPosInfinity =
                    new Complex(Real.PositiveInfinity, Real.PositiveInfinity);

                /// <summary>Parses a <see cref="string"/> into <see cref="Complex"/></summary>
                /// <returns><see cref="Complex"/></returns>
                /// <exception cref="ParseException">Thrown when <paramref name="source"/> cannot be parsed.</exception>
                public static Complex Parse(string source) =>
                    TryParse(source, out var res) ? res : throw new CannotParseInstanceException(typeof(Complex), source);

                /// <summary>Tries to parse a <see cref="Complex"/> from <see cref="string"/></summary>
                /// <param name="source"></param>
                /// <param name="dst">
                /// The result will be written to this variable only if parsing was successful,
                /// if it was not, do not access this variable
                /// </param>
                /// <returns>If parsing was successful - <see langword="true"/>, <see langword="false"/> otherwise</returns>
                public static bool TryParse(string source,
                    [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Complex? dst)
                {
                    dst = null;
                    if (string.IsNullOrEmpty(source))
                        return false;
                    if (Real.TryParse(source, out var real))
                    {
                        dst = real;
                        return true;
                    }
                    if (source.Last() == 'i')
                    {
                        if (source == "i")
                        {
                            dst = ImaginaryOne;
                            return true;
                        }
                        else if (source == "-i")
                        {
                            dst = MinusImaginaryOne;
                            return true;
                        }
                        else if (Real.TryParse(source.Substring(0, source.Length - 1), out var realPart))
                        {
                            dst = new Complex(0, realPart);
                            return true;
                        }
                        else
                            return false;
                    }
                    return false;
                }

                /// <summary>
                /// Converts the Complex to its of the system module Numerics
                /// </summary>
                public System.Numerics.Complex ToNumerics() =>
                    new System.Numerics.Complex(RealPart.EDecimal.ToDouble(), ImaginaryPart.EDecimal.ToDouble());

#pragma warning disable CS1591
                public static explicit operator System.Numerics.Complex(Complex it)
                    => it.ToNumerics();

                public static Complex operator +(Complex a, Complex b) => OpSum(a, b);
                public static Complex operator -(Complex a, Complex b) => OpSub(a, b);
                public static Complex operator *(Complex a, Complex b) => OpMul(a, b);
                public static Complex operator /(Complex a, Complex b) => OpDiv(a, b);
                public static Complex operator +(Complex a) => a;
                public static Complex operator -(Complex a) => OpMul(Integer.MinusOne, a);
                public static implicit operator Complex(sbyte value) => Integer.Create(value);
                public static implicit operator Complex(byte value) => Integer.Create(value);
                public static implicit operator Complex(short value) => Integer.Create(value);
                public static implicit operator Complex(ushort value) => Integer.Create(value);
                public static implicit operator Complex(int value) => Integer.Create(value);
                public static implicit operator Complex(uint value) => Integer.Create(value);
                public static implicit operator Complex(long value) => Integer.Create(value);
                public static implicit operator Complex(ulong value) => Integer.Create(value);
                public static implicit operator Complex(EInteger value) => Integer.Create(value);
                public static implicit operator Complex(ERational value) => Rational.Create(value);
                public static implicit operator Complex(EDecimal value) => Real.Create(value);
                public static implicit operator Complex(float value) => Real.Create(EDecimal.FromSingle(value));
                public static implicit operator Complex(double value) => Real.Create(EDecimal.FromDouble(value));
                public static implicit operator Complex(decimal value) => Real.Create(EDecimal.FromDecimal(value));
                public static implicit operator Complex(System.Numerics.Complex value) =>
                    Create(EDecimal.FromDouble(value.Real), EDecimal.FromDouble(value.Imaginary));
                public static implicit operator Complex((int re, int im) v) => Complex.Create(v.re, v.im);
                public static implicit operator Complex((float re, float im) v) => Complex.Create(v.re, v.im);
                public static implicit operator Complex((decimal re, decimal im) v) => Complex.Create(v.re, v.im);
                public static implicit operator Complex((double re, double im) v) => Complex.Create(v.re, v.im);

#pragma warning restore CS1591

            }
        }
    }
}
