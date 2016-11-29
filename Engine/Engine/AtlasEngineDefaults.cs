using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using System;

namespace Atlas.Engine.Engine
{
	static class AtlasEngineDefaults
	{
		public static Type DefaultEntity = typeof(AtlasEntity);
		public static Type DefaultFamily = typeof(AtlasFamily);

		public static int DefaultEntityPoolCapacity = 100;
		public static int DefaultFamilyPoolCapacity = 20;
	}
}
