using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Com0Com.CSharp
{
	public static class Com0ComSetupCFacade
	{
		private readonly static string _com0comSetupc = @"C:\Program Files (x86)\com0com\setupc.exe";

		/// <summary>
		/// Get all com0com Port Pairs currently installed in the system.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CrossoverPortPair> GetCrossoverPortPairs()
		{
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _com0comSetupc,
					Arguments = "list",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};

			proc.Start();

			var lines = new List<string>();
			while (!proc.StandardOutput.EndOfStream)
			{
				lines.Add(proc.StandardOutput.ReadLine());
			}

			return ParsePortPairsFromStdOut(lines);
		}

		public static CrossoverPortPair CreatePortPair(string comPortNameA, string comPortNameB)
		{
			if (UacHelper.IsUacEnabled && !UacHelper.IsProcessElevated
			    || !UacHelper.IsAdministrator())
				throw new ApplicationException("This process must be run as an administrator.");

			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					WorkingDirectory = Path.GetDirectoryName(_com0comSetupc),
					FileName = _com0comSetupc,
					Arguments = $"install portname={comPortNameA} portname={comPortNameB}",
					UseShellExecute = true,
					CreateNoWindow = false,
					Verb = "runas"
				}
			};
			proc.Start();

			var lines = new List<string>();
			while (!proc.StandardOutput.EndOfStream)
			{
				lines.Add(proc.StandardOutput.ReadLine());
			}

			return ParsePortPairsFromStdOut(lines).First();
		}

		public static void DeletePortPair(int n)
		{
			if (UacHelper.IsUacEnabled && !UacHelper.IsProcessElevated
				|| !UacHelper.IsAdministrator())
				throw new ApplicationException("This process must be run as an administrator.");

			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					WorkingDirectory = Path.GetDirectoryName(_com0comSetupc),
					FileName = _com0comSetupc,
					Arguments = $"remove {n}",
					UseShellExecute = true,
					CreateNoWindow = false,
					Verb = "runas"
				}
			};
			proc.Start();

			while (!proc.HasExited) { }

			if (proc.ExitCode != 0)
				throw new ApplicationException($"Could not delete port pair number {n}");
		}

		private static IEnumerable<CrossoverPortPair> ParsePortPairsFromStdOut(IEnumerable<string> lines)
		{
			var portAMap = new Dictionary<int, string>();
			var portBMap = new Dictionary<int, string>();

			foreach (var line in lines)
			{
				var lineRegex = new Regex(@"^CNC([A,B])(\d+)\sPortName=(\w+)");
				var match = lineRegex.Match(line);
				if (match == null) continue;

				var portNum = Convert.ToInt32(match.Groups[1].Value);
				var aOrB = match.Groups[0].Value;
				var portName = match.Groups[2].Value;

				if (aOrB == "A")
				{
					portAMap.Add(portNum, portName);
				}
				else
				{
					portBMap.Add(portNum, portName);
				}
			}

			var ret = new List<CrossoverPortPair>();
			for (var i = 0; i < portAMap.Keys.Count; i++)
			{
				ret.Add(new CrossoverPortPair(portBMap[i], portBMap[i], i));
			}

			return ret;
		}
	}

}

