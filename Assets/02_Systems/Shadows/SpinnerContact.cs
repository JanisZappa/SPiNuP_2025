using Anim;
using Clips;
using UnityEngine;


public class SpinnerContact : SpinnerShadow
{
    protected override void UpdateTransform() 
    {
        Clip clip = spinner.currentClip;
        
        if(clip == null)
            return;

        Vector2 offset = (clip.GetSide(GTime.Now).front? -1 : 1) * Level.PlaneOffset * LightingSet.SunXFactor / MapCam.mapSizeA * 5;
        
        Placement placement = clip.BasicPlacement(GTime.Now, true);
        _transform.position = placement.pos.V2() + offset;
        _transform.rotation = placement.rot;
    }
    
    
    public override void Setup(Vector3 size)
    {
        wideFactor = size.x / size.z;
        const float multi = 1.75f;
        _transform.localScale = this.size = new Vector3(.5f / .5f * multi,  2.56466f / size.y * multi, 1);
    }
}
