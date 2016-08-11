using System;
using System.Diagnostics;
using System.IO;
using Com0Com.CSharp.Configs;

namespace Com0Com.CSharp
{
	public class Com2TcpFacade
	{
		private readonly string _com2TcpPath;

		public Com2TcpFacade(string com2TcpPath = @"C:\Program Files (x86)\com0com\com2tcp.exe")
		{
			_com2TcpPath = com2TcpPath;
		}

		public void CreateCom2TcpLink(Com2TcpConfig config)
		{
			if (UacHelper.IsUacEnabled && !UacHelper.IsProcessElevated
				|| !UacHelper.IsAdministrator())
				throw new ApplicationException("This process must be run as an administrator.");

			var proc = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					WorkingDirectory = Path.GetDirectoryName(_com2TcpPath),
					FileName = _com2TcpPath,
					Arguments = $"--telnet \\\\.\\{config.ComPortNameToForward} {config.ForwardingEndpoint.Address.ToString()} {config.ForwardingEndpoint.Port}",
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

		// TODO: ADD METHOD TO remove a com2tcp link
		// TODO: HOW TO CLEAN UP AFTERWARDS?
	}
}

