using System;
using  SignalDecoder.Domain.Interfaces;
using  SignalDecoder.Domain.Models;

namespace SignalDecoder.Application.Service
{
    public class SignalDecoderService : ISignalDecoderService
    {
        private const int INDEX_DEFAULT = 0;
        private void RunDecode(int startindex, int target, int Tolerance, List<(string, int)> pairs, IList<(string, int)> Combo, IList<IList<(string, int)>> records)
        {
            if (0 == target)
            {
                records.Add(Combo);
            }
            else
            {
                int lastvalue = 0;
                for (int itr = startindex; itr < pairs.Count; itr++)
                {
                    int Value = pairs[itr].Item2;
                    if (pairs[itr].Item2 > target)
                    {
                        break ;
                    }

                    if (Value != lastvalue)
                    {
                        lastvalue = Value;
                        Combo.Add((pairs[itr].Item1, lastvalue));
                        RunDecode(itr + 1, target - lastvalue, Tolerance, pairs, Combo.ToList(), records);
                        Combo.RemoveAt(Combo.Count - 1);
                    }
                }
            }
        }

        public DecodeResponse Decode(DecodeRequest request)
        {

            IList<IList<(string, int)>> ValidMatches = new List<IList<(string, int)>>();

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

            RunDecode(INDEX_DEFAULT, Sum, request.Tolerance, pairs, new List<(string, int)>(), ValidMatches);

            List<DecodeResult> SolutionsRet = [];

            List<string> lst = [];
            Dictionary<string, int[]> dict = [];

            foreach (var Outer in ValidMatches)
            {
                 foreach (var (DeviceId, Value) in Outer)
                {
                    dict.Add(DeviceId, request.Devices[DeviceId]);
                    lst.Add(DeviceId);
                }

                int[] SummedDeviceArr = new int [dict.First().Value.Length];
                foreach (KeyValuePair<string, int[]> device in dict)
                {
                    SummedDeviceArr = SummedDeviceArr.Zip(device.Value, (a, b) => a + b).ToArray();
                }

                SolutionsRet.Add(new DecodeResult { TransmittingDevices = lst, DecodedSignals = dict, ComputedSum = SummedDeviceArr, MatchesReceived = false });
            }

            return new DecodeResponse
            {
                Solutions = SolutionsRet,
                SolutionCount = 1,
                SolveTimeMs = 1
            };
        }
    }
}

