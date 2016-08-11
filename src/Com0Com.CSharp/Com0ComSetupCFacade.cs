using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Com0Com.CSharp
{
	public static class Com0comSetup
	{
		private readonly static string _com0comSetupc = @"C:\Program Files (x86)\com0com\setupc.exe";

		/// <summary>
		/// Get all com0com Port Pairs currently installed in the system.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<CrossoverPortPair> GetCrossoverPortPairs()
		{
			var ports = new List<CrossoverPortPair>();
			//get the output from setupc --detail-prms list
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _com0comSetupc,
					Arguments = "--detail-prms list",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};

			proc.Start();
			while (!proc.StandardOutput.EndOfStream)
			{
				try
				{
					string line = proc.StandardOutput.ReadLine();
					//get port number
					Regex regex = new Regex(@"(?<=CNC[A,B])\d+(?=\s)");
					int portnum = int.Parse(regex.Match(line).Value);

					CrossoverPortPair pair = ports.FirstOrDefault(d => d.PairNumber == portnum);

					if (pair == null)
					{
						pair = new CrossoverPortPair(portnum);
						ports.Add(pair);
					}
					regex = new Regex(@"(?<=CNC)[A,B](?=\d+\s)");
					string portAB = regex.Match(line).Value;
					if (portAB == "A")
					{
						pair.PortConfigStringA = line;
					}
					else if (portAB == "B")
					{
						pair.PortConfigStringB = line;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
			return ports;
		}

		public static void CreatePortPair()
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
					Arguments = "install - -",
					UseShellExecute = true,
					CreateNoWindow = false,
					Verb = "runas"
				}
			};
			proc.Start();

			//TODO: add a timeout here
			while (!proc.HasExited) { }

			if (proc.ExitCode != 0)
				throw new ApplicationException($"Could not add port pair.");
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

			//TODO: add a timeout here
			while (!proc.HasExited) { }

			if (proc.ExitCode != 0)
				throw new ApplicationException($"Could not delete port pair number {n}");
		}
	}
}

