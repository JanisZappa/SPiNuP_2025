using UnityEngine;


public static class GPhysics
{
    public const float StepsPerSecond = 20;
    public const float TimeStep       = 1f / StepsPerSecond;
    
    public const float Gravity        = 17; //19.25f;//19;

    public const float JumpForce = .9f; //.95f; //1f;
    
    public const int ForceFPS = 120;

    
    #region Spin

    private const float spinDamp = 95; //95f;   
    public static float Get_SpinAngle_Deg(float startSpin, float time)
    {
        time = Mathf.Min(time, Get_SpinStopTime(startSpin));
        float absStartSpin = Mathf.Abs(startSpin);
        
        return Mathf.Sign(startSpin) * ( absStartSpin * 35f * time + .5f * -spinDamp * Mth.IntPow(time, 2) );
    }

    
    public static float Get_SpinSpeed_After(float startSpin, float time)
    {
        float timeUntilStop = Get_SpinStopTime(startSpin);

        if (time <= timeUntilStop)
            return startSpin + spinDamp / 35f * time * -Mathf.Sign(startSpin);
        
        return 0;
    }
    
    
    public static float Get_SpinStopTime(float startSpin)
    {
        return Mathf.Abs(startSpin) * 35f / spinDamp;
    }


    public static float GetStartSpin(float endAngle, float time)
    {
        return Mathf.Sign(endAngle) * (Mathf.Abs(endAngle) / time - -spinDamp * time * .5f) / 35f;
    }
    #endregion
    
    
    public static float Oscillate(float time, float maxTime, float frequency, float dampExp, float springAccel, float offset = 0)
    {
        float lerp      = time / maxTime;
        
        float amplitude = Mathf.Pow(1 - lerp, dampExp);

        float accel     = .5f * frequency * springAccel * time * time;
        return (Mathf.Sin((offset + frequency * time + accel) * Mathf.PI * 2) * amplitude).NaNChk();
    }
    
    
    public static float NewOscillate(float time, float frequency, float duration = 10, bool oneSided = false)
    {
        float lerp      = Mathf.Clamp01(time / duration);
        float sideLerp  = oneSided ? 1 - Mathf.Pow(1 - lerp, 3) : 0;
        float amplitude = Mathf.Pow(1 - lerp, 2) * (1 - .5f * sideLerp);

        return ((Mathf.Sin(frequency * time * Mathf.PI * 2) * amplitude).NaNChk() + sideLerp) * amplitude;
    }
}


public struct FlyPath
    {
        public const float  CrashSpeed       = 35;
        private const float Pow              = 2.5f;  //2.4f;
        private const float LinearStartSpeed = 31.5f;//27.5f;
        private const float MaxFallSpeed     = 60;
        
        private const float Gravity = GPhysics.Gravity;


        public readonly Vector2 startPos, startMV;
        private readonly Vector2 expMV;

        public readonly float apexTime, linearStart, maxSpeedTime;
        private readonly float expY;
        
        
        public FlyPath(Vector2 startPos, Vector2 startMv)
        {
            this.startPos = startPos;
            this.startMV       = startMv;
            
            linearStart   = GetLinearStartTime(startMv);
            maxSpeedTime  = GetMaxFallSpeedTime(startMv, linearStart);
            apexTime      = Get_ApexTime(startMv);
            
            expY  = startMv.y * linearStart - .5f * GPhysics.Gravity * Mathf.Pow(linearStart, Pow) + startPos.y;
            expMV = Get_MotionVector(linearStart, startMv, linearStart, maxSpeedTime);
        }


        public Vector2 GetPos(float time)
        {
        //  X Air Resistance  //
            float w      = 1 / (.005f * Mathf.Abs(startMV.x));
            float movedX = startMV.x * w * Mathf.Log(1 + time / w);

            float x = movedX + startPos.x;
            float y;
            
            if (time <= linearStart)
                y = startMV.y * time - .5f * Gravity * Mathf.Pow(time, Pow) + startPos.y;
            
            else if (time <= maxSpeedTime)
            {
                float linearTime = time - linearStart;
                y = expMV.y * linearTime - .5f * Gravity * Mth.IntPow(linearTime, 2) + expY;
            }
            else
            {
                float linearTime = maxSpeedTime - linearStart;
                float linearY    = expMV.y * linearTime - .5f * Gravity * Mth.IntPow(linearTime, 2) + expY;
                y = linearY - MaxFallSpeed * (time - maxSpeedTime);
            }
            
            return new Vector2(x, y);
        }


        public Vector2 GetMV(float time)
        {
            return Get_MotionVector(time, startMV, linearStart, maxSpeedTime);
        }

        
        private static float Get_ApexTime(Vector3 mV)
        {
            if (mV.y <= 0)
                return 0;
        
            float expAccel = Mathf.Pow(mV.y / (.5f * Gravity * Pow), 1f/ (Pow - 1));
        
            return expAccel;
        }
        
        
        private static Vector2 Get_MotionVector(float time, Vector2 mV, float linearStart, float maxSpeedTime)
        {
            float xVel = AirXVelocity(mV.x, time);
            float yVel;
            
            if (time <= linearStart)
                yVel = mV.y - .5f * Gravity * Pow * Mathf.Pow(time, Pow - 1);
            
            else if (time <= maxSpeedTime)
            {
                float expY_Vel = mV.y - .5f * Gravity * Pow * Mathf.Pow(linearStart, Pow - 1);
                float linearTime = time - linearStart;
                yVel = expY_Vel - Gravity * linearTime;
            }
            else
                yVel = -MaxFallSpeed;
        
            return new Vector2(xVel, yVel); 
        }
    
    
        private static float AirXVelocity(float xSpeed, float time)
        {
            float w = 1 / (.005f * Mathf.Abs(xSpeed));
            return xSpeed / (1 + time / w);
        }
    
    
        private static float GetMaxFallSpeedTime(Vector3 mV, float linearStart)
        {
            Vector2 expMV = Get_MotionVector(linearStart, mV, linearStart, float.MaxValue);
        
            float speed = expMV.y + MaxFallSpeed;
        
            if (speed <= 0)
                return linearStart;
        
            return linearStart + speed / Gravity;
        }
    
    
        private static float GetLinearStartTime(Vector3 mV)
        {
            float speed = mV.y + LinearStartSpeed;
        
            if (speed <= 0)
                return 0;
        
            return Mathf.Pow(speed / (.5f * Gravity * Pow), 1f/ (Pow - 1));
        }
    }
    

