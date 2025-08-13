using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace CoreFramework.Mathematics
{
    /// <summary>
    /// A custom struct representing a pair of 64-bit unsigned integers.
    /// Designed to mirror Unity's int2/uint2 structs.
    /// </summary>
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [Serializable]
    [BurstCompile]
    public struct ulong2 : IEquatable<ulong2>, IFormattable
    {
        public ulong x;
        public ulong y;

        public static readonly ulong2 zero = new ulong2(0, 0);

        #region Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(ulong x, ulong y) => (this.x, this.y) = (x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(long x, long y) => (this.x, this.y) = ((ulong)x, (ulong)y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(ulong2 xy) => (x, y) = (xy.x, xy.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(uint2 v) => (x, y) = (v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(int2 v) => (x, y) = ((ulong)v.x, (ulong)v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(float2 v) => (x, y) = ((ulong)v.x, (ulong)v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(double2 v) => (x, y) = ((ulong)v.x, (ulong)v.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(bool2 v)=> (x, y) = (v.x ? 1u : 0u, v.y ? 1u : 0u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(ulong v) => (x, y) = (v, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(long v) => (x, y) = ((ulong)v, (ulong)v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(uint v) => (x, y) = (v, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(int v) => (x, y) = ((ulong)v, (ulong)v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(float v) => (x, y) = ((ulong)v, (ulong)v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(double v) => (x, y) = ((ulong)v, (ulong)v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong2(bool v) => (x, y) = (v ? 1u : 0u, v ? 1u : 0u);

        #endregion
        
        #region Implicit Conversions From Unity Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(uint2 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(int2 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(float2 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(double2 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(bool2 v) => new(v);

        #endregion
        
        #region Implicit Scalar Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2((ulong x, ulong y) v) => new(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator (ulong, ulong)(ulong2 v) => (v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(ulong v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(long v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(uint v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(int v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(float v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(double v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong2(bool v) => new(v);

        #endregion
        
        #region Conversions

        // To scalar types (takes x component)
        public static explicit operator uint(ulong2 v) => (uint)v.x;
        public static explicit operator ulong(ulong2 v) => v.x;
        public static explicit operator long(ulong2 v) => (long)v.x;
        public static explicit operator int(ulong2 v) => (int)v.x;
        public static explicit operator float(ulong2 v) => v.x;
        public static explicit operator double(ulong2 v) => v.x;
        
        // (from Unity.Mathematics)
        public static explicit operator uint2(ulong2 v) => new((uint)v.x, (uint)v.y);
        public static explicit operator int2(ulong2 v) => new((int)v.x, (int)v.y);
        public static explicit operator float2(ulong2 v) => new(v.x, v.y);
        public static explicit operator double2(ulong2 v) => new(v.x, v.y);

        #endregion

        #region Operators

        // Arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(ulong2 a, ulong2 b) => new(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(ulong2 a, ulong b) => new(a.x + b, a.y + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(ulong a, ulong2 b) => new(a + b.x, a + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(ulong2 a, uint b) => new(a.x + b, a.y + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(uint a, ulong2 b) => new(a + b.x, a + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(ulong2 a, ulong2 b) => new(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(ulong2 a, ulong b) => new(a.x - b, a.y - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(ulong a, ulong2 b) => new(a - b.x, a - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(ulong2 a, uint b) => new(a.x - b, a.y - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(uint a, ulong2 b) => new(a - b.x, a - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator *(ulong2 a, ulong2 b) => new(a.x * b.x, a.y * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator *(ulong2 a, ulong b) => new(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator *(ulong a, ulong2 b) => new(a * b.x, a * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator *(ulong2 a, uint b) => new(a.x * b, a.y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator *(uint a, ulong2 b) => new(a * b.x, a * b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator /(ulong2 a, ulong2 b) => new(a.x / b.x, a.y / b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator /(ulong2 a, ulong b) => new(a.x / b, a.y / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator /(ulong a, ulong2 b) => new(a / b.x, a / b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator /(ulong2 a, uint b) => new(a.x / b, a.y / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator /(uint a, ulong2 b) => new(a / b.x, a / b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator %(ulong2 a, ulong2 b) => new(a.x % b.x, a.y % b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator %(ulong2 a, ulong b) => new(a.x % b, a.y % b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator %(ulong a, ulong2 b) => new(a % b.x, a % b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator %(ulong2 a, uint b) => new(a.x % b, a.y % b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator %(uint a, ulong2 b) => new(a % b.x, a % b.y);

        // Unary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ++ (ulong2 val) { return new ulong2 (++val.x, ++val.y); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -- (ulong2 val) { return new ulong2 (--val.x, --val.y); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator +(ulong2 a) => a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator -(ulong2 a) => new(0 - a.x, 0 - a.y);

        // Bitwise
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator &(ulong2 a, ulong2 b) => new(a.x & b.x, a.y & b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator &(ulong2 a, ulong b) => new(a.x & b, a.y & b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator &(ulong a, ulong2 b) => new(a & b.x, a & b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator &(ulong2 a, uint b) => new(a.x & b, a.y & b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator &(uint a, ulong2 b) => new(a & b.x, a & b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator |(ulong2 a, ulong2 b) => new(a.x | b.x, a.y | b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator |(ulong2 a, ulong b) => new(a.x | b, a.y | b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator |(ulong a, ulong2 b) => new(a | b.x, a | b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator |(ulong2 a, uint b) => new(a.x | b, a.y | b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator |(uint a, ulong2 b) => new(a | b.x, a | b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ^(ulong2 a, ulong2 b) => new(a.x ^ b.x, a.y ^ b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ^(ulong2 a, ulong b) => new(a.x ^ b, a.y ^ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ^(ulong a, ulong2 b) => new(a ^ b.x, a ^ b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ^(ulong2 a, uint b) => new(a.x ^ b, a.y ^ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ^(uint a, ulong2 b) => new(a ^ b.x, a ^ b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator ~(ulong2 a) => new(~a.x, ~a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator <<(ulong2 a, int n) => new(a.x << n, a.y << n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 operator >>(ulong2 a, int n) => new(a.x >> n, a.y >> n);

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(ulong2 a, ulong2 b) => new(a.x == b.x, a.y == b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(ulong2 a, ulong b) => new(a.x == b, a.y == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(ulong a, ulong2 b) => new(a == b.x, a == b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(ulong2 a, uint b) => new(a.x == b, a.y == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(uint a, ulong2 b) => new(a == b.x, a == b.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(ulong2 a, ulong2 b) => new(a.x != b.x, a.y != b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(ulong2 a, ulong b) => new(a.x != b, a.y != b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(ulong a, ulong2 b) => new(a != b.x, a != b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(ulong2 a, uint b) => new(a.x != b, a.y != b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(uint a, ulong2 b) => new(a != b.x, a != b.y);

        // Comparisons (returns bool2)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(ulong2 a, ulong2 b) => new(a.x < b.x, a.y < b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(ulong2 a, ulong b) => new(a.x < b, a.y < b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(ulong a, ulong2 b) => new(a < b.x, a < b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(ulong2 a, uint b) => new(a.x < b, a.y < b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <(uint a, ulong2 b) => new(a < b.x, a < b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(ulong2 a, ulong2 b) => new(a.x <= b.x, a.y <= b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(ulong2 a, ulong b) => new(a.x <= b, a.y <= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(ulong a, ulong2 b) => new(a <= b.x, a <= b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(ulong2 a, uint b) => new(a.x <= b, a.y <= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator <=(uint a, ulong2 b) => new(a <= b.x, a <= b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(ulong2 a, ulong2 b) => new(a.x > b.x, a.y > b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(ulong2 a, ulong b) => new(a.x > b, a.y > b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(ulong a, ulong2 b) => new(a > b.x, a > b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(ulong2 a, uint b) => new(a.x > b, a.y > b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >(uint a, ulong2 b) => new(a > b.x, a > b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(ulong2 a, ulong2 b) => new(a.x >= b.x, a.y >= b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(ulong2 a, ulong b) => new(a.x >= b, a.y >= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(ulong a, ulong2 b) => new(a >= b.x, a >= b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(ulong2 a, uint b) => new(a.x >= b, a.y >= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator >=(uint a, ulong2 b) => new(a >= b.x, a >= b.y);

        #endregion
        
        #region Math Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 min(ulong2 a, ulong2 b) => new(math.min(a.x, b.x), math.min(a.y, b.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 max(ulong2 a, ulong2 b) => new(math.max(a.x, b.x), math.max(a.y, b.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 clamp(ulong2 value, ulong2 min, ulong2 max) =>
            new(math.clamp(value.x, min.x, max.x), math.clamp(value.y, min.y, max.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong dot(ulong2 a, ulong2 b) => a.x * b.x + a.y * b.y;

        #endregion
        
        #region Overrides

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ulong2 other) => x == other.x && y == other.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is ulong2 other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            //(int)math.hash(new uint2((uint)(x ^ (x >> 32)), (uint)(y ^ (y >> 32))));
            (int)Math.hash(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"{nameof(x)}: {x}, {nameof(y)}: {y}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            FormattableString formattable = $"{nameof(x)}: {x}, {nameof(y)}: {y}";
            return formattable.ToString(formatProvider);
        }

        #endregion

        #region Swizzles

        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, y, y); }
        }

        /// <summary>Swizzles the vector.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ulong2 xx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(x, x);
        }

        /// <summary>Swizzles the vector.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ulong2 xy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(x, y);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; y = value.y; }
        }

        /// <summary>Swizzles the vector.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ulong2 yx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(y, x);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; x = value.y; }
        }

        /// <summary>Swizzles the vector.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ulong2 yy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(y, y);
        }

        #endregion

        internal sealed class DebuggerProxy
        {
            public ulong x;
            public ulong y;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DebuggerProxy(ulong2 v)
            {
                x = v.x;
                y = v.y;
            }
        }
    }
    
    public static partial class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(ulong x, ulong y) { return new ulong2(x, y); }
        
        /// <summary>Returns a ulong2 vector constructed from two uint values.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="y">The constructed vector's y component will be set to this value.</param>
        /// <returns>ulong2 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(uint x, uint y) { return new ulong2(x, y); }

        /// <summary>Returns a ulong2 vector constructed from a ulong2 vector.</summary>
        /// <param name="xy">The constructed vector's xy components will be set to this value.</param>
        /// <returns>ulong2 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(ulong2 xy) { return new ulong2(xy); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(ulong v) { return new ulong2(v); }

        /// <summary>Returns a ulong2 vector constructed from a single uint value by assigning it to every component.</summary>
        /// <param name="v">uint to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(uint v) { return new ulong2(v); }

        /// <summary>Returns a ulong2 vector constructed from a single bool value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">bool to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(bool v) { return new ulong2(v); }

        /// <summary>Return a ulong2 vector constructed from a bool2 vector by componentwise conversion.</summary>
        /// <param name="v">bool2 to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(bool2 v) { return new ulong2(v); }

        /// <summary>Returns a ulong2 vector constructed from a single int value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">int to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(int v) { return new ulong2(v); }

        /// <summary>Return a ulong2 vector constructed from a int2 vector by componentwise conversion.</summary>
        /// <param name="v">int2 to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(int2 v) { return new ulong2(v); }

        /// <summary>Returns a ulong2 vector constructed from a single float value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">float to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(float v) { return new ulong2(v); }

        /// <summary>Return a ulong2 vector constructed from a float2 vector by componentwise conversion.</summary>
        /// <param name="v">float2 to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(float2 v) { return new ulong2(v); }

        /// <summary>Returns a ulong2 vector constructed from a single double value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">double to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(double v) { return new ulong2(v); }

        /// <summary>Return a ulong2 vector constructed from a double2 vector by componentwise conversion.</summary>
        /// <param name="v">double2 to convert to ulong2</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 ulong2(double2 v) { return new ulong2(v); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong hash(ulong2 v)
        {
            return (v.x * 0xD6E8FEB86659FD93UL + v.y * 0xC2B2AE3D27D4EB4FUL) ^ 0x9E3779B97F4A7C15UL;
        }

        /// <summary>
        /// Returns a ulong2 vector hash code of a ulong2 vector.
        /// When multiple elements are to be hashes together, it can more efficient to calculate and combine wide hash
        /// that are only reduced to a narrow uint hash at the very end instead of at every step.
        /// </summary>
        /// <param name="v">Vector value to hash.</param>
        /// <returns>ulong2 hash of the argument.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 hashwide(ulong2 v)
        {
            return (v * ulong2(0xD6E8FEB86659FD93UL, 0xC2B2AE3D27D4EB4FUL)) + 0x9E3779B97F4A7C15UL;
        }

        /// <summary>Returns the result of specified shuffling of the components from two ulong2 vectors into a uint value.</summary>
        /// <param name="left">ulong2 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong2 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting uint.</param>
        /// <returns>uint result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong shuffle(ulong2 left, ulong2 right, math.ShuffleComponent x)
        {
            return select_shuffle_component(left, right, x);
        }

        /// <summary>Returns the result of specified shuffling of the components from two ulong2 vectors into a ulong2 vector.</summary>
        /// <param name="left">ulong2 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong2 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting ulong2 x component.</param>
        /// <param name="y">The ShuffleComponent to use when setting the resulting ulong2 y component.</param>
        /// <returns>ulong2 result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 shuffle(ulong2 left, ulong2 right, math.ShuffleComponent x, math.ShuffleComponent y)
        {
            return ulong2(
                select_shuffle_component(left, right, x),
                select_shuffle_component(left, right, y));
        }

        /// <summary>Returns the result of specified shuffling of the components from two ulong2 vectors into a uint4 vector.</summary>
        /// <param name="left">ulong2 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong2 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting uint4 x component.</param>
        /// <param name="y">The ShuffleComponent to use when setting the resulting uint4 y component.</param>
        /// <param name="z">The ShuffleComponent to use when setting the resulting uint4 z component.</param>
        /// <param name="w">The ShuffleComponent to use when setting the resulting uint4 w component.</param>
        /// <returns>uint4 result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 shuffle(ulong2 left, ulong2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
        {
            return ulong4(
                select_shuffle_component(left, right, x),
                select_shuffle_component(left, right, y),
                select_shuffle_component(left, right, z),
                select_shuffle_component(left, right, w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong select_shuffle_component(ulong2 a, ulong2 b, math.ShuffleComponent component)
        {
            return component switch
            {
                math.ShuffleComponent.LeftX => a.x,
                math.ShuffleComponent.LeftY => a.y,
                math.ShuffleComponent.RightX => b.x,
                math.ShuffleComponent.RightY => b.y,
                _ => throw new ArgumentException("Invalid shuffle component: " + component)
            };
        }

    }
}