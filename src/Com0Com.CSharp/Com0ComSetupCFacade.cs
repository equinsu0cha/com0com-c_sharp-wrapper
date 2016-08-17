﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Com0Com.CSharp
{
	public class Com0ComSetupCFacade
	{
	    private readonly ICmdRunner _cmdRunner;
	    private readonly string _com0ComSetupC;

        /// <summary>
        /// Create a setupc.exe facade using the default implementation of ICmdRunner
        /// </summary>
        /// <param name="com0ComSetupC"></param>
        public Com0ComSetupCFacade(string com0ComSetupC = @"C:\Program Files (x86)\com0com\setupc.exe")
        {
            _cmdRunner = new CmdRunner();
            _com0ComSetupC = com0ComSetupC;
        }

        /// <summary>
        /// Create a setupc.exe facade with your own implementation of ICmdRunner
        /// </summary>
        /// <param name="cmdRunner"></param>
        /// <param name="com0ComSetupC"></param>
		public Com0ComSetupCFacade(ICmdRunner cmdRunner, string com0ComSetupC = @"C:\Program Files (x86)\com0com\setupc.exe")
		{
		    _cmdRunner = cmdRunner;
			_com0ComSetupC = com0ComSetupC;
		}

        /// <summary>
        /// Get all com0com null-modem connections installed on the system
        /// </summary>
        /// <returns>All com0com null-modem connections installed on the system</returns>
        public IEnumerable<CrossoverPortPair> GetCrossoverPortPairs()
		{
            if (!IsElevatedOrAdmin())
                throw new ApplicationException("This process must be run as an administrator.");

		    var stdOutLines = _cmdRunner.RunCommandGetStdOut(
                Path.GetDirectoryName(_com0ComSetupC), 
                _com0ComSetupC, 
                "list");
            
			return ParsePortPairsFromStdOut(stdOutLines.Select(l => l.Trim()));
		}

        /// <summary>
        /// Create a com0com null-modem connection between virtual com ports with default names
        /// </summary>
        /// <returns>The created virtual port pair</returns>
        public CrossoverPortPair CreatePortPair()
	    {
            if (!IsElevatedOrAdmin())
                throw new ApplicationException("This process must be run as an administrator.");

            var stdOutLines = _cmdRunner.RunCommandGetStdOut(
                Path.GetDirectoryName(_com0ComSetupC), 
                _com0ComSetupC, 
                "install - -");

            return ParsePortPairsFromStdOut(stdOutLines.Select(l => l.Trim())).First();
        }

	    /// <summary>
	    /// Create a com0com null-modem connection between virtual com ports with specified names
	    /// </summary>
	    /// <param name="comPortNameA">The name of virtual com port A</param>
	    /// <param name="comPortNameB">The name of virtual com port B</param>
	    /// <returns>The created virtual port pair</returns>
	    public CrossoverPortPair CreatePortPair(string comPortNameA, string comPortNameB)
		{
			if (!IsElevatedOrAdmin())
				throw new ApplicationException("This process must be run as an administrator.");

            var stdOutLines = _cmdRunner.RunCommandGetStdOut(
                Path.GetDirectoryName(_com0ComSetupC),
                _com0ComSetupC,
                $"install portname={comPortNameA} portname={comPortNameB}");

			return ParsePortPairsFromStdOut(stdOutLines.Select(l => l.Trim())).First();
		}

        /// <summary>
        /// Remove a com0com null-modem connection and the two virtual com ports associated with the connection
        /// </summary>
        /// <param name="n"></param>
		public void DeletePortPair(int n)
		{
			if (!IsElevatedOrAdmin())
				throw new ApplicationException("This process must be run as an administrator.");

            _cmdRunner.RunCommandGetStdOut(
                Path.GetDirectoryName(_com0ComSetupC),
                _com0ComSetupC,
                $"remove {n}");
		}

	    private bool IsElevatedOrAdmin()
	    {
            return (UacHelper.IsUacEnabled && !UacHelper.IsProcessElevated) || !UacHelper.IsAdministrator();
        }

		private IEnumerable<CrossoverPortPair> ParsePortPairsFromStdOut(IEnumerable<string> lines)
		{
			var portAMap = new Dictionary<int, string>();
			var portBMap = new Dictionary<int, string>();
            var lineRegex = new Regex(@"^CNC([AB])(\d+)\sPortName=(-|\w+)");

            foreach (var line in lines)
			{
				var match = lineRegex.Match(line);
				if (!match.Success) continue;

				var aOrB = match.Groups[1].Value;
				var portNum = Convert.ToInt32(match.Groups[2].Value);
				var portName = match.Groups[3].Value;

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
		    foreach (var key in portAMap.Keys)
		    {
                ret.Add(new CrossoverPortPair(portAMap[key],portBMap[key], key));
            }

			return ret;
		}
	}

}

