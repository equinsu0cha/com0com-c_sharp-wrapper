namespace Com0Com.CSharp.Configs
{
	public class Com0ComConfig
	{
		public Com0ComConfig(string comPortNameA, string comPortNameB)
		{
			ComPortNameA = comPortNameA;
			ComPortNameB = comPortNameB;
		}

		public string ComPortNameA { get; }
		public string ComPortNameB { get; }
	}
}
