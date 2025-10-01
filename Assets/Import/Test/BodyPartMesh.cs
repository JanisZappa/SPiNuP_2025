using System;
using UnityEngine;


[CreateAssetMenu]
public class BodyPartMesh : ScriptableObject
{
    public Mesh mesh;
    public PartType partType;

    [Space(10)] 
    public Locator[] locators;


    public Vector3 Get(PartType partType)
    {
        for (int i = 0; i < locators.Length; i++)
            if (locators[i].type == partType)
                return locators[i].pos;

        return locators[0].pos;
    }
}


[Serializable]
public class Locator
{
    public PartType type;
    public Vector3 pos;


    public Locator(PartType type, Vector3 pos)
    {
        this.type = type;
        this.pos  = pos;
    }
    
    public Locator(string name, Vector3 pos)
    {
             type = (PartType)Enum.Parse(typeof(PartType), name);
        this.pos  = pos;
    }
}


public enum PartType
{
    Stick,
    Head,
    Torso,
    Arm, ArmL, ArmR,
    Leg, LegL, LegR,
    Mouth,
    Eye, EyeL, EyeR
}
