using System;

namespace SignalDecoder.Domain.Models
{
    public class DecodeResponse
    {
        public List<DecodeResult>? Solutions { get; set; }           // All valid solutions
        public int SolutionCount { get; set; }                      // Number of solutions
        public long SolveTimeMs { get; set; }                       // Time taken to solve
    }
}