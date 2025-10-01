using System;
using System.Runtime.CompilerServices;
using UnityEngine;


public static class StaticConstructor
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitStaticClasses()
    {
        Type[] initTypes =
        {
            typeof(KeyMap),    //
        
            typeof(ElementMask),
            typeof(Level),    //
            typeof(Search),   //
            typeof(LevelSaveLoad),    //
            typeof(LevelElements.Element),
            typeof(LevelElements.Item),
            typeof(LevelElements.Track),    //
            typeof(Generation.Cell),        //
            typeof(GPhysics),
        
            typeof(GeoMath.Bounds2D),    //
            typeof(GeoMath.Circle),
            typeof(GeoMath.Line),
            typeof(GeoMath.Quad),
            typeof(GeoMath.Rectangle),
            typeof(GeoMath.ShapeCollision),
            typeof(GeoMath.Tri),
        
            typeof(Tape),
        
            typeof(Clips.Clip),
            typeof(Clips.AirLaunch),
            typeof(Clips.Bump),
            typeof(Clips.Dead),
            typeof(Clips.Jump),
            typeof(Clips.None),
            typeof(Clips.Spawn),
            typeof(Clips.Swing),
        
            typeof(Anim.SpinnerAnim),    //
            typeof(Anim.Anim_Jump),      //
            typeof(Anim.Anim_Swing),     //
            typeof(Anim.AnimLerp),    //
        
            typeof(ActorAnimation.ActorAnim), //
            typeof(ActorAnimator.ActorList),  //
            typeof(ActorAnimation.Shake),     //
        
            typeof(Future.Prediction),   //
            typeof(Future.PathSlice),    //
            typeof(JumpInfo),    //
        
            typeof(Spinner),        //
            typeof(ScoreStick),     //
            typeof(SquashAnim),     //
        
            typeof(Score),    //
            typeof(Controll),
            typeof(UI_Manager),
        
            typeof(SpinnerDebug), //
        
            typeof(Rot),
        
            typeof(DRAW),
            typeof(DRAW.ReusableMesh),
            typeof(DRAW.Shape),
            
            typeof(SetVolume),        //
            typeof(Audio),
            typeof(Audio.Ambient),    //
            typeof(Audio.Music),      //
            typeof(Audio.Reaction),   //
            typeof(Audio.Sound),      //
            typeof(Audio.UI),         //
            
            typeof(Mixer),    //
            
            typeof(Encryption)    //
        };
        
        
        for (int i = 0; i < initTypes.Length; i++)
            RuntimeHelpers.RunClassConstructor(initTypes[i].TypeHandle);
    }
}
