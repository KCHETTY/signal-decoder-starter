using System;

namespace SignalDecoder.Domain.Models
{
    public class DecodeResult
    {
        public required List<string> TransmittingDevices { get; set; }       // Device IDs in solution
        public required Dictionary<string, int[]> DecodedSignals { get; set; } // Device patterns
        public required int[] ComputedSum { get; set; }                      // Sum of device patterns
        public bool MatchesReceived { get; set; }                   // Within tolerance?
    }
}