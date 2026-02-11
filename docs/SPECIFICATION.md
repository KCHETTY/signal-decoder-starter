# Signal Decoder Technical Assessment - Specification

## Problem Overview

You are building a system that can identify which devices transmitted a signal based on the combined signal received. Multiple devices can transmit simultaneously, and their signals add together. Your job is to decode the combined signal and find all possible combinations of transmitting devices.

## The Challenge

Imagine you have N devices, each broadcasting a unique signal pattern (an array of integers). When multiple devices transmit at the same time, their signals combine by element-wise addition at each position.

**Example:**
- Device D01 transmits: `[2, 4, 1, 3]`
- Device D03 transmits: `[3, 6, 2, 8]`
- Device D05 transmits: `[5, 2, 3, 1]`
- Combined received signal: `[10, 12, 6, 12]` (sum of all three)

Your decoder must determine which devices were transmitting by analyzing the received signal.

## Core Requirements

### 1. Device Signal Generation

Create a service that generates random device signal patterns.

**Requirements:**
- Generate N devices with unique IDs (format: `D01`, `D02`, etc.)
- Each device has a signal pattern (array of integers)
- All patterns must have the same length
- Signal values are non-negative integers within a specified range

### 2. Signal Simulation

Create a service that simulates signal transmission.

**Requirements:**
- Accept a dictionary of devices with their signal patterns
- Randomly select 1 to N devices to be "active"
- Compute the combined signal (element-wise sum of active device patterns)
- Return the combined signal WITHOUT revealing which devices were active

### 3. Signal Decoding (Core Algorithm)

Create a service that decodes a received signal to identify transmitting devices.

**Requirements:**
- Accept a dictionary of all possible devices
- Accept the received (combined) signal
- Find ALL combinations of devices that could produce the received signal
- Support "tolerance" for fuzzy matching (allow signals to be off by ±tolerance at each position)
- Return all valid solutions with detailed information

**Algorithm Constraints:**
- Must handle up to 25 devices efficiently
- Performance requirements:
  - 10 devices: Complete in under 1 second
  - 15 devices: Complete in under 3 seconds
  - 20 devices: Complete in under 5 seconds

**Solution Requirements:**
Each solution must include:
- List of transmitting device IDs
- Dictionary of decoded signals (device ID → signal pattern)
- Computed sum (verification that devices sum to received signal)
- Whether the solution matches the received signal (within tolerance)

## Domain Models

You must implement these models in your `SignalDecoder.Domain` project:

### DecodeRequest
```csharp
public class DecodeRequest
{
    public Dictionary<string, int[]> Devices { get; set; }      // All available devices
    public int[] ReceivedSignal { get; set; }                   // Signal to decode
    public int Tolerance { get; set; } = 0;                     // Fuzzy match tolerance
}
```

### DecodeResult
```csharp
public class DecodeResult
{
    public List<string> TransmittingDevices { get; set; }       // Device IDs in solution
    public Dictionary<string, int[]> DecodedSignals { get; set; } // Device patterns
    public int[] ComputedSum { get; set; }                      // Sum of device patterns
    public bool MatchesReceived { get; set; }                   // Within tolerance?
}
```

### DecodeResponse
```csharp
public class DecodeResponse
{
    public List<DecodeResult> Solutions { get; set; }           // All valid solutions
    public int SolutionCount { get; set; }                      // Number of solutions
    public long SolveTimeMs { get; set; }                       // Time taken to solve
}
```

### SimulateRequest
```csharp
public class SimulateRequest
{
    public Dictionary<string, int[]> Devices { get; set; }      // Devices to simulate
}
```

### SimulateResponse
```csharp
public class SimulateResponse
{
    public int[] ReceivedSignal { get; set; }                   // Combined signal
    public int ActiveDeviceCount { get; set; }                  // How many transmitted
    public int SignalLength { get; set; }                       // Pattern length
    public int TotalDevices { get; set; }                       // Total devices available
}
```

## Service Interfaces

### IDeviceGeneratorService
```csharp
public interface IDeviceGeneratorService
{
    Dictionary<string, int[]> GenerateDevices(int count, int signalLength, int maxStrength);
}
```

