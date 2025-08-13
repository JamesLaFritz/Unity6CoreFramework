# 🐿️ Noise-Based Randomness for Procedural Generation

## **A Manifesto and Implementation Guide**

*Inspired by Squirrel Eiserloh’s GDC 2017 “Math for Game Programmers”*

---

## **What’s Wrong with “Normal” RNGs?**

Traditional PRNGs (like `System.Random`, Mersenne Twister, `std::rand`) are great for single-threaded gameplay, but are **ill-suited** for large-scale procedural generation, parallel worlds, and reproducibility. Their main flaws:

* **Stateful**: Outputs depend on *global mutable state*; can’t jump to an arbitrary position.
* **Order-dependent**: Generating random values out-of-order (or on a different thread!) gives different results.
* **Difficult to parallelize**: Thread-safe use is slow or awkward.
* **Re-seeding is slow**: Rebuilding the sequence or jumping ahead is expensive.
* **Not deterministic across platforms**: Floating point or implementation quirks can break consistency.

---

## **Why Noise Functions are the Future**

A **noise-based RNG** is simply a function:

```
random_value = NoiseFunction(position, seed)
```

Where `position` is any index/coordinate, and `seed` is a user-chosen value (like a world seed).

### **Benefits:**

* **Stateless & Order-independent:** Any value, any order, any time.
* **Perfect for procedural content:**

  * Generate a world, dungeon, or landscape *piecemeal*, yet get the *same result* every time for the same position+seed.
* **Lock-free parallelism:** Any thread can access any random value—no synchronization needed!
* **Lightning-fast “seeding”:** Want to generate a new world? Just pick a new seed.
* **Reproducible & deterministic:** Content is consistent across platforms.
* **No “bad” seeds:** Every possible seed is valid.

---

## **How Does It Work?**

A good **noise-based RNG** typically mangles its inputs through a sequence of cheap operations and “bit noise” constants:

```csharp
uint MangledNoise(uint pos, uint seed) {
    uint mangled = pos * 0xB5297A4D;        // Mix index with a big, odd constant
    mangled += seed;
    mangled ^= (mangled >> 8);              // Bitwise shift/XOR for extra diffusion
    mangled += 0x68E31DA4;
    mangled ^= (mangled << 8);
    mangled *= 0x1B56C4E9;
    mangled ^= (mangled >> 8);
    return mangled;
}
```

* *For 2D/3D/4D inputs*: “Flatten” coordinates using unique, large primes:

  * 2D: `index = x + y * 198491317`
  * 3D: `index = x + y * 198491317 + z * 6542989`
  * 4D: `index = x + y * 198491317 + z * 6542989 + w * 357239`

---

### **ASCII Value Flow Diagram**

```
+-----------+      +-----------+      +----------------+      +-----------------+
|  Index    | ---> | Multiply  | ---> | Add Seed       | ---> |  Bit Mangling   |
| (pos/key) |      | by PRIME  |      |                |      | (XOR/Shift/Mul) |
+-----------+      +-----------+      +----------------+      +-----------------+
                                                                     |
                                                                     v
                                                         +----------------------+
                                                         |  More Bit Mangling   |
                                                         |  (Add/XOR/Shift/Mul) |
                                                         +----------------------+
                                                                     |
                                                                     v
                                                          +--------------------+
                                                          |   Final Output     |
                                                          | (UInt/Float/etc.)  |
                                                          +--------------------+
```

**Summary:**

* **Index** (e.g. position, flat coordinate, object ID) is multiplied by a big prime.
* **Seed** is added (to ensure unique output for each world/session).
* **Bit-mangling**: Several rounds of XOR, shifts, additions, and multiplications with more noise constants scramble the bits.
* **Final Output**: The fully-mixed integer is returned (possibly mapped to float \[0,1] if needed).

---

### **Mermaid Diagram**

```mermaid
flowchart LR
    A[Index / Position / Key] --> B[Multiply by Prime]
    B --> C[Add Seed]
    C --> D[Bit Mangling<br/>(XOR/Shift/Multiply/Add)]
    D --> E{More Bit Mangling?}
    E -- Yes --> D
    E -- No --> F[Final Output<br/>(UInt32 / UInt64 / Float)]
```

---

### **Short Explanation (for README or Presentation):**

1. **Index Calculation:**

   * (x, y, z, ...) → flat index (using big primes for each dimension)
2. **Mix With Seed:**

   * Adds variability between worlds/games/runs.
3. **Bit Mangling:**

   * A sequence of operations (XOR, shifts, multiplies, adds) using handpicked “noisy” constants thoroughly diffuses every input bit.
4. **Result:**

   * The output is a deterministic, stateless, uniformly-distributed random number for that position/seed.

---

## **Statistical Goals**

A great procedural RNG must deliver:

* **Uniform distribution**: No bias in any bit, byte, or output range.
* **Bit independence**: Each bit behaves randomly.
* **No repetition**: Astronomically large period; practically unique for any coordinate/seed combo.
* **Determinism**: Same output for same inputs, everywhere.
* **Speed**: Billions of random numbers per second.
* **Low memory**: No tables or internal state.
* **Parallel safety**: 100% safe for multithreaded use.

---

## **Testing for Quality**

**For each RNG, check:**

* Average bits per output (`bits avg`)
* Per-bit entropy (`bits entropy`)
* Byte/fractional distribution (histograms)
* Modulo distribution (e.g., mod 10)
* Delta/increment between outputs
* Calls per millisecond

> For practical code and results, see [this test suite](#).

---

## **Crypto-Safe Needs?**

Noise-based RNGs **are not cryptographically secure**—they’re perfect for content, not secrets.
If you need strong randomness for things like GUIDs or save file encryption:

* Use a proven CSPRNG (ChaCha20, SHA-256, Blake3, etc).
* This can be slower—use only for critical, non-gameplay randomness.

**Recommendation:** Keep your fast procedural RNG and your crypto-safe RNG separate.

---

## **Portability**

* Use `Unity.Mathematics` in Unity/Burst, or just replace math functions for other engines (System.MathF, etc).
* The only dependency is a handful of well-chosen constants and simple integer math.

---

## **Implementation Examples**

### **1D Stateless Random**

```csharp
uint Noise1D(uint index, uint seed) { /* ...see above... */ }
```

### **2D/3D/4D Stateless Random**

```csharp
uint FlatIndex2D(int x, int y) => (uint)(x + y * 198491317);
uint Noise2D(int x, int y, uint seed) => Noise1D(FlatIndex2D(x, y), seed);
```

### **Normalized Float**

```csharp
float Random01(uint index, uint seed) => Noise1D(index, seed) / 4294967295.0f;
```

---

## **Summary: The Future is Noise**

> “With noise-based RNGs, you get fair, reproducible, fast, and parallel randomness for your worlds, levels, or procedural content. Once you go stateless, you’ll never go back!”

---

## **Further Reading**

* [2017 GDC “Math for Game Programmers” by Squirrel Eiserloh (YouTube)](https://www.youtube.com/watch?v=LWFzPP8ZbdU)
* [Wikipedia: Random number generation](https://en.wikipedia.org/wiki/Random_number_generation)

---

**Happy worldbuilding!** 🦄🌎
*— The Noise-Based RNG Community*

---