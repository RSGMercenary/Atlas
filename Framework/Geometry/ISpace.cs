namespace Atlas.Framework.Geometry
{
	interface ISpace
	{
		IChildVector3<ISpace> Position { get; }
		IChildVector3<ISpace> Rotation { get; }
		IChildVector3<ISpace> Scale { get; }
		IChildVector3<ISpace> Skew { get; }
	}
}