### ISignalSimulatorService
```csharp
public interface ISignalSimulatorService
{
    SimulateResponse Simulate(Dictionary<string, int[]> devices);
}
```

### ISignalDecoderService
```csharp
public interface ISignalDecoderService
{
    DecodeResponse Decode(DecodeRequest request);
}
```

## Algorithm Approach

### Subset Sum Problem

This is a variant of the subset sum problem. You need to find all subsets of devices whose signal patterns sum to the received signal (within tolerance).

**Recommended Approach:**
1. Use backtracking with pruning
2. Early termination when sum exceeds target
3. Prune branches that can't possibly match
4. Track solve time for performance monitoring

**Key Optimizations:**
- Sort devices by signal magnitude (helps with pruning)
- Use early bounds checking at each position
- Avoid redundant computations
- Consider memoization for repeated subproblems

### Tolerance Handling

Tolerance allows fuzzy matching:
- `tolerance = 0`: Exact match required
- `tolerance = 1`: Allow ±1 difference at each position
- `tolerance = n`: Allow ±n difference at each position

A solution matches if: `|computed[i] - received[i]| ≤ tolerance` for all positions `i`

## Edge Cases to Handle

1. **Empty solution**: Received signal is all zeros (no devices transmitting)
2. **No solution**: No combination of devices produces the received signal
3. **Multiple solutions**: Different device combinations produce the same signal
4. **Single device**: Only one device in the pool
5. **All devices active**: Every device transmits simultaneously
6. **Large signal lengths**: Patterns with 10+ elements
7. **High tolerance**: Many fuzzy matches possible

## Validation Requirements

Your implementation must validate:
- All signal patterns have the same length
- Received signal matches pattern length
- Tolerance is non-negative
- Device dictionary is not empty
- Signal values are non-negative

## Testing Strategy

Your solution will be evaluated with:
- **Correctness tests**: Finding all valid solutions
- **Performance tests**: Meeting time constraints for 10, 15, 20 devices
- **Tolerance tests**: Proper fuzzy matching
- **Validation tests**: Handling edge cases and invalid input
- **Integration tests**: Full workflow (generate → simulate → decode)

## Implementation Requirements

### Project Structure

You must use clean architecture with these projects:

```
SignalDecoder/
├── src/
│   ├── SignalDecoder.Api/              # ASP.NET Core Web API (controllers)
│   ├── SignalDecoder.Domain/           # Models, interfaces
│   └── SignalDecoder.Application/      # Services, business logic
└── SignalDecoder.sln
```

### Technology Stack

- .NET 8.0
- ASP.NET Core Web API
- xUnit for testing (optional, for your own tests)
- Clean Architecture principles

### API Endpoints

See `API_CONTRACT.md` for the exact API specification you must implement.

## Success Criteria

Your implementation will be considered successful if:

1. ✅ All API endpoints work correctly
2. ✅ Decoder finds all valid solutions
3. ✅ Performance requirements are met
4. ✅ Tolerance handling works properly
5. ✅ Edge cases are handled correctly
6. ✅ Code follows clean architecture principles
7. ✅ All hidden assessment tests pass

## Tips for Success

1. **Start with the domain models** - Get your data structures right first
2. **Implement services incrementally** - Generator → Simulator → Decoder
3. **Test as you go** - Don't wait until the end to test
4. **Focus on the algorithm** - The decoder is the hardest part
5. **Optimize smartly** - Profile before optimizing
6. **Handle edge cases** - Empty signals, no solutions, single device, etc.
7. **Use proper abstractions** - Interfaces, dependency injection, clean code

## Time Estimate

This assessment typically takes 4-6 hours for experienced developers:
- Project setup: 30 minutes
- Domain models: 30 minutes
- Generator service: 30 minutes
- Simulator service: 30 minutes
- Decoder service: 2-3 hours (this is the core challenge)
- API controllers: 30 minutes
- Testing and debugging: 1 hour

## Questions?

If you have questions about the specification:
1. Review the API contract documentation
2. Check the README for submission instructions
3. Review the assessment guide for evaluation criteria

Good luck! 🚀
