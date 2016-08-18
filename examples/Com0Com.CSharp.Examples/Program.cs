﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com0Com.CSharp.Examples
{
    class Program
    {
        private static readonly Com0ComSetupCFacade SetupCFacade = new Com0ComSetupCFacade();

        static void Main(string[] args)
        {
            SyncExample();

            Console.WriteLine("\n***********************************\n");

            Task.Run(AsyncExample).Wait();
        }

        private static async Task AsyncExample()
        {
            Console.WriteLine("Async Example");
            Console.WriteLine("Pre-existing virtual crossover port pairs:");
            var preExistingPortPairs = await SetupCFacade.GetCrossoverPortPairsAsync(CancellationToken.None);
            foreach (var pp in preExistingPortPairs)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }
            Console.WriteLine();

            // Create some new virtual com port pairs
            var pp1 = await SetupCFacade.CreatePortPairAsync(CancellationToken.None);
            var pp2 = await SetupCFacade.CreatePortPairAsync("COM180", "COM181", CancellationToken.None);

            Console.WriteLine("Virtual crossover port pairs after creation:");
            var portPairsAfterCreation = await SetupCFacade.GetCrossoverPortPairsAsync(CancellationToken.None);
            foreach (var pp in portPairsAfterCreation)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }
            Console.WriteLine();

            // Remove the virtual com port pairs that we created
            await SetupCFacade.DeletePortPairAsync(pp1.PairNumber, CancellationToken.None);
            await SetupCFacade.DeletePortPairAsync(pp2.PairNumber, CancellationToken.None);

            Console.WriteLine("Virtual crossover port pairs after removal:");
            var portPairsAfterDelete = await SetupCFacade.GetCrossoverPortPairsAsync(CancellationToken.None);
            foreach (var pp in portPairsAfterDelete)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }

            Console.ReadLine();
        }

        private static void SyncExample()
        {
            Console.WriteLine("Sync Example");
            Console.WriteLine("Pre-existing virtual crossover port pairs:");
            var preExistingPortPairs = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in preExistingPortPairs)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }
            Console.WriteLine();

            // Create some new virtual com port pairs
            var pp1 = SetupCFacade.CreatePortPair();
            var pp2 = SetupCFacade.CreatePortPair("COM180", "COM181");

            Console.WriteLine("Virtual crossover port pairs after creation:");
            var portPairsAfterCreation = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in portPairsAfterCreation)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }
            Console.WriteLine();

            // Remove the virtual com port pairs that we created
            SetupCFacade.DeletePortPair(pp1.PairNumber);
            SetupCFacade.DeletePortPair(pp2.PairNumber);

            Console.WriteLine("Virtual crossover port pairs after removal:");
            var portPairsAfterDelete = SetupCFacade.GetCrossoverPortPairs();
            foreach (var pp in portPairsAfterDelete)
            {
                Console.WriteLine($"Virtual Port Pair: CNCA{pp.PairNumber}({pp.PortNameA}) <-> CNCB{pp.PairNumber}({pp.PortNameB})");
            }
        }
    }
}
