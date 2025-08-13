# 🛡️ Crypto-Safe RNG Integration with ChaCha in C# (Unity Friendly)

## What is This?

* **ChaCha20** is a cryptographically-secure, *deterministic* stream cipher—perfect for generating secure random numbers that are reproducible if you keep the key/nonce the same.
* With a CSPRNG like ChaCha20, you can safely generate unique tokens, save encryption keys, secret loot tables, or anything that requires “randomness” no one can predict.
* Unlike `System.Random`, *output depends only on the key, nonce, and counter*—no global state, no history required.
* **Usage:** Set up a state block, permute (mix) it, and read random values from the result.

---

## 🔑 Core ChaCha Constants

```csharp
const uint chaChaPrime1 = 0x61707865; // "expa"
const uint chaChaPrime2 = 0x3320646E; // "nd 3"
const uint chaChaPrime3 = 0x79622D32; // "2-by"
const uint chaChaPrime4 = 0x6B206574; // "te k"
```

---

## 🔄 ChaCha Quarter-Round and Permutation

```csharp
using System.Runtime.CompilerServices;

// 32-bit rotate left
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

// 64-bit version if needed
[MethodImpl(MethodImplOptions.AggressiveInlining)]
public static ulong RotateLeft(ulong value, int offset) => (value << offset) | (value >> (64 - offset));

// The core quarter-round
void QuarterRound(ref uint[] state, uint a, uint b, uint c, uint d)
{
    state[a] += state[b];
    state[d] ^= state[a];
    state[d] = RotateLeft(state[d], 16);

    state[c] += state[d];
    state[b] ^= state[c];
    state[b] = RotateLeft(state[b], 12);

    state[a] += state[b];
    state[d] ^= state[a];
    state[d] = RotateLeft(state[d], 8);

    state[c] += state[d];
    state[b] ^= state[c];
    state[b] = RotateLeft(state[b], 7);
}
```

---

## 🌀 ChaCha20 Block Permutation

```csharp
// Applies the quarter round to all columns
void ColumnRound(ref uint[] state)
{
    QuarterRound(ref state, 0, 4, 8, 12);
    QuarterRound(ref state, 1, 5, 9, 13);
    QuarterRound(ref state, 2, 6, 10, 14);
    QuarterRound(ref state, 3, 7, 11, 15);
}

// Applies the quarter round to all diagonals
void DiagonalRound(ref uint[] state)
{
    QuarterRound(ref state, 0, 5, 10, 15);
    QuarterRound(ref state, 1, 6, 11, 12);
    QuarterRound(ref state, 2, 7, 8, 13);
    QuarterRound(ref state, 3, 4, 9, 14);
}

// Full ChaCha20 block function (10 column/diagonal rounds = 20 rounds)
void Permute(ref uint[] state)
{
    for (int i = 0; i < 10; i++)
    {
        ColumnRound(ref state);
        DiagonalRound(ref state);
    }
}
```

---

## 🧩 State Initialization

**State Layout:**

```csharp
uint[] state = new uint[16]
{
    chaChaPrime1, chaChaPrime2, chaChaPrime3, chaChaPrime4,
    key0, key1, key2, key3,     // Your 256-bit key (as 8 × uint)
    key4, key5, key6, key7,     // ...
    ctr, nonce0, nonce1, nonce2 // Counter + 96-bit nonce (as 3 × uint)
};
```

* *Fill `key0..key7` and `nonce0..nonce2` from your world/secret seed and context.*
* *Start `ctr` at zero. Increment for new output blocks!*

---

## 🏃‍♂️ Generate Random Data

**To get a block of random data:**

```csharp
// 1. Copy the state to a working buffer
uint[] workingState = new uint[16];
Array.Copy(state, workingState, 16);

// 2. Permute (mix) the state
Permute(ref workingState);

// 3. Optionally, add original state to output (as ChaCha20 does)
for (int i = 0; i < 16; i++) workingState[i] += state[i];

// 4. Increment the counter for next call
if (++state[12] == 0) ++state[13]; // Handles counter rollover

// 5. Now workingState[0..15] is your "keystream" = 64 bytes random data
// Example: get one random uint32
uint randomValue = workingState[0];

// To get more, use workingState[1], [2], ..., [15], or run again for new block
```

**To get a random float in \[0,1):**

```csharp
float random01 = (workingState[0] & 0xFFFFFF) / (float)(1 << 24);
```

---

## 🗝️ Seed, Nonce, and Counter

* **Key:** Use your main seed, hashed or split into 8 × 32-bit words.
* **Nonce:** Use context, object id, or chunk coordinates to ensure unique streams.
* **Counter:** Use to "seek" further blocks, or always zero for single-use.

---

## 🚀 Usage in Procedural/Deterministic Context

**Example: Generate secure, reproducible randoms for each entity**

```csharp
// entityId: a unique number for this entity/chunk
// worldSeed: your game/secret seed, as 32 bytes

uint[] state = new uint[16]
{
    chaChaPrime1, chaChaPrime2, chaChaPrime3, chaChaPrime4,
    key0, key1, key2, key3,
    key4, key5, key6, key7,
    0, // Counter (start at 0)
    entityId, 0, 0 // Nonce (can be split as needed)
};

// Permute, extract values as above...
```

---

## 📝 TL;DR: When to Use

| Use Case        | RNG Recommendation    | Deterministic? | Crypto-Safe? |
| --------------- | --------------------- | :------------: | :----------: |
| Gameplay/world  | SquirrelNoise/xxHash  |       ✔️       |       ❌      |
| Secure token    | ChaCha20 (CSPRNG)     |      ✔️/❌      |      ✔️      |
| Save encryption | ChaCha20/AES          |      ✔️/❌      |      ✔️      |
| Procedural loot | Squirrel + ChaCha mix |       ✔️       |  ✔️ (hybrid) |

---

## 📚 References

* [musigma.blog — Exploring the ChaCha stream cipher](https://musigma.blog/2021/02/06/chacha.html)
* [RFC 8439 - ChaCha20 and Poly1305 for IETF protocols](https://datatracker.ietf.org/doc/html/rfc8439)
* [ChaCha20 C# implementation](https://github.com/mcraiha/CSharp-ChaCha20-NetStandard)
* [libsodium ChaCha20 documentation](https://libsodium.gitbook.io/doc/advanced/stream_ciphers/xchacha20)

---

### **This is now a cryptographically-safe, deterministic RNG—usable for procedural content, secure tokens, or encrypted saves.**

Seed it with your world/game/user key and a nonce per context, and enjoy crypto-grade reproducibility.

---