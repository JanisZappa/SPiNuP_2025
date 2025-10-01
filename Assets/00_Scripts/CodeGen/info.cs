


/*[Flags]
public enum info
{
	IsItem         = 1 << 0,
	IsTrack        = 1 << 1, 
	IsFluff        = 1 << 2, 
	IsCollectable  = 1 << 3,
	
	Shake = 1 << 4,
	Grab  = 1 << 5,
	Hide  = 1 << 7,
	
	Button = 1 << 13,
	Action = 1 << 14
}


public struct Filter
{
	private readonly info anyMatch, mustMatch;
	
	private Filter(info anyMatch, info mustMatch)
	{
		this.anyMatch  = anyMatch;
		this.mustMatch = mustMatch;
	}
	

	public bool Matches(elementType elementType)
	{
		return (anyMatch  == 0 || elementType.AnyMatch(anyMatch)) && 
		       (mustMatch == 0 || elementType.Matches(mustMatch));
	}
	
	public bool Matches(info info)
	{
		return (anyMatch  == 0 || info.AnyMatch(anyMatch)) && 
		       (mustMatch == 0 || info.Matches(mustMatch));
	}

	public static bool operator == (Filter a, Filter b)
	{
		return a.anyMatch == b.anyMatch && a.mustMatch == b.mustMatch;
	}

	public static bool operator !=(Filter a, Filter b)
	{
		return !(a == b);
	}


	public static Filter IsItem, IsTrack, IsFluff, IsCollectable, CanBeGrabbed, AnyThing, MustShake, 
		                 ShakeItems, ShakeFluff, None, Action;
	
	static Filter()
	{
		IsItem        = new Filter(0, info.IsItem);
		IsTrack       = new Filter(0, info.IsTrack);
		IsFluff       = new Filter(0, info.IsFluff);
		IsCollectable = new Filter(0, info.IsCollectable);
		
		CanBeGrabbed  = new Filter(0, info.Grab);
		AnyThing      = new Filter(info.IsItem | info.IsFluff | info.IsCollectable, 0);
		MustShake     = new Filter(0, info.Shake);
		ShakeItems    = new Filter(0, info.IsItem | info.Shake);
		ShakeFluff    = new Filter(0, info.IsFluff | info.Shake);
		None          = new Filter(0, 0);
		Action        = new Filter(0, info.Action);
	}


	public override string ToString()
	{
		return "Any: " + anyMatch + " - Must: " + mustMatch;
	}
}*/
