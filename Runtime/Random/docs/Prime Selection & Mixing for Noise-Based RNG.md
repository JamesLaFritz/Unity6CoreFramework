# 🎲 Cheat-Sheet: Prime Selection & Mixing for Noise-Based RNG

## **Why Do We Use Large Primes and Mixing?**

* **Large primes** break up regular patterns, avoid overlaps, and maximize hash diffusion.
* **Mixing operations** (XOR, shifts, multiplies, adds) further scramble bits for maximum entropy and uniformity.
* **Goal:** Ensure that any (x, y, z, w, seed) combination yields a “unique” output with no obvious patterns or correlations.

---

## **32-Bit Primes and Mixing Constants**

| Name                  | Value (Hex)  | Source / Purpose      |
| --------------------- | ------------ | --------------------- |
| **MurmurHash3Final1** | `0x85EBCA6B` | MurmurHash3 final mix |
| **MurmurHash3Final2** | `0xC2B2AE35` | MurmurHash3 final mix |
| **GoldenRatio**       | `0x9E3779B9` | 2³² / Golden ratio    |
| **ChaChaPrime1**      | `0x61707865` | ChaCha: `"expa"`      |
| **ChaChaPrime2**      | `0x3320646E` | ChaCha: `"nd 3"`      |
| **ChaChaPrime3**      | `0x79622D32` | ChaCha: `"2-by"`      |
| **ChaChaPrime4**      | `0x6B206574` | ChaCha: `"te k"`      |
| **2D Prime**          | `0x0BDADD45` | `198491317` (Y)       |
| **3D Prime**          | `0x0063CFFD` | `6542989` (Z)         |
| **4D Prime**          | `0x00057327` | `357239` (W)          |

---

## **64-Bit Primes and Mixing Constants**

| Name                    | Value (Hex)            | Source / Purpose               |
| ----------------------- | ---------------------- | ------------------------------ |
| **MurmurHash3Final1Ul** | `0x85EBCA77C2B2AE63UL` | MurmurHash3 / xxHash64         |
| **MurmurHash3Final2Ul** | `0xC2B2AE3D27D4EB4FUL` | MurmurHash3 / xxHash64         |
| **GoldenRatioUl**       | `0x9E3779B97F4A7C15UL` | 2⁶⁴ / Golden ratio             |
| **ChaChaPrime1Ul**      | `0x6170786561707865UL` | ChaCha: `"expaxpax"`           |
| **ChaChaPrime2Ul**      | `0x3320646E6E20646EUL` | ChaCha: `"nd 3nd 3"`           |
| **ChaChaPrime3Ul**      | `0x79622D3279622D32UL` | ChaCha: `"2-by2-by"`           |
| **ChaChaPrime4Ul**      | `0x6B2065746B206574UL` | ChaCha: `"te kte k"`           |
| **Flatten Prime1**      | `0xD6E8FEB86659FD93UL` | xxHash64, high entropy         |
| **Flatten Prime2**      | `0xC2B2AE3D27D4EB4FUL` | Murmur/xxHash, high entropy    |
| **Flatten Prime3**      | `0x165667B19E3779F9UL` | Golden ratio + irregular prime |
| **Alt Golden Ratio**    | `0x9E3779B185EBCA87UL` | 2⁶⁴ / Golden ratio (alt form)  |
| **Alt Mix Prime**       | `0xC13FA9A902A6328FUL` | PCG, hash mixing               |
| **Alt Mix Prime2**      | `0x91E10DA5C79E7B1DUL` | PCG, hash mixing               |
| **2D Prime (Y)**        | `0xC4B1D213A7B07FFDUL` | 1,417,348,726,984,969,213 Large, busy, all bits used   |
| **3D Prime (Z)**        | `0x8F0F8D2636C5A3D1UL` | 1,029,101,856,066,667,729 Similar scale, unique bits   |
| **4D Prime (W)**        | `0xE23C159B63B9A3B7UL` | 1,627,230,356,672,515,639 Also busy, plenty of entropy |

---

## **Usage Patterns (Copy/Paste Snippets)**

### **32-bit Index Flattening**

```c
// For 2D
uint flatIndex = x + y * 198491317;

// For 3D
uint flatIndex = x + y * 198491317 + z * 6542989;

// For 4D
uint flatIndex = x + y * 198491317 + z * 6542989 + w * 357239;
```
* *Can swap in any primes you like, as long as they’re large and not simple multiples of each other.*

```csharp
uint flatIndex2D = (uint)x + (uint)y * 0x0BDADD45;
uint flatIndex3D = (uint)x + (uint)y * 0x0BDADD45 + (uint)z * 0x0063CFFD;
uint flatIndex4D = (uint)x + (uint)y * 0x0BDADD45 + (uint)z * 0x0063CFFD + (uint)w * 0x00057327;
```

