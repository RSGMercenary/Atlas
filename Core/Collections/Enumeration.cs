namespace Atlas.Core.Collections
{
	/// <summary>
	/// Base class for the type-safe enum pattern.
	/// </summary>
	class Enumeration
	{
		public string Name { get; } = "";
		public int Value { get; } = 0;

		protected Enumeration(int value, string name)
		{
			Value = value;
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		public static implicit operator string(Enumeration enumeration)
		{
			return enumeration.Name;
		}

		public static implicit operator int(Enumeration enumeration)
		{
			return enumeration.Value;
		}
	}
}
