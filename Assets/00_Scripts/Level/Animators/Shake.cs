using Clips;
using LevelElements;
using UnityEngine;


namespace ActorAnimation
{
    public enum shake { Jump = 1, Swing = 2, WarpAway = 3, FlyBy = 4 }
    
    public class Shake : PoolObject
    {
        static Shake()
        {
            shakePool = new Pool<Shake>(() => new Shake(), 2000);
        }
        
        
        private static readonly Pool<Shake> shakePool;

        public static Shake Get(shake shake, float eventTime, Item item, Vector2 motionV, Clip clip)
        {
            return shakePool.GetFree().Setup(shake, eventTime, item, motionV, clip);
        }

        public float eventTime, shakeEndTime;
        public Item item;
        
        private Vector2 shakeV;
        private float   offset, frequency, duration, itemMass, itemDamp, itemAccel, itemLazyness;
        private Clip    clip;
        
        public Spinner Spinner { get { return clip != null ? clip.spinner : null; } }

        
        public void Reset()
        {
            clip = null;
            shakePool.Return(this);
        }


        private void SetItemParams(elementType elementType)
        {
            itemMass     = elementType.Mass();
            itemDamp     = elementType.Damp();
            itemAccel    = elementType.Accel();
            itemLazyness = elementType.Lazyness();
        }


        private Shake Setup(shake shake, float eventTime, Item item, Vector2 motionV, Clip clip)
        {
            this.clip      = clip;
            this.eventTime = eventTime;
            this.item      = item;
            
            if(item != null)
                SetItemParams(item.elementType);
            else
                Debug.Log("Shake Without Item");
            
            
            float maxSpeed = 2.5f / itemMass;

            const float testMulti = 1.15f;
            
            
            const float jumpMulti = 1.5f;
            switch (shake)
            {
                case shake.Jump:
                {
                    shakeV = motionV * jumpMulti;
                    float magnitude = shakeV.magnitude;
                    frequency = Mathf.Min(magnitude * 15.75f, maxSpeed);
                    duration  = magnitude * 25;
                    offset    = Mathf.Asin(1f / jumpMulti) / (2 * Mathf.PI);
                    break;
                }


                case shake.Swing:
                {
                    shakeV = motionV * .015f / itemMass;
                    float magnitude = shakeV.magnitude * testMulti;
                    frequency = Mathf.Min(magnitude * 7.875f, maxSpeed);
                    duration  = magnitude * 25;
                    offset    = 0;
                    break;
                }


                case shake.WarpAway:
                {
                    shakeV = motionV;
                    float magnitude = shakeV.magnitude * testMulti;
                    frequency = Mathf.Min(magnitude * 15.75f, maxSpeed);
                    duration  = magnitude * 25;
                    offset    = .25f;
                    break;
                }


                case shake.FlyBy:
                {
                    shakeV = motionV * .005f / itemMass;
                    float magnitude = shakeV.magnitude * testMulti;
                    frequency = Mathf.Min(magnitude * 35.75f, maxSpeed);
                    duration  = magnitude * 100;
                    offset    = 0;
                    break;
                }
            }
            
            frequency *= Random.Range(.95f, 1.05f);

            if (itemAccel < 0)
                duration = Mathf.Min(duration, 1f / Mathf.Abs(itemAccel));
            
            shakeEndTime = eventTime + duration / itemLazyness;
            return this;
        }
        
        
        public void GetShake(float time, ref Vector2 returnVector)
        {
            if (time < eventTime || time > shakeEndTime)
                return;

            float occi = GPhysics.Oscillate((time - eventTime) * itemLazyness, duration, frequency, itemDamp, itemAccel, offset);
            
            returnVector = new Vector2(returnVector.x + occi * shakeV.x, returnVector.y + occi * shakeV.y);
        }


        public void Shift(float restartShift)
        {
            eventTime    += restartShift;
            shakeEndTime += restartShift;
        }
    }
}