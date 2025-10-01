using Anim;
using UnityEngine;


public class Bone
{
    public Vector3    rigPos, squashPos, turnPos, bendPos;
    public Quaternion turnRot, bendRot;
        
		
    public void SetupBone(Vector3 rigPos)
    {
        this.rigPos = rigPos;
    }


    public Placement GetBendPlacement(Vector3 offset)
    {
        return new Placement(bendPos + bendRot * offset, bendRot);
    }
}


public class TBone : Bone
{
    private Transform  transform;
    public  Vector3    updateScale = V3.one, twistScale = V3.zero;
    public  Quaternion twistRot    = Quaternion.identity;
        
    public float chainLerp;
    
    public void SetupBone(Transform transform, Vector3 rigPos, float chainLerp = 0)
    {
        this.transform = transform;
        this.rigPos    = rigPos;

        this.chainLerp = chainLerp;
        
        
        transform.position = rigPos;
    }
    
    
    public void SetXZScale(Vector3 scale)
    {
        updateScale = scale.SetY(1);
    }
		
      
    public void UpdateTransform()
    {
        transform.localPosition = bendPos;
        transform.localRotation = bendRot * (turnRot * twistRot);
        transform.localScale    = updateScale + twistScale;
    }


    public void SimpleUpdateTransform()
    {
        transform.localPosition = bendPos;
        transform.localRotation = bendRot;
        transform.localScale    = updateScale;
    }
}
