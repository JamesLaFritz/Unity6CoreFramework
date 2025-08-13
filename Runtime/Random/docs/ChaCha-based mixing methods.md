Focus on the **ChaCha-based mixing methods** in the four implementations:

* `SquirrelNoise32`
* `SquirrelNoise64`
* `SquirrelNoise32Rng`
* `SquirrelNoise64Rng`

and compare how they’re used, what you get out of each, and practical tips for usage.


---

## **What Are the ChaCha Mixing Methods?**

In your options:

```
[Description("ChaCha Quarter Round")]
    ChaChaQuarterRound, // Renamed to ChaChaSimple
[Description("ChaCha Quarter Round Compact Mixing 1")]
    ChaChaQuarterRoundCompactMixing1, // Removed, same as ChaChaQuarterRoundCompactMixing2; 32 Bit b = (b << 31/7) | (b >> 1); 64 Bit b = RotateLeft(b, 63/7)
[Description("ChaCha Quarter Round Compact Mixing 2")]
    ChaChaQuarterRoundCompactMixing2, // Renamed to ChaChaAdnvanced
```

---

## 🟦 **How ChaCha-Based Mixing Works in SquirrelNoise Variants**

All four classes use the **same core ChaCha-inspired mixing technique**, but differ by:

* **Bitness:** 32-bit (`uint`) vs 64-bit (`ulong`)
* **Usage:** Stateless noise (for index/seed-to-value) vs stateful RNG (advanceable, sequence-like random generator)

### 1. **SquirrelNoise32**

**(Stateless, 32-bit, supports ChaCha mixers)**

* Provides `Get1DNoise(index, seed, NoiseType)` and friends
* When `NoiseType.ChaChaQuarterRound*` is selected, it uses a *compact* ChaCha quarter-round
* **Purpose:** Map `(index, seed)` to a 32-bit random value, always deterministically

#### **Core Mixing Function (32-bit):**

```csharp
public static uint ChaChaQuarterRound(uint index, uint seed)
{
    uint a = index ^ ChaChaPrime1;
    uint b = seed ^ ChaChaPrime2;
    uint c = index + ChaChaPrime3;
    uint d = seed + ChaChaPrime4;

    a += b; d ^= a; d = RotateLeft(d, 16);
    c += d; b ^= c; b = RotateLeft(b, 12);
    a += b; d ^= a; d = RotateLeft(d, 8);
    c += d; b ^= c; b = RotateLeft(b, 7);

    return a ^ b ^ c ^ d;
}
```

* **Strong mixing, highly resistant to low/high bit patterns**
* **Works for infinite terrain, worldgen, “any-index” access**

### 2. **SquirrelNoise64**

**(Stateless, 64-bit, supports ChaCha mixers)**

* Provides `Get1DNoise(index, seed, NoiseType)` for `ulong`
* When `NoiseType.ChaChaQuarterRound*` is selected, uses an *expanded* ChaCha-style mixing
* **Purpose:** Same as above, but for 64-bit indices/seeds or when higher entropy is required

#### **Core Mixing Function (64-bit):**

```csharp
public static ulong ChaChaQuarterRound(ulong index, ulong seed)
{
    ulong a = index + ChaChaPrime1Ul;
    ulong b = seed + ChaChaPrime2Ul;
    ulong c = index ^ ChaChaPrime3Ul;
    ulong d = seed ^ ChaChaPrime4Ul;

    a += b; d ^= a; d = RotateLeft(d, 32);
    c += d; b ^= c; b = RotateLeft(b, 24);
    a += b; d ^= a; d = RotateLeft(d, 16);
    c += d; b ^= c; b = RotateLeft(b, 7);

    return a ^ b ^ c ^ d;
}
```

* **Same “quarter round” structure, but with 64-bit math**
* Use for **chunk IDs, world seeds, GUID generation, or long indices**

### 3. **SquirrelNoise32Rng**

**(Stateful RNG, 32-bit, supports ChaCha mixers)**

* Implements `IRandomFunction<uint, uint>`
* Internally advances its index/counter, uses ChaCha mixing per call (when selected)
* `ResetSeed(seed, index)` lets you jump to *any* sequence position deterministically

#### **How it works:**

```csharp
private uint _seed, _index;
public uint Uint32 => SquirrelNoise32.ChaChaQuarterRound(_index++, _seed);
```

* **No “real” RNG state—just index and seed!**
* Every next value is `ChaChaQuarterRound(index++, seed)`
* **Fast, stateless, parallelizable, deterministic**

### 4. **SquirrelNoise64Rng**

**(Stateful RNG, 64-bit, supports ChaCha mixers)**

* Implements `IRandomFunction<ulong, ulong>`
* Advances `index` internally, uses 64-bit ChaCha mixing for each next value
* Good for huge spaces or “never repeat in a lifetime” streams

#### **How it works:**

```csharp
private ulong _seed, _index;
public ulong Uint64 => SquirrelNoise64.ChaChaQuarterRound(_index++, _seed);
```

* **Each call advances index** and returns new mixed value
* **Perfect for parallel/async content generation** or anywhere you want deterministic randomness on demand

---

## 🟧 **Summary Table: When to Use Each**

| Class              | Bitness | Usage        | When to Use ChaCha Mixer?         |
| ------------------ | ------- | ------------ | --------------------------------- |
| SquirrelNoise32    | 32-bit  | Stateless    | General world/feature noise       |
| SquirrelNoise64    | 64-bit  | Stateless    | Long indices, world IDs, GUIDs    |
| SquirrelNoise32Rng | 32-bit  | Stateful RNG | Sequence, loot tables, quick runs |
| SquirrelNoise64Rng | 64-bit  | Stateful RNG | Massive sequences, global streams |

**Choose ChaCha mixer for:**

* **Best bit diffusion, less bias/artifacts, great for unpredictable content**
* **Edge-case safety:** Unusual indices or high/low bit concentration won’t create repeating patterns

---

## 🟩 **Why ChaCha Mixing Stands Out in All Four**

* **Consistent:** Same mixing logic across stateless and stateful implementations
* **Fast:** Only a handful of adds/xors/rotates per number—barely slower than simpler hash, but much higher quality
* **“No-warmup, no state leaks”:** Each output is immediately high-quality, no seeding/“warmup” tricks needed
* **Order-independent:** For stateless use, you can jump to any index—*perfect for procedural worlds*

---

## 🟨 **When NOT to Use ChaCha Mixers?**

* If you’re super tight on cycles (micro-optimizing for fastest possible noise): consider classic Squirrel or MangledBits, but almost never needed—the difference is negligible except in the hottest inner loops.

---

## **TL;DR:**

**ChaCha-based mixing in SquirrelNoise* is the best “default” if you want:*\*

* No RNG patterns
* Easy debugging/porting
* Safe for content/loot/world gen
* Secure enough for pseudo-random IDs (but not true cryptography unless you run many rounds)

---
