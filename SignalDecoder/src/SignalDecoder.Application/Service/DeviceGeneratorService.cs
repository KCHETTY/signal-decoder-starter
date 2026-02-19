using System;
using SignalDecoder.Domain.Interfaces;

namespace SignalDecoder.Application.Service
{
    public class DeviceGeneratorService : IDeviceGeneratorService
    {
        public Dictionary<string, int[]> GenerateDevices(int count, int signalLength, int maxStrength)
        {
            Random rand = new Random();

            var DeviceDictionary = Enumerable.Range(1, count)
                             .ToDictionary(
                                i => $"D{i:D2}",                                  // Key: formatted string
                                i => Enumerable.Range(1, signalLength)
                                .Select(_ => rand.Next(0, maxStrength + 1))       // Value: int[] of numbers
                                .ToArray()
                            );

            return DeviceDictionary;
        }
    }
}
