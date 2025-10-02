using Clips;
using UnityEngine;


public static class Interaction 
{
    public static void ActivateClip(Clip clip)
    {
        FunctionalResponse(clip);
        Sounds(clip);
        EnvironmentAnimation(clip);
    }
    
    
    private static void FunctionalResponse(Clip clip)
    {
        switch (clip.Type)
        {
            case ClipType.Jump:
            case ClipType.AirLaunch:
            {
                //  Collect Gets  //
                Collector.CollectThings(clip as Jump);
                break;
            }
                
            case ClipType.Swing:
            {
            //  Mood  //
                if (clip.spinner.isPlayer)
                    Mood.OnSwing(clip.startTime);

                goto case ClipType.Spawn;
            }

            case ClipType.Spawn:
            {
                Swing swing = clip as Swing;

                Link.SwingResponse(swing);
                Score.OnSwing(swing);
                break;
            }

            case ClipType.Dead:
            {
                if (clip.spinner.isPlayer)
                    Spinner.SetGameOver = true;
                else
                    ByteReplays.LoadRandomReplayFor(clip.spinner);
                break;
            }
        }
    }


    private static void EnvironmentAnimation(Clip clip)
    {
        switch (clip.Type)
        {
            case ClipType.Jump:
            {
                ActorAnimator.OnJump(clip as Jump);
                goto case ClipType.AirLaunch;
            }

            case ClipType.AirLaunch:
            {
            //  Collect Gets  //
                //Collector.CollectThings(clip as Jump);

                if (clip.after is Swing swing)
                {
                    ActorAnimator.OnSwing(swing, true);
                    
               
                //  Poof  //
                    Vector3 mV      = clip.GetMV(swing.startTime);
                    Vector3 charDir = swing.startStick.Item.GetPos(swing.startTime).V3() - clip.BasicPlacement(swing.startTime).pos;

                    float spinLerp   = swing.spinSpeed * .01f;
                    float impactLerp = mV.magnitude    * .0075f;

                    Vector3 mix = Vector3.Slerp(charDir.normalized, mV.normalized, impactLerp);

                    Poof.Show(swing.spinner, swing.startStick.Item, mix, spinLerp, swing.startTime);
                }
                break;
            }

            case ClipType.Spawn:
            {
                Swing swing = clip as Swing;
                ActorAnimator.OnSwing(swing, clip.Type == ClipType.Spawn);
                break;
            }

            case ClipType.Bump:
            {
                ActorAnimator.OnBump(clip as Bump);
                break;
            }
        }
    }


    private static void Sounds(Clip clip)
    {
        switch (clip.Type)
        {
            case ClipType.Jump:
            {
                Jump jump = clip as Jump;
                
                if(jump.spinner.isPlayer)
                    DrumRoll.Jump();
        
                float multi  = jump.jumpV.magnitude * .025f;
                float volume = multi - Random.Range(.05f, .15f);
                float pitch  = .45f + Random.Range(0, .1f) + multi * .4f;
                Sound.Get(Audio.Sound.StickJump).Volume(volume).PlayerMulti(clip.spinner).Pitch(pitch).SetItem(jump.stick.Item).Play();
                break;
            }
            
            case ClipType.Swing:
            {
                Swing swing = clip as Swing;
                
                float multi  = swing.startMotion.magnitude * .025f;
                float volume = multi - .02f - Random.Range(.05f, .15f);
                float pitch  = 1.05f + Random.Range(0, .06f) + multi * .6f;
                Sound.Get(Audio.Sound.StickContact).Volume(volume).PlayerMulti(clip.spinner).Pitch(pitch).SetItem(swing.startStick.Item).Play();       
            }    
                goto case ClipType.Spawn;

            case ClipType.Spawn:
            {
                Swing swing = clip as Swing;
                
                if(swing.spinner.isPlayer)
                    DrumRoll.Swing(swing.startStick.Item);
                break;
            }

            case ClipType.Dead:
            {
                Sound.Get(Audio.Sound.DeathHit).Play();
                break;
            }
            
            case ClipType.Bump:
            {
                Sound.Get(Audio.Sound.DeathHit).PlayerMulti(clip.spinner).SetItem((clip as Bump).bumpItem).Play();
                break;
            }
        }
    }
}
