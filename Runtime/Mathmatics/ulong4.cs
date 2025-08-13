using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace CoreFramework.Mathematics
{
    
    /// <summary>A 4 component vector of ulongs.</summary>
    
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [Serializable]
    [BurstCompile]
    public struct ulong4: IEquatable<ulong4>, IFormattable
    {
        /// <summary>x component of the vector.</summary>
        public ulong x;
        /// <summary>y component of the vector.</summary>
        public ulong y;
        /// <summary>z component of the vector.</summary>
        public ulong z;
        /// <summary>w component of the vector.</summary>
        public ulong w;

        /// <summary>uint4 zero value.</summary>
        public static readonly ulong4 zero;

        #region Constructors

        /// <summary>Constructs a ulong4 vector from four ulong values.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="y">The constructed vector's y component will be set to this value.</param>
        /// <param name="z">The constructed vector's z component will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong x, ulong y, ulong z, ulong w) => (this.x, this.y, this.z, this.w) = (x, y, z, w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(long x, long y, long z, long w) =>
            (this.x, this.y, this.z, this.w) = ((ulong)x, (ulong)y, (ulong)z, (ulong)w);

        /// <summary>Constructs a ulong4 vector from two ulong values and a ulong2 vector.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="y">The constructed vector's y component will be set to this value.</param>
        /// <param name="zw">The constructed vector's zw components will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong x, ulong y, ulong2 zw) => (this.x, this.y, z, w) = (x, y, zw.x, zw.y);

        /// <summary>Constructs a ulong4 vector from a ulong value, a ulong2 vector and a ulong value.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="yz">The constructed vector's yz components will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong x, ulong2 yz, ulong w) => (this.x, y, z, this.w) = (x, yz.x, yz.y, w);

        /*
        /// <summary>Constructs a ulong4 vector from a ulong value and a ulong3 vector.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="yzw">The constructed vector's yzw components will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong x, ulong3 yzw)
        {
            this.x = x;
            this.y = yzw.x;
            this.z = yzw.y;
            this.w = yzw.z;
        }
        */

        /// <summary>Constructs a ulong4 vector from a ulong2 vector and two ulong values.</summary>
        /// <param name="xy">The constructed vector's xy components will be set to this value.</param>
        /// <param name="z">The constructed vector's z component will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong2 xy, ulong z, ulong w) => (x, y, this.z, this.w) = (xy.x, xy.y, z, w);

        /// <summary>Constructs a ulong4 vector from two ulong2 vectors.</summary>
        /// <param name="xy">The constructed vector's xy components will be set to this value.</param>
        /// <param name="zw">The constructed vector's zw components will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong2 xy, ulong2 zw) => (x, y, z, w) = (xy.x, xy.y, zw.x, zw.y);

        /*
        /// <summary>Constructs a ulong4 vector from a ulong3 vector and a ulong value.</summary>
        /// <param name="xyz">The constructed vector's xyz components will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong3 xyz, ulong w)
        {
            this.x = xyz.x;
            this.y = xyz.y;
            this.z = xyz.z;
            this.w = w;
        }
        */

        /// <summary>Constructs a ulong4 vector from a ulong4 vector.</summary>
        /// <param name="xyzw">The constructed vector's xyzw components will be set to this value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(ulong4 xyzw) => (x, y, z, w) = (xyzw.x, xyzw.y, xyzw.z, xyzw.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(uint v) => (x, y, z, w) = (v, v, v, v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(uint4 v) => (x, y, z, w) = (v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(bool v) =>
            (x, y, z, w) = (v ? 1u : 0u, v ? 1u : 0u, v ? 1u : 0u, v ? 1u : 0u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(bool4 v) => (x, y, z, w) =
            (v.x ? 1u : 0u, v.y ? 1u : 0u, v.z ? 1u : 0u, v.w ? 1u : 0u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(int v) => (x, y, z, w) = ((ulong)v, (ulong)v, (ulong)v, (ulong)v);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(int4 v) => (x, y, z, w) = ((ulong)v.x, (ulong)v.y, (ulong)v.z, (ulong)v.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(float v) => (x, y, z, w) = ((ulong)v, (ulong)v, (ulong)v, (ulong)v);
        
        public ulong4(float4 v) => (x, y, z, w) = ((ulong)v.x, (ulong)v.y, (ulong)v.z, (ulong)v.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong4(double v) => (x, y, z, w) = ((ulong)v, (ulong)v, (ulong)v, (ulong)v);
        
        public ulong4(double4 v) => (x, y, z, w) = ((ulong)v.x, (ulong)v.y, (ulong)v.z, (ulong)v.w);

        #endregion
        
        #region Implicit Conversions From Unity Types

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(uint4 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(int4 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(float4 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(double4 v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(bool4 v) => new(v);

        #endregion
        
        #region Implicit Scalar Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4((ulong x, ulong y, ulong z, ulong w) v) => new(v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator (ulong, ulong, ulong, ulong)(ulong4 v) => (v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(ulong v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(long v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(uint v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(int v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(float v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(double v) => new(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong4(bool v) => new(v);

        #endregion
        
        #region Conversions

        // To scalar types (takes x component)
        public static explicit operator uint(ulong4 v) => (uint)v.x;
        public static explicit operator ulong(ulong4 v) => v.x;
        public static explicit operator long(ulong4 v) => (long)v.x;
        public static explicit operator int(ulong4 v) => (int)v.x;
        public static explicit operator float(ulong4 v) => v.x;
        public static explicit operator double(ulong4 v) => v.x;
        
        // (from Unity.Mathematics)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator uint4(ulong4 v) => new((uint)v.x, (uint)v.y, (uint)v.z, (uint)v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int4(ulong4 v) => new((int)v.x, (int)v.y, (int)v.z, (int)v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float4(ulong4 v) => new(v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double4(ulong4 v) => new(v.x, v.y, v.z, v.w);

        #endregion

        #region Operators

        // Arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator *(ulong4 a, ulong4 b) =>
            new(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator * (ulong4 a, ulong b) => new (a.x* b, a.y* b, a.z* b, a.w* b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator *(ulong a, ulong4 b) => new(a * b.x, a * b.y, a * b.z, a * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator * (ulong4 a, uint b) => new (a.x* b, a.y* b, a.z* b, a.w* b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator *(uint a, ulong4 b) => new(a * b.x, a * b.y, a * b.z, a * b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator +(ulong4 a, ulong4 b) =>
            new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator + (ulong4 a, ulong b) => new (a.x+ b, a.y+ b, a.z+ b, a.w+ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator +(ulong a, ulong4 b) => new(a + b.x, a + b.y, a + b.z, a + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator + (ulong4 a, uint b) => new (a.x+ b, a.y+ b, a.z+ b, a.w+ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator +(uint a, ulong4 b) => new(a + b.x, a + b.y, a + b.z, a + b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator -(ulong4 a, ulong4 b) =>
            new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator - (ulong4 a, ulong b) => new (a.x- b, a.y- b, a.z- b, a.w- b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator -(ulong a, ulong4 b) => new(a - b.x, a - b.y, a - b.z, a - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator - (ulong4 a, uint b) => new (a.x- b, a.y- b, a.z- b, a.w- b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator -(uint a, ulong4 b) => new(a - b.x, a - b.y, a - b.z, a - b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator /(ulong4 a, ulong4 b) =>
            new(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator / (ulong4 a, ulong b) => new (a.x/ b, a.y/ b, a.z/ b, a.w/ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator /(ulong a, ulong4 b) => new(a / b.x, a / b.y, a / b.z, a / b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator / (ulong4 a, uint b) => new (a.x/ b, a.y/ b, a.z/ b, a.w/ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator /(uint a, ulong4 b) => new(a / b.x, a / b.y, a / b.z, a / b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator %(ulong4 a, ulong4 b) =>
            new(a.x % b.x, a.y % b.y, a.z % b.z, a.w % b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator % (ulong4 a, ulong b) => new (a.x% b, a.y% b, a.z% b, a.w% b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator %(ulong a, ulong4 b) => new(a % b.x, a % b.y, a % b.z, a % b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator % (ulong4 a, uint b) => new (a.x% b, a.y% b, a.z% b, a.w% b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator %(uint a, ulong4 b) => new(a % b.x, a % b.y, a % b.z, a % b.w);

        // Unary
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ++(ulong4 val) => new(++val.x, ++val.y, ++val.z, ++val.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator -- (ulong4 val) => new (--val.x, --val.y, --val.z, --val.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator +(ulong4 a) => new(+a.x, +a.y, +a.z, +a.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator -(ulong4 a) => new (0-a.x, 0-a.y,  0-a.z,  0-a.w);

        // Bitwise
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator &(ulong4 a, ulong4 b) =>
            new(a.x & b.x, a.y & b.y, a.z & b.z, a.w & b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator & (ulong4 a, ulong b) => new (a.x& b, a.y& b, a.z& b, a.w& b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator &(ulong a, ulong4 b) => new(a & b.x, a & b.y, a & b.z, a & b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator & (ulong4 a, uint b) => new (a.x& b, a.y& b, a.z& b, a.w& b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator &(uint a, ulong4 b) => new(a & b.x, a & b.y, a & b.z, a & b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator |(ulong4 a, ulong4 b) =>
            new(a.x | b.x, a.y | b.y, a.z | b.z, a.w | b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator | (ulong4 a, ulong b) => new (a.x| b, a.y| b, a.z| b, a.w| b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator |(ulong a, ulong4 b) => new(a | b.x, a | b.y, a | b.z, a | b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator | (ulong4 a, uint b) => new (a.x| b, a.y| b, a.z| b, a.w| b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator |(uint a, ulong4 b) => new(a | b.x, a | b.y, a | b.z, a | b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ^(ulong4 a, ulong4 b) =>
            new(a.x ^ b.x, a.y ^ b.y, a.z ^ b.z, a.w ^ b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ^ (ulong4 a, ulong b) => new (a.x^ b, a.y^ b, a.z^ b, a.w^ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ^(ulong a, ulong4 b) => new(a ^ b.x, a ^ b.y, a ^ b.z, a ^ b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ^ (ulong4 a, uint b) => new (a.x^ b, a.y^ b, a.z^ b, a.w^ b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ^(uint a, ulong4 b) => new(a ^ b.x, a ^ b.y, a ^ b.z, a ^ b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator ~(ulong4 a) => new(~a.x, ~a.y, ~a.z, ~a.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator <<(ulong4 a, int n) => new(a.x << n, a.y << n, a.z << n, a.w << n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 operator >> (ulong4 a, int n) => new(a.x >> n, a.y >> n, a.z >> n, a.w >> n);

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator ==(ulong4 a, ulong4 b) =>
            new(a.x == b.x, a.y == b.y, a.z == b.z, a.w == b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator == (ulong4 a, ulong b) => new (a.x== b, a.y== b, a.z== b, a.w== b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator ==(ulong a, ulong4 b) => new(a == b.x, a == b.y, a == b.z, a == b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator == (ulong4 a, uint b) => new (a.x== b, a.y== b, a.z== b, a.w== b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator ==(uint a, ulong4 b) => new(a == b.x, a == b.y, a == b.z, a == b.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator !=(ulong4 a, ulong4 b) =>
            new(a.x != b.x, a.y != b.y, a.z != b.z, a.w != b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator != (ulong4 a, ulong b) => new (a.x!= b, a.y!= b, a.z!= b, a.w!= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator !=(ulong a, ulong4 b) => new(a != b.x, a != b.y, a != b.z, a != b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator != (ulong4 a, uint b) => new (a.x!= b, a.y!= b, a.z!= b, a.w!= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator !=(uint a, ulong4 b) => new(a != b.x, a != b.y, a != b.z, a != b.w);

        // Comparisons (returns bool4)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <(ulong4 a, ulong4 b) =>
            new(a.x < b.x, a.y < b.y, a.z < b.z, a.w < b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator < (ulong4 a, ulong b) => new (a.x< b, a.y< b, a.z< b, a.w< b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <(ulong a, ulong4 b) => new(a < b.x, a < b.y, a < b.z, a < b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator < (ulong4 a, uint b) => new (a.x< b, a.y< b, a.z< b, a.w< b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <(uint a, ulong4 b) => new(a < b.x, a < b.y, a < b.z, a < b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <=(ulong4 a, ulong4 b) =>
            new(a.x <= b.x, a.y <= b.y, a.z <= b.z, a.w <= b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <= (ulong4 a, ulong b) => new (a.x<= b, a.y<= b, a.z<= b, a.w<= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <=(ulong a, ulong4 b) => new(a <= b.x, a <= b.y, a <= b.z, a <= b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <= (ulong4 a, uint b) => new (a.x<= b, a.y<= b, a.z<= b, a.w<= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator <=(uint a, ulong4 b) => new(a <= b.x, a <= b.y, a <= b.z, a <= b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >(ulong4 a, ulong4 b) =>
            new(a.x > b.x, a.y > b.y, a.z > b.z, a.w > b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator > (ulong4 a, ulong b) => new (a.x> b, a.y> b, a.z> b, a.w> b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >(ulong a, ulong4 b) => new(a > b.x, a > b.y, a > b.z, a > b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator > (ulong4 a, uint b) => new (a.x> b, a.y> b, a.z> b, a.w> b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >(uint a, ulong4 b) => new(a > b.x, a > b.y, a > b.z, a > b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >=(ulong4 a, ulong4 b) =>
            new(a.x >= b.x, a.y >= b.y, a.z >= b.z, a.w >= b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >= (ulong4 a, ulong b) => new (a.x>= b, a.y>= b, a.z>= b, a.w>= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >=(ulong a, ulong4 b) => new(a >= b.x, a >= b.y, a >= b.z, a >= b.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >= (ulong4 a, uint b) => new (a.x>= b, a.y>= b, a.z>= b, a.w>= b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 operator >=(uint a, ulong4 b) => new(a >= b.x, a >= b.y, a >= b.z, a >= b.w);

        #endregion
        
        #region Math Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 min(ulong4 a, ulong4 b) => new(math.min(a.x, b.x), math.min(a.y, b.y), math.min(a.z, b.z),
            math.min(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 max(ulong4 a, ulong4 b) => new(math.max(a.x, b.x), math.max(a.y, b.y), math.max(a.z, b.z),
            math.max(a.w, b.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 clamp(ulong4 value, ulong4 min, ulong4 max) =>
            new(math.clamp(value.x, min.x, max.x),
                math.clamp(value.y, min.y, max.y),
                math.clamp(value.z, min.z, max.z),
                math.clamp(value.w, min.w, max.w));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong dot(ulong4 a, ulong4 b) => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        #endregion
        
        #region Overrides

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ulong4 other) => x == other.x && y == other.y && z == other.z && w == other.w;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is ulong4 other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            (int)Math.hash(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"ulong4({x}, {y}, {z}, {w})";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) =>
            $"ulong4({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)}, {w.ToString(format, formatProvider)})";
        
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
        public ulong4 xxxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, x, w); }
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
        public ulong4 xxyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xxww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, x, w, w); }
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
        public ulong4 xyxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, x, w); }
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
        public ulong4 xyyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, z, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; y = value.y; z = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xywx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xywy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xywz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, w, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; y = value.y; w = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xyww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, y, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, y, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; z = value.y; y = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, w, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; z = value.y; w = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xzww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, z, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, y, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; w = value.y; y = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, z, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; w = value.y; z = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 xwww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(x, w, w, w); }
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
        public ulong4 yxxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, x, w); }
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
        public ulong4 yxyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, z, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; x = value.y; z = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, w, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; x = value.y; w = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yxww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, x, w, w); }
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
        public ulong4 yyxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, x, w); }
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yywx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yywy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yywz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yyww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, y, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, x, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; z = value.y; x = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, w, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; z = value.y; w = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 yzww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, z, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, x, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; w = value.y; x = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, z, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; w = value.y; z = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 ywww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(y, w, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, y, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; x = value.y; y = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, w, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; x = value.y; w = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zxww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, x, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, x, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; y = value.y; x = value.z; w = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zywx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, w, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; y = value.y; w = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zywy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zywz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zyww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, y, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zzww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, z, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, x, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; w = value.y; x = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, y, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; w = value.y; y = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 zwww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(z, w, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, y, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; x = value.y; y = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, z, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; x = value.y; z = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wxww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, x, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, x, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; y = value.y; x = value.z; z = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, z, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; y = value.y; z = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wywx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wywy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wywz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wyww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, y, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, x, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; z = value.y; x = value.z; y = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, y, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; z = value.y; y = value.z; x = value.w; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wzww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, z, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, x, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwxz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, x, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwxw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, x, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, y, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwyz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, y, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwyw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, y, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwzx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, z, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwzy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, z, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwzz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwzw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, z, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwwx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, w, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwwy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, w, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwwz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, w, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong4 wwww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong4(w, w, w, w); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 xx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(x, x); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 xy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(x, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; y = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 xz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(x, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; z = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 xw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(x, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { x = value.x; w = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 yx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(y, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; x = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 yy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(y, y); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 yz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(y, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; z = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 yw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(y, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { y = value.x; w = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 zx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(z, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; x = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 zy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(z, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; y = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 zz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(z, z); }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 zw
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(z, w); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { z = value.x; w = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 wx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(w, x); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; x = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 wy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(w, y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; y = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 wz
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(w, z); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { w = value.x; z = value.y; }
        }


        /// <summary>Swizzles the vector.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ulong2 ww
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new ulong2(w, w); }
        }

        #endregion

        internal sealed class DebuggerProxy
        {
            public ulong x;
            public ulong y;
            public ulong z;
            public ulong w;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DebuggerProxy(ulong4 v)
            {
                x = v.x;
                y = v.y;
                z = v.z;
                w = v.w;
            }
        }
    }

    public static partial class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong x, ulong y, ulong z, ulong w) => new(x, y, z, w);

        /// <summary>Returns a ulong4 vector constructed from two uint values and a uint2 vector.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="y">The constructed vector's y component will be set to this value.</param>
        /// <param name="zw">The constructed vector's zw components will be set to this value.</param>
        /// <returns>ulong4 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong x, ulong y, ulong2 zw) { return new ulong4(x, y, zw); }

        /// <summary>Returns a ulong4 vector constructed from a uint value, a uint2 vector and a uint value.</summary>
        /// <param name="x">The constructed vector's x component will be set to this value.</param>
        /// <param name="yz">The constructed vector's yz components will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        /// <returns>ulong4 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong x, ulong2 yz, ulong w) { return new ulong4(x, yz, w); }

        /// <summary>Returns a ulong4 vector constructed from a uint2 vector and two uint values.</summary>
        /// <param name="xy">The constructed vector's xy components will be set to this value.</param>
        /// <param name="z">The constructed vector's z component will be set to this value.</param>
        /// <param name="w">The constructed vector's w component will be set to this value.</param>
        /// <returns>ulong4 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong2 xy, ulong z, ulong w) { return new ulong4(xy, z, w); }

        /// <summary>Returns a ulong4 vector constructed from two uint2 vectors.</summary>
        /// <param name="xy">The constructed vector's xy components will be set to this value.</param>
        /// <param name="zw">The constructed vector's zw components will be set to this value.</param>
        /// <returns>ulong4 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong2 xy, ulong2 zw) { return new ulong4(xy, zw); }

        /// <summary>Returns a ulong4 vector constructed from a ulong4 vector.</summary>
        /// <param name="xyzw">The constructed vector's xyzw components will be set to this value.</param>
        /// <returns>ulong4 constructed from arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong4 xyzw) { return new ulong4(xyzw); }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(ulong v) { return new ulong4(v); }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(long v) { return new ulong4(v); }

        /// <summary>Returns a ulong4 vector constructed from a single bool value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">bool to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(bool v) { return new ulong4(v); }

        /// <summary>Return a ulong4 vector constructed from a bool4 vector by componentwise conversion.</summary>
        /// <param name="v">bool4 to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(bool4 v) { return new ulong4(v); }

        /// <summary>Returns a ulong4 vector constructed from a single int value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">int to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(int v) { return new ulong4(v); }

        /// <summary>Return a ulong4 vector constructed from a int4 vector by componentwise conversion.</summary>
        /// <param name="v">int4 to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(int4 v) { return new ulong4(v); }

        /// <summary>Returns a ulong4 vector constructed from a single uint value by assigning it to every component.</summary>
        /// <param name="v">uint to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(uint v) { return new ulong4(v); }

        /// <summary>Return a ulong4 vector constructed from a uint4 vector by componentwise conversion.</summary>
        /// <param name="v">uint4 to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(uint4 v) { return new ulong4(v); }

        /// <summary>Returns a ulong4 vector constructed from a single float value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">float to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(float v) { return new ulong4(v); }

        /// <summary>Return a ulong4 vector constructed from a float4 vector by componentwise conversion.</summary>
        /// <param name="v">float4 to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(float4 v) { return new ulong4(v); }

        /// <summary>Returns a ulong4 vector constructed from a single double value by converting it to uint and assigning it to every component.</summary>
        /// <param name="v">double to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(double v) { return new ulong4(v); }

        /// <summary>Return a ulong4 vector constructed from a double4 vector by componentwise conversion.</summary>
        /// <param name="v">double4 to convert to ulong4</param>
        /// <returns>Converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 ulong4(double4 v) { return new ulong4(v); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong hash(ulong4 v) => (v.x * 0xD6E8FEB86659FD93UL
                                               + v.y * 0xC2B2AE3D27D4EB4FUL
                                               + v.z * 0x165667B19E3779F9UL
                                               + v.w * 0x9E3779B97F4A7C15UL)
                                              ^ 0xDB4F0B9175AE2165UL;

        /// <summary>
        /// Returns a ulong4 vector hash code of a ulong4 vector.
        /// When multiple elements are to be hashes together, it can more efficient to calculate and combine wide hash
        /// that are only reduced to a narrow uint hash at the very end instead of at every step.
        /// </summary>
        /// <param name="v">Vector value to hash.</param>
        /// <returns>ulong4 hash of the argument.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 hashwide(ulong4 v) => ulong4(
            v.x * 0xD6E8FEB86659FD93UL + v.y * 0xC2B2AE3D27D4EB4FUL + v.z * 0x165667B19E3779F9UL +
            v.w * 0x9E3779B97F4A7C15UL,
            v.y * 0xC2B2AE3D27D4EB4FUL + v.z * 0x9E3779B97F4A7C15UL + v.w * 0xDB4F0B9175AE2165UL +
            v.x * 0xD6E8FEB86659FD93UL,
            v.z * 0x165667B19E3779F9UL + v.w * 0xDB4F0B9175AE2165UL + v.x * 0xC2B2AE3D27D4EB4FUL +
            v.y * 0xD6E8FEB86659FD93UL,
            v.w * 0x9E3779B97F4A7C15UL + v.x * 0xDB4F0B9175AE2165UL + v.y * 0x165667B19E3779F9UL +
            v.z * 0xC2B2AE3D27D4EB4FUL);

        /// <summary>Returns the result of specified shuffling of the components from two ulong4 vectors into a ulong value.</summary>
        /// <param name="left">ulong4 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong4 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting ulong.</param>
        /// <returns>ulong result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong shuffle(ulong4 left, ulong4 right, math.ShuffleComponent x)
        {
            return select_shuffle_component(left, right, x);
        }

        /// <summary>Returns the result of specified shuffling of the components from two ulong4 vectors into a ulong2 vector.</summary>
        /// <param name="left">ulong4 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong4 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting ulong2 x component.</param>
        /// <param name="y">The ShuffleComponent to use when setting the resulting ulong2 y component.</param>
        /// <returns>ulong2 result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong2 shuffle(ulong4 left, ulong4 right, math.ShuffleComponent x, math.ShuffleComponent y)
        {
            return ulong2(
                select_shuffle_component(left, right, x),
                select_shuffle_component(left, right, y));
        }

        /// <summary>Returns the result of specified shuffling of the components from two ulong4 vectors into a ulong4 vector.</summary>
        /// <param name="left">ulong4 to use as the left argument of the shuffle operation.</param>
        /// <param name="right">ulong4 to use as the right argument of the shuffle operation.</param>
        /// <param name="x">The ShuffleComponent to use when setting the resulting ulong4 x component.</param>
        /// <param name="y">The ShuffleComponent to use when setting the resulting ulong4 y component.</param>
        /// <param name="z">The ShuffleComponent to use when setting the resulting ulong4 z component.</param>
        /// <param name="w">The ShuffleComponent to use when setting the resulting ulong4 w component.</param>
        /// <returns>ulong4 result of the shuffle operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong4 shuffle(ulong4 left, ulong4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
        {
            return ulong4(
                select_shuffle_component(left, right, x),
                select_shuffle_component(left, right, y),
                select_shuffle_component(left, right, z),
                select_shuffle_component(left, right, w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong select_shuffle_component(ulong4 a, ulong4 b, math.ShuffleComponent component)
        {
            return component switch
            {
                math.ShuffleComponent.LeftX => a.x,
                math.ShuffleComponent.LeftY => a.y,
                math.ShuffleComponent.LeftZ => a.z,
                math.ShuffleComponent.LeftW => a.w,
                math.ShuffleComponent.RightX => b.x,
                math.ShuffleComponent.RightY => b.y,
                math.ShuffleComponent.RightZ => b.z,
                math.ShuffleComponent.RightW => b.w,
                _ => throw new ArgumentException("Invalid shuffle component: " + component)
            };
        }

    }
}