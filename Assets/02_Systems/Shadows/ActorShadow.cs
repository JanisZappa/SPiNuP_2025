using UnityEngine;

public class ActorShadow : Shadow
{
    [Space(10)] 
    public Actor actor;
    
    protected Vector2 depthOffset;

    
    public void DepthShadowUpdate(Vector2 depthOffset)
    {
        this.depthOffset = depthOffset;
        
        ShadowUpdate(true);
    }
}
