namespace Atlas.Engine.Collections
{
	/// <summary>
	/// Base class for the type-safe enum pattern.
	/// </summary>
	class Enumeration
	{
		private readonly int value = 0;
		private readonly string name = "";

		protected Enumeration(int value, string name)
		{
			this.value = value;
			this.name = name;
		}

		public int Value
		{
			get { return value; }
		}

		public string Name
		{
			get { return name; }
		}

		public override string ToString()
		{
			return name;
		}

		public static implicit operator string(Enumeration enumeration)
		{
			return enumeration.name;
		}

		public static implicit operator int(Enumeration enumeration)
		{
			return enumeration.value;
		}
	}
}
