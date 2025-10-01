using Clips;
using LevelElements;
using UnityEngine;


public class Coin : Actor
{
    public const float CollectionLength = .135f;
    
    private Quaternion itemRot;

    
    public override void SetItem(Item item)
    {
        base.SetItem(item);
        
        if(item != null)
            itemRot = Quaternion.AngleAxis(Mth.SmoothPP(-50, 50, item.ID * 13.161f), Vector3.forward);
    }
    
    
    public override void SetTransform(bool forcedUpdate)
    {
        HasToUpdate();
        
        Vector2 animPos  = item.GetPos(GTime.Now);
        float    offset  = animPos.x * 2 + animPos.y * 2;
        pivot.rotation = Rot.Y(GTime.Now * 360 * GTime.LoopMulti * 8 + offset * 4) * itemRot;
        
        if (Collector.IsCollected(item))
        {
            Collector.CollectionState state = Collector.collectionStates[item.ID];
            float start = state.time;
            float end   = start + CollectionLength;

            float animTime = GTime.Now - start;
            float lerp     = animTime / (end - start);

            if (lerp < 1)
            {
                Vector2 startPos = item.GetPos(start);
                if (anim != null)
                    startPos += anim.GetLean(start);

                Clip clip = state.spinner.tape.GetClip(end);
                Vector2 endPos = clip.BasicPlacement(end).pos;
                Vector2 mV     = clip.GetMV(end) * .25f;

                float smoothLerp = lerp * lerp * .75f;
                
                Vector2 pos      = Vector2.Lerp(startPos, endPos, smoothLerp);
                Vector2 mVOffset = mV * animTime * lerp;

                _transform.position = (pos + mVOffset).V3(Level.WallDepth * item.side.Sign);
                ShadowUpdate(true);
            }
            else
                _transform.position = V3.away;
            return;
        }

        if (anim != null)
            animPos += anim.GetLeanNow();
        
        _transform.position = animPos.V3(Level.WallDepth * item.side.Sign);
        
        ShadowUpdate(true);
    }
}
