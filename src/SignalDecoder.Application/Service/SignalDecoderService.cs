using System;
using System.Diagnostics;
using  SignalDecoder.Domain.Interfaces;
using  SignalDecoder.Domain.Models;

namespace SignalDecoder.Application.Service
{
    public class SignalDecoderService : ISignalDecoderService
    {
        private const int INDEX_DEFAULT = 0;
        private struct DeviceStruct
        {
            public DeviceStruct(List<(string, int)> _DeviceList, Boolean _IsMatch)
            {
                DeviceList = _DeviceList;
                IsMatch = _IsMatch;
            }

            public List<(string, int)> DeviceList { get; }
            public Boolean IsMatch { get; }
        }

        private void RecursiveDecode(int startindex, int target, int Tolerance, List<(string, int)> Device, IList<(string, int)> ListTree, IList<DeviceStruct> DeviceList)
        {
            if (0 == (target - Tolerance))
            {
                DeviceList.Add(new DeviceStruct(ListTree.ToList(), true));
            }
            else if (target <= Tolerance)
            {
                DeviceList.Add(new DeviceStruct(ListTree.ToList(), false));
            }
            else
            {
                int lastvalue = 0;
                for (int itr = startindex; itr < Device.Count; itr++)
                {
                    int Value = Device[itr].Item2;
                    if (Device[itr].Item2 > target)
                    {
                        break ;
                    }

                    if (Value != lastvalue)
                    {
                        lastvalue = Value;
                        ListTree.Add((Device[itr].Item1, lastvalue));
                        RecursiveDecode(itr + 1, target - lastvalue, Tolerance, Device, ListTree.ToList(), DeviceList);
                        ListTree.RemoveAt(ListTree.Count - 1);
                    }
                }
            }
        }

        public DecodeResponse Decode(DecodeRequest request)
        {
            IList<DeviceStruct> ValidMatches = new List<DeviceStruct>();

            List<(string, int)> pairs = new List<(string, int)>();

            foreach (KeyValuePair<string, int[]> device in request.Devices)
            {
                int SumItem = 0;
                foreach (int num in device.Value)
                {
                    SumItem += num;
                }
                
                pairs.Add((device.Key, SumItem));
            }

            pairs.Sort((s1, s2) => s1.Item2.CompareTo(s2.Item2));

            int Sum = 0;
            foreach (int Item in request.ReceivedSignal)
            {
                Sum += Item;
            }

            Stopwatch sw = Stopwatch.StartNew();

            RecursiveDecode(INDEX_DEFAULT, Sum + request.Tolerance, request.Tolerance, pairs, new List<(string, int)>(), ValidMatches);

            sw.Stop();

            List<DecodeResult> SolutionsRet = [];

            foreach (DeviceStruct devicedata in ValidMatches)
            {
                List<string> lst = [];
                Dictionary<string, int[]> dict = [];
                foreach (var (DeviceId, Value) in devicedata.DeviceList)
                {
                    dict.Add(DeviceId, request.Devices[DeviceId]);
                    lst.Add(DeviceId);
                }

                int[] SummedDeviceArr = new int [dict.First().Value.Length];
                foreach (KeyValuePair<string, int[]> device in dict)
                {
                    SummedDeviceArr = SummedDeviceArr.Zip(device.Value, (a, b) => a + b).ToArray();
                }

                SolutionsRet.Add(new DecodeResult { TransmittingDevices = lst, DecodedSignals = dict, ComputedSum = SummedDeviceArr, MatchesReceived = devicedata.IsMatch });
            }

            return new DecodeResponse
            {
                Solutions = SolutionsRet,
                SolutionCount = SolutionsRet.Count,
                SolveTimeMs = sw.ElapsedMilliseconds
            };
        }
    }
}

