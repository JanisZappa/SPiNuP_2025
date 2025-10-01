public struct Side
{
    public static readonly Side Front = new Side(true);
    public static readonly Side Back = new Side(false);
    
    public Side(bool front){ this.front = front; }
    public Side(sbyte sign){ front = sign == -1; }
    
    public readonly bool front;
    
    public float  Sign      { get { return front ? -1 : 1; }}
    public bool   IsCamSide { get { return this == GameCam.CurrentSide; }}
    
    
    public Side Opposite { get { return new Side(!front);}}
   
    #region Equality
    
    public static bool operator == (Side one, Side two)
    {
        return one.front == two.front;
    }
    
    
    public static Side operator !(Side old)
    {
        return new Side(!old.front);
    }
      
    
    public static bool operator != (Side one, Side two)
    {
        return one.front != two.front;
    }


    private bool Equals(Side other)
    {
        return other.front == front;
    }

    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) 
            return false;
        
        return obj is Side && Equals((Side) obj);
    }

    
    public override int GetHashCode()
    {
        return 0;
    }


    public override string ToString()
    {
        return front ? "Side-Front" : "Side_Back";
    }

    #endregion
}