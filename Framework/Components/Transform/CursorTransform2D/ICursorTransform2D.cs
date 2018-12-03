namespace Atlas.Framework.Components.Transform
{
	/// <summary>
	/// ICursorTransform2D is a special Transform that allows the Cursor
	/// Entity to follow the mouse, regardless of where the Cursor is in
	/// the Entity hierarchy.
	/// 
	/// <para>When not disabled, the Entity will follow the mouse's position and always
	/// retain a constant global rotation. When disabled, the Cursor will follow the
	/// Transform rules of the hierarchy its in. This is useful for disabling user control
	/// and letting something else "script" the Cursor's movements.</para>
	/// </summary>
	public interface ICursorTransform2D : ITransform2D
	{
		bool IsDisabled { get; set; }
		int Disabled { get; }
	}
}
