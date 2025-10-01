using GeoMath;
using UnityEngine;


[System.Serializable]
public class TrajectoryInfo
{
    public Vector2 startPos;
    public float startAngle;
    [Space(5)]
    public Vector2 motionVector;
    public float spin;
    [Space(5)]
    public float time;
}

public class collisiontest : Singleton<collisiontest> {
    
    public Vector2 initialMotionVector;
    public float initialSpin;
    
    [Space(10)]
    [Header("FlyState")]
    public TrajectoryInfo trajectory;

    private FlyPath flyPath;

    [Space(10)]
    [Range(0, 1)]
    public float slowMo;
    public static float SlowMoFactor { get { return Inst.slowMo; } }

    [Space(10)]
    [Range(0,1)]
    public float bouncy;

    [Space(10)]
    public Transform spinner;
    public bouncy[] bouncys;

    public float Inertia;

    // Private  
        private Vector2 initialPosition;
        private float initialAngle;
        private Vector2 rectangleScale;
        private bool go;

        private readonly Quad playerQuad = new Quad();

        private const float mass = 10;
        private float inertia { get { return Inertia; } }    //(mass / 12f) * (.6f * .6f + 2f * 2f);
    

    private Vector2 stickVelocity;
    private float stickMass;

    private int hitCount;
    private int round;

    private void Start ()
    {
        stickVelocity = Vector2.zero;
        stickMass = 100000;

        initialPosition  = spinner.position;
        rectangleScale = spinner.localScale;
        initialAngle = spinner.eulerAngles.z;
        DRAW.Enabled = DRAW.EditorDraw = true;
    }


    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            go = !go;
            if ( go )
            {
                round = (round + 1) % 3;

                hitCount = 0;
                trajectory.time = 0;
                trajectory.startAngle = initialAngle;
                trajectory.startPos = initialPosition;
                trajectory.motionVector = initialMotionVector;
                trajectory.spin = initialSpin;
                
                flyPath = new FlyPath(initialPosition, initialMotionVector);
            }
        }

        if ( !go )
            return;

        trajectory.time += Time.deltaTime * slowMo;
        
        //    Set Transform    //
        spinner.position = flyPath.GetPos(trajectory.time);
        spinner.eulerAngles = new Vector3(0, 0, trajectory.startAngle + GPhysics.Get_SpinAngle_Deg(trajectory.spin, trajectory.time));
        
        
        //    Check Collision    //
        SetPlayerQuad(trajectory.time);
        
        for (int i = 0; i < bouncys.Length; i++)
        {
            if ( !bouncys[i].gameObject.activeInHierarchy )
                continue;

            Circle circle = new Circle(bouncys[i].Position, .5f);
            if ( !playerQuad.Intersects(circle))
                continue;

            Collision(bouncys[i]);
            break;
        }
    }


    private void SetPlayerQuad(float t)
    {
        Vector2 pos = flyPath.GetPos(t);
        float angle = trajectory.startAngle + GPhysics.Get_SpinAngle_Deg(trajectory.spin, t);
        playerQuad.SetRect(pos, rectangleScale, angle); 
    }


    private string ColorString
    {
        get
        {
            switch ( round )
            {
                default: return "red";
                case 0: return "lime";
                case 1: return "orange";
                case 2: return "yellow";
            }
        }
    }


    private bool CollisionOccured(float t, Vector2 bouncyPos)
    {
        SetPlayerQuad(t);
        Circle circle = new Circle(bouncyPos, .5f);
        return playerQuad.Intersects(circle);
    }


    private void Collision(bouncy bouncy)
    {
        hitCount++;

        if(hitCount == 1)
        Debug.Log("<color=" + ColorString + ">" + hitCount + "_HIT Bouncy " + bouncy.name + "</color>");
        
        float stepLength = Time.deltaTime * 2;
        float impactTime = trajectory.time;
        Vector2 bouncyPos = bouncy.Position;
        
        for(int i = 0; i < 30; i++ )
        {
            stepLength *= .5f;

            if ( CollisionOccured(impactTime, bouncyPos) )
                impactTime -= stepLength;
            else
                impactTime += stepLength;

            if (stepLength < 0.0000001f)
            {
                Debug.Log(i);
                break;
            }
        }

        impactTime += stepLength;

        trajectory.time -= impactTime;
        trajectory.startAngle = trajectory.startAngle + GPhysics.Get_SpinAngle_Deg(trajectory.spin, impactTime);
        
        
        //    Resolve Collision    //
        float currentRadSpin = GPhysics.Get_SpinSpeed_After(trajectory.spin,impactTime) * 35f * Mathf.Deg2Rad;

        Vector2 charPos      = flyPath.GetPos(impactTime);
        Vector2 charVelocity = flyPath.GetMV(impactTime);

        Vector2 charRadiusVector = Tri.HitPoint - charPos;
        Vector2 charAngularVelocity = currentRadSpin.Cross(charRadiusVector);


        Vector2 charVelAtContact = charVelocity + charAngularVelocity;


        Vector2 stickPos = bouncy.Position;
        Vector2 stickVel = stickVelocity;

        Vector2 stickRadiusVector    = Tri.HitPoint - stickPos;
        Vector2 stickAngularVelocity = (0f /*stickSpin*/).Cross(stickRadiusVector);

        Vector2 stickVelAtContact = stickVel + stickAngularVelocity;


        Vector2 relativeVelocity = charVelAtContact - stickVelAtContact;


        float velAlongNormal = Vector2.Dot(relativeVelocity, Tri.HitNormalInverse);

        float hitDirCross = charRadiusVector.Cross(Tri.HitNormalInverse);
        
        float e = this.bouncy;
        float impulse = -(1.0f + e) * velAlongNormal / 
                         ( 
                            1.0f / mass + 1.0f / stickMass 
                            + Mth.IntPow(hitDirCross, 2) / inertia 
                            /* + Mathf.Pow(Extensions.CrossProduct(stickRadiusVector, Tri.HitNormalInverse), 2) / inertia */  //Stick inertia is infinite
                         );

        Vector2 impulseV = Tri.HitNormalInverse * impulse;

        trajectory.motionVector = charVelocity + impulseV / mass;
        trajectory.spin = (currentRadSpin + hitDirCross / inertia * impulse) * Mathf.Rad2Deg / 35f;
        trajectory.startPos = charPos;

        bouncy.Impact(relativeVelocity);

        if ( hitCount == 1 )
        {
            Debug.Log("Time: "          + impactTime);
            Debug.Log("CharPos x: "     + charPos.x + "  y: " + charPos.y);
            Debug.Log("CharAngle: "     + trajectory.startAngle);
            Debug.Log("ImpactV x: "     + relativeVelocity.x + "  y: " + relativeVelocity.y);
            Debug.Log("SpinSpeed: "     + currentRadSpin);
            Debug.Log("New Motion x: "  + trajectory.motionVector.x + "  y: " + trajectory.motionVector.y);
            Debug.Log("New SpinSpeed: " + trajectory.spin);
        }
    }
}