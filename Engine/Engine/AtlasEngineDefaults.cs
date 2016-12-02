using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Engine
{
	static class AtlasEngineDefaults
	{
		/// <summary>
		/// If <see cref="InstanceWithEntity"/> is true when <see cref="AtlasEngine.Instance"/>
		/// is first called, then an <see cref="IEntity"/> of type <see cref="DefaultEntity"/>
		/// will automatically be added as its <see cref="Components.IComponent.Manager"/>.
		/// </summary>
		public static bool InstanceWithEntity = false;

		/// <summary>
		/// The default <see cref="IEntity"/> type of the <see cref="IEngine"/>. Entities pooled,
		/// unpooled, or instantiated through <see cref="IEngine.GetEntity(bool, string, string)"/>
		/// will be of this type.
		/// </summary>
		public static Type DefaultEntity = typeof(AtlasEntity);

		/// <summary>
		/// The default <see cref="IFamily"/> type of the <see cref="IEngine"/>. Families pooled,
		/// unpooled, or instantiated through <see cref="IEngine.AddFamily(Type)"/>
		/// will be of this type.
		/// </summary>
		public static Type DefaultFamily = typeof(AtlasFamily);

		/// <summary>
		/// The default pool capacity of <see cref="IEngine.EntityPool"/> when
		/// <see cref="AtlasEngine.Instance"/> is first called. The pool capacity
		/// can be manually changed afterwards.
		/// </summary>
		public static int DefaultEntityPoolCapacity = 100;

		/// <summary>
		/// The default pool capacity of <see cref="IEngine.FamilyPool"/> when
		/// <see cref="AtlasEngine.Instance"/> is first called. The pool capacity
		/// can be manually changed afterwards.
		/// </summary>
		public static int DefaultFamilyPoolCapacity = 20;
	}
}
