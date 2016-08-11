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
	}
}

