using System;
using System.Collections.Generic;

namespace Com0Com.CSharp.Examples
{
    class Program
    {
        private static readonly Com0ComSetupCFacade SetupCFacade = new Com0ComSetupCFacade();

        static void Main(string[] args)
        {
            // Get all the pre existing virtual com port pairs
            var preExistingPortPairs = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in preExistingPortPairs)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }

            // Create some new virtual com port pairs
            var pp1 = SetupCFacade.CreatePortPair();
            var pp2 = SetupCFacade.CreatePortPair("COM180", "COM181");

            var portPairsAfterCreation = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in portPairsAfterCreation)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }

            // Remove the virtual com port pairs that we created
            SetupCFacade.DeletePortPair(pp1.PairNumber);
            SetupCFacade.DeletePortPair(pp2.PairNumber);

            var portPairsAfterDelete = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in portPairsAfterDelete)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }

            Console.ReadLine();
        }
    }
}