### **64-bit Index Flattening**

```csharp
ulong flatIndex2D = (ulong)x + (ulong)y * 0xD6E8FEB86659FD93UL;
ulong flatIndex3D = (ulong)x + (ulong)y * 0xD6E8FEB86659FD93UL + (ulong)z * 0xC2B2AE3D27D4EB4FUL;
ulong flatIndex4D = (ulong)x + (ulong)y * 0xD6E8FEB86659FD93UL + (ulong)z * 0xC2B2AE3D27D4EB4FUL + (ulong)w * 0x165667B19E3779F9UL;
```

### **Classic SquirrelNoise Mixing (32-bit)**

```csharp
uint mangled = index * 0xB5297A4D;
mangled += seed;
mangled ^= mangled >> 8;
mangled += 0x68E31DA4;
mangled ^= mangled << 8;
mangled *= 0x1B56C4E9;
mangled ^= mangled >> 8;
```

### **MurmurHash3 Finalizer (32-bit)**

```csharp
uint value = index;
value ^= value >> 16;
value *= 0x85EBCA6B;
value ^= value >> 13;
value *= 0xC2B2AE35;
value ^= value >> 16;
```

### **PCG/Golden Ratio Mix (32/64-bit)**

```csharp
uint value = index * 0x9E3779B9;
ulong value64 = index * 0x9E3779B97F4A7C15UL;
```

---

## **Noise Bit-Mangling: Recommended Mixing Operations**

### **Classic “SquirrelNoise” Mixing:**

```c
uint mangled = index * 0xB5297A4D;
mangled += seed;
mangled ^= mangled >> 8;
mangled += 0x68E31DA4;
mangled ^= mangled << 8;
mangled *= 0x1B56C4E9;
mangled ^= mangled >> 8;
```

* **Explanation:** Multiply, add, shift, XOR, repeat.
* **Variation:** Try rotating shifts, adding other noise constants, etc.

### **Other Proven Mixes:**

* **PCG/Golden Ratio**

  * `value *= 0x9E3779B9;`
* **MurmurHash3 Finalizer**

  ```c
  value ^= value >> 16;
  value *= 0x85EBCA6B;
  value ^= value >> 13;
  value *= 0xC2B2AE35;
  value ^= value >> 16;
  ```
* **ChaCha Quarter Round Primes (for crypto or heavy-duty mixing):**

  * `0x61707865, 0x3320646E, 0x79622D32, 0x6B206574`
* **Mixing “Interesting Bits”:**

  * Choose numbers that are odd, high-entropy, and not close in value.

---

## **Tips**

* For each new dimension, choose a prime at least 10x larger than the previous, and use *weird* bit-patterns for diffusion.
* Use these same patterns for seed mixing and any “extra randomness” steps in your noise functions.
* For 64-bit systems, use the `UL` suffix and 0x-prefixed hex constants.

---

**Reference these constants and code snippets directly in your RNG/noise implementations for reliable, portable, and fair procedural randomness!**

---

## **Best Practices**

* **Use distinct, odd, high-bit primes** for every axis/step.
* **Mix** at least 2–3 different large constants in every output.
* **Don’t just multiply—combine multiply, add, XOR, and shifts/rotates** for best diffusion.
* **Avoid simple sequences** (e.g., don’t just use 2, 3, 5, 7, …).
* **For cryptographic use:**

  * Use proven cryptographic mixing (ChaCha, SHA2, etc.), not homegrown methods.

---

## **References**

* Squirrel Eiserloh (GDC 2017) [YouTube](https://www.youtube.com/watch?v=LWFzPP8ZbdU)
* PCG Paper: [PCG-Random.org](https://www.pcg-random.org/)
* MurmurHash3: [Wikipedia](https://en.wikipedia.org/wiki/MurmurHash)
* Golden Ratio: \[Knuth, TAOCP Vol. 2]

---

## **Quick Copy-Paste Constants Table**

```c
// Primes for flattening:
198491317, 6542989, 357239

// SquirrelNoise (Squirrel Eiserloh GDC):
0xB5297A4D, 0x68E31DA4, 0x1B56C4E9

// PCG/Golden Ratio:
0x9E3779B9

// MurmurHash3 Finalizers:
0x85EBCA6B, 0xC2B2AE35

// ChaCha (crypto):
0x61707865, 0x3320646E, 0x79622D32, 0x6B206574
```

---

## **Rule of Thumb**

> *If you see visible patterns, try larger primes or add more mixing steps!*
>
> *For game worlds, you rarely need more than 32 bits and three mixing steps, but crypto and serialization need more care.*

---

Let me know if you want this as a Markdown file, image, printable PDF, or want a code snippet generator for specific dimensions!
