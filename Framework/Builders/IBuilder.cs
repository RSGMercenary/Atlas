using System;

namespace Atlas.Framework.Builders
{
	interface IBuilder<T> : IReadOnlyBuilder<T>
	{
		/// <summary>
		/// The current state of the build process. This can be
		/// unbuilt, building, or built.
		/// </summary>
		new BuildState BuildState { get; set; }

		void Built();

		bool AddBuilder(Action builder);
	}
}
