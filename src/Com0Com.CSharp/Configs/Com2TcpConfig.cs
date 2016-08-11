using System.Net;

namespace Com0Com.CSharp.Configs
{
	public class Com2TcpConfig
	{
		public Com2TcpConfig(string readWriteComPortName, string comPortNameToFoward, IPEndPoint forwardingEndpoint)
		{
			ReadWriteComPortName = readWriteComPortName;
			ComPortNameToForward = comPortNameToFoward;
			ForwardingEndpoint = forwardingEndpoint;
		}

		public string ReadWriteComPortName { get; }
		public string ComPortNameToForward { get; }
		public IPEndPoint ForwardingEndpoint { get; }
	}
}
