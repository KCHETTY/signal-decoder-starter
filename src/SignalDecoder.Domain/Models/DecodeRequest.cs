using System;

namespace SignalDecoder.Domain.Models
{
    public class DecodeRequest
    {
        public required Dictionary<string, int[]> Devices { get; set; }      // All available devices
        public required int[] ReceivedSignal { get; set; }                   // Signal to decode
        public int Tolerance { get; set; } = 0;                     // Fuzzy match tolerance
    }
}
