using LevelElements;


public static class LevelDevFilter
{
	private static readonly BoolSwitch LoadCollectables = new("Level/Show Collectables", true);
	private static readonly BoolSwitch LoadFlowers      = new("Level/Show Flowers", true);
	
	
	public static bool ShowThis(this elementType value)
	{
		if (!LoadCollectables && Mask.IsCollectable.Fits(value))
			return false;

		return LoadFlowers || !Mask.IsFluff.Fits(value); //!Mask.HidingShakeFluff.Fits(value) && !Mask.ShakeFluff.Fits(value));
	}
}
