using System.Net;

namespace Com0Com.CSharp
{
	public class Com2TcpLink
	{
		public Com2TcpLink(string comPortNameToFoward, IPEndPoint forwardingEndpoint)
		{
			ComPortNameToForward = comPortNameToFoward;
			ForwardingEndpoint = forwardingEndpoint;
		}
		public string ComPortNameToForward { get; }
		public IPEndPoint ForwardingEndpoint { get; }
	}
}

