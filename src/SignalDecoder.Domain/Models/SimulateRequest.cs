using System;

namespace SignalDecoder.Domain.Models
{
    public class SimulateRequest
    {
        public required Dictionary<string, int[]> Devices { get; set; }      // Devices to simulate
    }
}
