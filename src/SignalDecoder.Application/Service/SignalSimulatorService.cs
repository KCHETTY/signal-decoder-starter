using System;
using SignalDecoder.Domain.Interfaces;
using SignalDecoder.Domain.Models;

namespace SignalDecoder.Application.Service
{
    public class SignalSimulatorService : ISignalSimulatorService
    {
        public SimulateResponse Simulate(Dictionary<string, int[]> devices)
        {
            int DeviceCount = devices.Count;
            int RandomInt = new Random().Next(1, DeviceCount + 1);

            int inSignalLength = devices.First().Value.Length;

            List<int> RandomNumberList = Enumerable.Range(0, DeviceCount)
                                          .OrderBy(_ => new Random().Next())
                                          .Take(RandomInt)
                                          .ToList();

            int Itr = 0;
            int[] SummedDeviceArr = new int [inSignalLength];
            foreach (KeyValuePair<string, int[]> device in devices)
            {
                if (RandomNumberList.Contains(Itr++))
                {
                    SummedDeviceArr = SummedDeviceArr.Zip(device.Value, (a, b) => a + b).ToArray();
                }
            }

            return new SimulateResponse
            {
                ReceivedSignal = SummedDeviceArr,
                ActiveDeviceCount = RandomInt,
                TotalDevices = devices.Count,
                SignalLength = inSignalLength  
            };
        }
    }
}
