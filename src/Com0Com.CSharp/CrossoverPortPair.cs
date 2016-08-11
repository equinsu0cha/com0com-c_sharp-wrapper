using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

namespace Com0Com.CSharp
{
	public class CrossoverPortPair
	{
		public CrossoverPortPair(string portNameA, string portNameB, int number)
		{
			PortNameA = portNameA;
			PortNameB = portNameB;
			PairNumber = number;
		}

		public string PortNameA { get; }
		public string PortNameB { get; }
		public int PairNumber { get; }

		/// <summary>
		/// Kill a process, and all of its children, grandchildren, etc.
		/// </summary>
		/// <param name="pid">Process ID.</param>
		private static void KillProcessAndChildren(int pid)
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher
			  ("Select * From Win32_Process Where ParentProcessID=" + pid);
			ManagementObjectCollection moc = searcher.Get();
			foreach (ManagementObject mo in moc)
			{
				KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
			}
			try
			{
				Process proc = Process.GetProcessById(pid);
				proc.Kill();
			}
			catch
			{
				//we might get exceptions here, as parent might auto exit once their children are terminated
			}
		}

		public void StartComms()
		{
			if (CommsStatus == CommsStatus.Running)
				return;
			string program = "";
			string arguments = "";

			switch (CommsMode)
			{
				case CommsMode.RFC2217:
					program = "com2tcp-rfc2217.bat";
					arguments = string.Format("\\\\.\\{0} {1} {2}", PortNameB, RemoteIP, RemotePort);
					break;
				case CommsMode.TCPClient:
					program = "com2tcp.exe";
					arguments = string.Format("\\\\.\\{0} {1} {2}", PortNameB, RemoteIP, RemotePort);
					break;
				case CommsMode.UDP:
					program = "com2tcp.exe";
					arguments = string.Format("--udp \\\\.\\{0} {1} {2} {3}", PortNameB, RemoteIP, RemotePort, LocalPort);
					break;

			}

			_p = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = @"C:\Program Files (x86)\com0com\" + program,
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			_p.EnableRaisingEvents = true;
			_p.Exited += _p_Exited;
			_p.OutputDataReceived += _p_OutputDataReceived;
			_p.ErrorDataReceived += _p_ErrorDataReceived;

			OutputData = "";
			_p.Start();
			_p.BeginOutputReadLine();
			_p.BeginErrorReadLine();

			CommsStatus = CommsStatus.Running;
		}

		public void StopComms()
		{
			if (_p == null)
			{
				CommsStatus = CommsStatus.Idle;
				return;
			}
			if (_p.HasExited)
				return;
			KillProcessAndChildren(_p.Id);
		}

		private void _p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			OutputData += e.Data + Environment.NewLine;
		}

		private void _p_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			OutputData += e.Data + Environment.NewLine;
		}

		private void _p_Exited(object sender, EventArgs e)
		{
			CommsStatus = CommsStatus.Idle;
		}

		#region INotifyPropertyChangedMembers
		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.VerifyPropertyName(propertyName);

			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}

		public void VerifyPropertyName(string propertyName)
		{
			//Verify that the property name matches a real,
			//public instance property on this object
			//an empty property name is ok, used to refresh all properties
			if (string.IsNullOrEmpty(propertyName))
			{
				return;
			}
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				Debug.Fail("Invalid property name: " + propertyName);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


	}
}

