using System.IO;
using System.Text;
using LevelElements;
using UnityEngine;


namespace Anim
{
    public struct StickID
    {
        public readonly Item Item;
        public readonly bool hands;

        public int  Sign => hands ? 1 : -1;
        public int  SerializeValue => (Item.ID + 1) * Sign;


        public StickID(Item Item, bool hands)
        {
            this.Item  = Item;
            this.hands = hands;
        }

        
        public bool SameAs(Item otherItem)
        {
            if (otherItem == null)
            {
                Debug.Log("Can't compare StickID to Other Item since it's " + "null!".B_Red());
                return false;
            }
            
            return Item == otherItem;
        }
    }
    
    
    public class Costume
    {
        private readonly sbyte head, torso, arms, legs;

        public Costume(){}

        private Costume(sbyte head, sbyte torso, sbyte arms, sbyte legs)
        {
            this.head = head;
            this.torso = torso;
            this.arms = arms;
            this.legs = legs;
        }
        
        public static void Serialize(Costume costume, BinaryWriter writer)
        {
            writer.Write(costume.head);
            writer.Write(costume.torso);
            writer.Write(costume.arms);
            writer.Write(costume.legs);
        }

        public static Costume Deserialize(BinaryReader reader)
        {
            return new Costume(reader.ReadSByte(), reader.ReadSByte(), reader.ReadSByte(), reader.ReadSByte());
        }
    }

    
    public class Pose
    {
        public const int Lean    = 0, 
                         Tumble = 1, 
                         Bend   = 2, 
                         Turn   = 3,
                         Pivot  = 4;
                    
        
        
        public const int ValueCount = 5;
        
        
        private readonly float[] values = new float[ValueCount];
        
        public static readonly Pose Reader = new Pose();

        
        public Pose Zero()
        {
            values[Lean]    = 0;
            values[Tumble] = 0;
            values[Bend]   = 0;
            values[Turn]   = .25f;
            values[Pivot]   = .5f;
            
            return this;
        }
        

        public Pose Set(int part, float value)
        {
            values[part] = value;
           
            return this;
        }
        

        public Pose Copy(Pose other)
        {
            for (int i = 0; i < ValueCount; i++)
                values[i] = other[i];

            return this;
        }

        
       public float this[int index] => values[index];

       //public Vector2 this[int index] { get { return new Vector2(values[index], 0); } }

        
        public StringBuilder GetInfo(StringBuilder writer, string[] names, bool reset = true)
        {
            if(reset)
                writer.Length = 0;
            
            for (int i = 0; i < ValueCount; i++)
                writer.Append(names[i]).Append(GetValueString(i)).Append(i < ValueCount - 1? "\n" : "");

            return writer;
        }

        
        private string GetValueString(int index)
        {
            float value = values[index];
            
            return value > 0? "+" + value.ToString("F4") : value < 0 ? value.ToString("F4") : " 0.0000";
        }
    }


    public struct BodyHalfState
    {
        public float bend, turn, tumble, tight;
    }
}