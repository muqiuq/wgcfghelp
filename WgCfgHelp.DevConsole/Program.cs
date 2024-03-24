using System.Globalization;
using System.Net;
using CsvHelper;
using WgCfgHelp.Lib.IPHelpers;

namespace WgCfgHelp.DevConsole
{
    internal class Program
    {
        public const int NumOfExamplesToGenerate = 10000;

        static void Main(string[] args)
        {

            var randomIpSet = GetRandomIpSet(NumOfExamplesToGenerate);
            WriteListToFile("randomIpSets.csv", randomIpSet);

            WriteListToFile("randomMaskSet.csv", GetRandomMaskSet(NumOfExamplesToGenerate));

            var randomSplitSets = new List<SubnetSplit>();
            randomSplitSets.AddRange(GetRandomSubnetSplits(NumOfExamplesToGenerate, 2));
            randomSplitSets.AddRange(GetRandomSubnetSplits(NumOfExamplesToGenerate, 3));
            WriteListToFile("randomSplits.csv", randomSplitSets);


        }

        static List<SubnetSplit> GetRandomSubnetSplits(int size, byte splits)
        {
            var outList = new List<SubnetSplit>();
            for (int a = 0; a < size; a++)
            {
                var subNetSet = GetRandomSubnet(min: 26);
                var ipNet = subNetSet.Item2;
                var subnets = ipNet.Subnet((byte)(ipNet.Cidr + splits)).ToList();
                outList.Add(new SubnetSplit()
                {
                    SourceSubnet = ipNet.ToString(),
                    NumOfSplits = ((int)Math.Pow(2, splits)),
                    FirstResultingSubnet = subnets.First().ToString(),
                    SecondResultingSubnet = subnets.ElementAtOrDefault(1)?.ToString() ?? "",
                    ThirdResultingSubnet = subnets.ElementAtOrDefault(2)?.ToString() ?? "",
                    LastResultingSubnet = subnets.Last().ToString(),
                });
            }
            return outList;
        }

        static void WriteListToFile<T>(string filename, List<T> inList)
        {
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
                csv.WriteRecords(inList);
            }
        }

        static List<MaskSet> GetRandomMaskSet(int size)
        {
            var outList = new List<MaskSet>();
            var random = new Random();
            for (int a = 0; a < size; a++)
            {
                var cidr = random.Next(0, 32+1);
                var ipNet = IPNetwork2.Parse($"0.0.0.0/{cidr}");
                outList.Add(new MaskSet()
                {
                    Binary = CidrToBinary(cidr),
                    CIDR = cidr,
                    Dez = ipNet.Netmask.ToString()
                });
            }
            return outList;
        }

        static List<IpSet> GetRandomIpSet(int size)
        {
            var outList = new List<IpSet>();
            for (int n = 0; n < size; n++)
            {
                var randomAddressSet = GetRandomSubnet();
                var ipNet = randomAddressSet.Item2;
                var ipSet = new IpSet()
                {
                    Broadcast = ipNet.Broadcast.ToString(),
                    First = ipNet.FirstUsable.ToString(),
                    Hosts = ipNet.Usable.ToString(),
                    Last = ipNet.LastUsable.ToString(),
                    Mask = ipNet.Netmask.ToString(),
                    Network = ipNet.Network.ToString(),
                    Random1 = randomAddressSet.Item1[0].ToString(),
                    Random2 = randomAddressSet.Item1[1].ToString(),
                    Random3 = randomAddressSet.Item1[2].ToString(),
                    Cidr = ipNet.Cidr.ToString(),
                    Subnet = ipNet.ToString()

                    
                };

                outList.Add(ipSet);
            }
            return outList;
        }

        static Tuple<List<IPAddress>, IPNetwork2> GetRandomSubnet(int max = 20, int min = 28)
        {
            var rand = new Random();
            IPNetwork2? randomSubnet = null;
            List<IPAddress> randomAddresses = new List<IPAddress>();
            if (min > 28) min = 28;
            if (max < 8) max = 8;
            while (randomSubnet == null 
                   || !randomAddresses.Any()
                   || randomAddresses.Any(r => randomSubnet.Network.Equals(r))
                   || randomAddresses.Any(r => randomSubnet.Broadcast.Equals(r)))
            {
                randomAddresses.Clear();
                var randomAddressStr = $"10.{rand.Next(0, 255)}.{rand.Next(0, 255)}.{rand.Next(0, 255)}";
                var randomNetworkStr = $"{randomAddressStr}/{rand.Next(max, min + 1)}";
                var randomAddress = IPAddress.Parse(randomAddressStr);
                randomSubnet = IPNetwork2.Parse(randomNetworkStr);
                randomAddresses.Add(randomAddress);
                while (randomAddresses.Count < 6)
                {
                    var newRandomAddress = IpHelper.GetNextIpAddress(randomAddress, (uint)rand.Next(1, (int)randomSubnet.Usable));
                    if(!randomAddresses.Contains(newRandomAddress)) randomAddresses.Add(newRandomAddress);
                }
            }

            return new Tuple<List<IPAddress>, IPNetwork2>(randomAddresses, randomSubnet);
        }



        static string CidrToBinary(int cidr) => 
            string.Join("", Enumerable.Range(1, 32)
                .Select(i => i <= cidr ? "1" : "0")
                .Select((c, i) => (i % 4 == 0 && i != 0 ? " " : "") + c));
    }
}
