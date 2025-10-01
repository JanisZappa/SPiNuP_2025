using Clips;
using UnityEngine;


public class SpinnerShadow : Shadow 
{
    [Space(10)]
    public Spinner spinner;
    protected Vector3 size;
    protected float wideFactor;
    
    
    protected override void UpdateTransform()
    {
        Clip      clip      = spinner.currentClip;
        if(clip?.spinner == null)
            return;
        
        Placement placement = clip.BasicPlacement(GTime.Now, true);
        Vector3   toWall    = LightingSet.ShadowDir * ((Level.PlaneOffset + clip.spinner.GetZShift(GTime.Now)) * (front ? 1 : -1) / LightingSet.ShadowDir.z);
        
        _transform.position = new Vector2(placement.pos.x + toWall.x, placement.pos.y + toWall.y);
        _transform.rotation = placement.rot;
    }


    public virtual void Setup(Vector3 size)
    {
        wideFactor = size.x / size.z;
        _transform.localScale = this.size = new Vector3(1, 2.56466f / size.y, 1);
    }
    
    
    public void ManualUpdate(bool front, float turn, Vector3 squash)
    {
        const float turnMulti = 1f / 360;
        float lerp = Mathf.PingPong(Mathf.Repeat(turn * turnMulti, 1) * 4, 1);
        float wideness = Mathf.Lerp(wideFactor, 1, lerp);
        _transform.localScale = Vector3.Lerp(size, size.MultiBy(squash), .5f).MultiX(wideness);
        
        SetSide(front);
        ShadowUpdate(true);
    }
}
