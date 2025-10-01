using UnityEngine;


public class ImpactSquashTest : MonoBehaviour
{
    public Vector2 startPos;
    public float maxForce, floorHeight;
    
    private FlyPath fP;
    private float animStart = float.MaxValue;
    private bool falling, crash;

    private Vector3 size;
    private Vector2 impactMV;
    private Vector3 impactPos;

    [Space(10)] 
    public float freqDamp;
    public float damp;

    [Space(10)] public float timeScale;


    private void Awake()
    {
        size = transform.localScale;
    }


    private void Update()
    {
        Transform t = transform;
        
        if(!falling && Input.GetKeyDown(KeyCode.Space))
        {
            fP = new FlyPath(startPos, new Vector2(.001f, Random.Range(-maxForce, maxForce)));
            
            animStart = Time.realtimeSinceStartup;
            falling   = true;
            crash     = false;
            
            t.localScale = size;
        }

        
        
        if (falling)
        {
            float time = (Time.realtimeSinceStartup - animStart) / timeScale;
            
            Vector3 pos = fP.GetPos(time);
            if (pos.y <= floorHeight + size.y * .5f)
            {
                impactMV = fP.GetMV(time);
                
                pos.y = floorHeight + size.y * .5f;
                falling = false;
                crash = true;
                animStart = Time.realtimeSinceStartup;
                impactPos = pos;
               
            }
            
            t.position = pos;
        }


        if (crash)
        {
            float frequency = -Mathf.Abs(impactMV.y) / size.y / freqDamp;
            float duration  =  Mathf.Abs(impactMV.y) / damp;
            
            float time = (Time.realtimeSinceStartup - animStart) / timeScale;
            
            float squash = 1 + GPhysics.NewOscillate(time, frequency, duration);

            t.localScale = size.VolumeScaleY(squash);
            t.position = new Vector3(impactPos.x, floorHeight + size.y * .5f * squash);
        }
        
        
        
    }
}