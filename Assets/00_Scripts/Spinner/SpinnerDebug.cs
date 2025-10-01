using System.Collections.Generic;
using Clips;
using Future;
using GeoMath;
using LevelElements;
using UnityEngine;


public static  class SpinnerDebug 
{
    private static readonly BoolSwitch predictionLines  = new("Jump/Jump Lines", false);
    private static readonly BoolSwitch predictionShapes = new("Jump/Connection Shapes", false);
    private static readonly BoolSwitch stepInfo         = new("Jump/Search Step Info", false);
    
    private static readonly BoolSwitch nearGrabs = new("Jump/Near Grabs", false);
    private static readonly BoolSwitch nearGets  = new("Jump/Near Gets", false);
    private static readonly BoolSwitch showZappy = new("Jump/Zappy", false);
    
    private static readonly BoolSwitch showRigPlane = new("Char/Rig Plane", false);
    
    private static readonly BoolSwitch showJumpInfo = new("Info/Jump", false);
    private static readonly BoolSwitch searchBounds = new("Dev/Search Bounds", false);
    private static readonly BoolSwitch closeEnough  = new("Dev/Close Enough", false);
    
    private static readonly BoolSwitch showPlayerBounds = new ("Bounds/Player", false);
    
    private static readonly BoolSwitch showShadowPlacement = new("Char/Shadow Placement", false);
    private static readonly BoolSwitch showMV              = new("Char/MV", false);

    private static readonly BoolSwitch squashVis = new("Char/Squash Vis", false);
   
    
    static SpinnerDebug()
    {
        anchorPath  = new List<Vector2>(50);
        midPath     = new List<Vector2>(20);
    }
    
    private static List<Vector2> anchorPath, midPath;
    private static Color         midPointColor;
    
    private static Vector3     detectionPoint;
    

    private static Clip       clip;
    private static Jump  jump;
    private static FlyPath flyPath;
    private static Swing _swing;


    private static void CreateJumpInfo()
    {
        jump = (Jump) clip;
        flyPath = jump.flyPath;

        Vector2 startPos = jump.startPos;
        float charHeight = clip.spinner.size.y;
        
        Vector2 startgrabPoint = startPos + V2.up.Rot(jump.startAngle) * charHeight * (jump.nextStick.hands ? .5f : -.5f);

        if (predictionShapes)
            detectionPoint = jump.nextStick.Item != null ? jump.nextStick.Item.GetLagPos(GTime.Now + jump.duration) : V2.down * 1000;
        
        if (predictionLines)
        {
            float duration = jump.WillHit ? jump.duration : 3;

            if (jump.WillConnect)
                anchorPath = GetPath(startPos, startgrabPoint - startPos, jump.jumpV, jump.startSpin, duration, anchorPath);
            else
                anchorPath.Clear();
            
            midPointColor = jump.WillConnect ? COLOR.grey.light : COLOR.purple.maroon;
            midPath       = GetPath(startPos, V3.zero, jump.jumpV, jump.startSpin, duration, midPath);
        }
    }

    
    private static List<Vector2> GetPath(Vector2 startPos, Vector2 anchorV, Vector2 jumpV, float spin, float time, List<Vector2> points)
    {
        const float range = .05f; 
        int max = Mathf.CeilToInt(time / range) + 1;
        
        FlyPath flyPath = new FlyPath(startPos, jumpV);
        
        points.Clear();
        for ( int i = 0; i < max; i++ )
        {
            float checkTime = i < max - 1 ? i * range : time;
            
            Vector2 newPos    = flyPath.GetPos(checkTime);
            Vector2 anchorPos = anchorV == V2.zero ? V2.zero : anchorV.Rot(GPhysics.Get_SpinAngle_Deg(spin, checkTime));
            points.Add(newPos + anchorPos);
        }

        return points;
    }    
    

    public static void DebugUpdate()
    {
        if (!GameManager.Running || GameManager.IsCreator)
            return;
        
        Clip newClip = Spinner.CurrentFocusClip;
        
     
        if (newClip == null)
            return;
        
        if (clip != newClip)
        {
            clip = newClip;
            switch (clip.Type)
            {
                case ClipType.Jump:  CreateJumpInfo(); break;
                case ClipType.Swing: _swing = (Swing)clip; break;
            }   
        }


        if(clip.spinner == null)
            return;


        Placement placement = clip.spinner.currentPlacement;
        Vector3    midPoint = placement.pos;
        Quaternion rotation = placement.rot;
        float      height   = clip.spinner.size.y;
        
        
        switch (clip.Type)
        {
            case ClipType.Jump:  DebugJump(midPoint, rotation, height); break;
            case ClipType.Swing: DebugSwing(midPoint);                  break;
        }

        
        if(showRigPlane)
        {
            Vector3  hands = midPoint + V3.forward * Level.PlaneOffset + rotation * V3.up * (height * .5f + Item.DefaultRadius);
            Vector3 center = midPoint + V3.forward * Level.PlaneOffset;
            Vector3   feet = midPoint + V3.forward * Level.PlaneOffset + rotation * V3.up * (height * -.5f - Item.DefaultRadius);
            Vector3    dir = -V3.forward * Level.PlaneOffset * 2;
            
            DRAW.Vector(center, dir).SetColor(COLOR.blue.deepsky);
            DRAW.Vector(hands,  dir).SetColor(COLOR.green.spring);
            DRAW.Vector(feet,   dir).SetColor(COLOR.green.spring);

            for (int i = 0; i < 20; i++)
                DRAW.Vector(hands + dir * ((float)i / 19), feet - hands).SetColor(Color.white.A(.2f));
        }


        if (showShadowPlacement)
        {
            Placement shadowP = clip.BasicPlacement(GTime.Now, true);
            Vector2   heightV = shadowP.rot * V2.up * clip.spinner.size.y * .5f * clip.spinner.squashes.GetSquash(GTime.Now);
            DRAW.Arrow(shadowP.pos,  heightV, .2f).SetColor(COLOR.red.tomato).SetDepth(Level.GetWallDist(clip.GetSide(GTime.Now), .1f)).Fill(1);
            DRAW.Arrow(shadowP.pos, -heightV, .2f).SetColor(COLOR.blue.cornflower).SetDepth(Level.GetWallDist(clip.GetSide(GTime.Now), .1f)).Fill(1);
        }
        
        
        if (showPlayerBounds)
        {
            int activeCount = Spinner.active.Count;
            for (int i = 0; i < activeCount; i++)
            {
                Spinner spinner = Spinner.active[i];
                
                Clip  currentClip = spinner.currentClip;
                Side  side        = currentClip.GetSide(GTime.Now);
                
                Color color = spinner.visible ? side.front ? COLOR.yellow.fresh : COLOR.blue.cornflower 
                                                : side.front ? COLOR.red.tomato   : COLOR.purple.orchid;

                DRAW.ZappRectangle(currentClip.FinalPlacement(GTime.Now).pos, V2.one * spinner.size.y, 16, .1f, .1f).SetColor(color).SetDepth(Level.GetPlaneDist(side));
            } 
        }

        if (showMV)
        {
            Clip spinnerClip = Spinner.CurrentFocusClip;
            if (spinnerClip != null)
            {
                Vector3 pos = spinnerClip.FinalPlacement(GTime.Now).pos;
                Vector3 mV  = spinnerClip.GetMV(GTime.Now);

                DRAW.Arrow(pos, mV, .3f).SetColor(Color.yellow).Fill(1);
            }
        }


        if (squashVis)
        {   
            float squash = clip.spinner.squashes.GetSquash(GTime.Now);

            Vector3 size = clip.spinner.size;
                    size = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z)).VolumeScaleY(squash);

            float visHeight = Screen.height * .1f;
            Color c = squash <= 1f ? Color.Lerp(COLOR.purple.maroon, COLOR.red.tomato, squash) : Color.Lerp(COLOR.red.tomato, COLOR.yellow.fresh, squash - 1);

            Vector2 pos      = new Vector2(Screen.width - visHeight, Screen.height * .5f);
            Vector2 drawSize = new Vector2(size.z, size.y) * visHeight;
            
            DRAW.Rectangle(pos + new Vector2(visHeight * .1f, visHeight * -.1f), drawSize).ToScreen().SetColor(Color.black).Fill(.18f, true);
            DRAW.Rectangle(pos, drawSize).ToScreen().SetColor(c).Fill(.85f, true);

            DRAW.Vector(pos + new Vector2(visHeight * -1, visHeight * clip.spinner.size.y * .5f),  new Vector3(visHeight * 2, 0, 0)).ToScreen().SetColor(Color.white.A(.5f));
            DRAW.Vector(pos + new Vector2(visHeight * -1, visHeight * clip.spinner.size.y * -.5f), new Vector3(visHeight * 2, 0, 0)).ToScreen().SetColor(Color.white.A(.5f));
        }
    }


    private static void DebugJump(Vector3 midPoint, Quaternion rotation, float height)
    {
        Side side  = jump.GetSide(GTime.Now);
        bool hands = jump.nextStick.hands;
        
        
        if (showJumpInfo)
        {
            string txt =
                (GTime.Now - jump.startTime).ToString("F1") + " | "
                + jump.duration.ToString("F1");

            DRAW.Text(txt, midPoint, COLOR.green.spring, 2f, offset: V2.up * 6);

            float  spin      = GPhysics.Get_SpinSpeed_After(jump.startSpin, GTime.Now - jump.startTime);
            int    sign      = (int) Mathf.Sign(spin);
            string rL        = sign == 0 ? "" : (sign == 1 ? "+ " : "- ");
            string spinSpeed = rL + Mathf.Abs(spin).ToString("F1");

            DRAW.Text(spinSpeed, midPoint, COLOR.green.spring, 2.4f, offset: V2.up * 6.5f);

            Vector2 mV = jump.GetMV(GTime.Now - jump.startTime);
            Color   c  = mV.magnitude >= FlyPath.CrashSpeed ? COLOR.red.tomato : COLOR.grey.light;
            DRAW.Text(mV.magnitude.ToString("F1"), midPoint, c, 2.4f, offset: V2.up * 8f);
        }
        
        
        if (predictionLines)
        {
            Color cool = Color.red.ToHLS().ShiftHue(Time.realtimeSinceStartup);
            jump.flyPath.Draw(0, jump.duration).SetColor(cool).SetDepth(Level.GetPlaneDist(jump.startSide));
            
            
            if (anchorPath.Count > 0)
                DRAW.Line(anchorPath.ToArray()).SetColor(hands ? COLOR.yellow.fresh : Color.Lerp(COLOR.red.tomato, COLOR.red.hot, .3f)).SetDepth(Z.P75);

            if (midPath.Count > 0)
                DRAW.Line(midPath.ToArray()).SetColor(midPointColor).SetDepth(Z.P75);


            Vector3 pivot2D = jump.BasicPlacement(GTime.Now).pos;
            Vector3 pivot3D = clip.spinner.currentPlacement.pos;
            
            DRAW.Circle(pivot2D, .15f).SetColor(COLOR.red.tomato).SetDepth(Z.P75).Fill(1);
            DRAW.Circle(pivot3D, .15f).SetColor(COLOR.blue.cornflower).SetDepth(Z.P75 + .01f * side.Sign).Fill(1);
            DRAW.Vector(pivot2D, pivot3D - pivot2D).SetColor(COLOR.blue.cornflower).SetDepth(Z.P75 + .05f * side.Sign);


            Vector3 stickPos = jump.stick.Item.GetPos(GTime.Now).V3(Level.WallDepth * side.Sign);
            DRAW.GapVector(stickPos, V3.forward * 6 * side.Sign, 16).SetColor(COLOR.red.tomato);

            if (jump.WillConnect && jump.nextStick.Item != jump.stick.Item)
            {
                Vector3 stickPosB = jump.nextStick.Item.GetPos(GTime.Now).V3(Level.WallDepth * side.Sign);
                DRAW.GapVector(stickPosB, V3.forward * 6 * side.Sign, 16).SetColor(COLOR.blue.cornflower);
            }
        }
        
        
        if (predictionShapes && jump.WillConnect)
        {
            DRAW.Rectangle(detectionPoint, Vector2.one * .3f).SetColor(COLOR.blue.cadet).SetDepth(Level.GetPlaneDist(side)).Fill();
            DRAW.Rectangle(jump.nextStick.Item.GetLagPos(GTime.Now), Vector2.one * (.8f + (Mathf.Sin(Time.realtimeSinceStartup * 16) + 1) * .1f), 45).
                SetDepth(Level.GetPlaneDist(side, -2.25f));
            
            Vector3 pos = midPoint + rotation * V3.up * (hands ? 1 : -1) * height * .5f;
            DRAW.Rectangle(pos, Vector2.one * .3f).SetColor(hands ? COLOR.yellow.fresh : COLOR.orange.sienna).SetDepth(Level.GetPlaneDist(side)).Fill();
        }


        if (stepInfo)
        {
            if (flyPath.apexTime > 0 && (!jump.WillHit || flyPath.apexTime < jump.duration))
            {
                bool closeToIt = Mathf.Abs(GTime.Now - (jump.startTime + flyPath.apexTime)) < .05f;
                DRAW.Circle(flyPath.GetPos(flyPath.apexTime), closeToIt? .3f: .1f, 16).SetDepth(Z.P75).Fill(1);
            }
            
            
            if ((!jump.WillConnect || flyPath.maxSpeedTime < jump.duration) && GTime.Now > clip.startTime + flyPath.maxSpeedTime - .25f)
            {
                Vector3 crashSpeedPos = flyPath.GetPos(flyPath.maxSpeedTime);
                DRAW.ZappCircle(crashSpeedPos, .3f, .25f, 10).SetColor(COLOR.orange.redish).SetDepth(Z.P75).Fill(.8f);
            }
            
            
            if ((!jump.WillConnect || flyPath.linearStart < jump.duration) && GTime.Now > clip.startTime + flyPath.linearStart - .25f)
            {
                Vector3 expEndPos = flyPath.GetPos(flyPath.linearStart);
                DRAW.ZappCircle(expEndPos, .3f, .25f, 10).SetColor(COLOR.purple.orchid).SetDepth(Z.P75).Fill(.8f);
            }
            
            
            float closestStepPoint = Mathf.Floor(jump.duration * GPhysics.StepsPerSecond) * GPhysics.TimeStep;
            for (int i = 0; i < 10; i++)
            {
                float time   = closestStepPoint - i * GPhysics.TimeStep;
                float alpha  = (1f - i * .1f) * .25f;
                float radius = .1f - i * .006f;

                if (time > flyPath.apexTime || flyPath.apexTime >= jump.duration)
                {
                    Vector3 stepPos = flyPath.GetPos(closestStepPoint - i * GPhysics.TimeStep);
                    DRAW.Circle(stepPos, radius).SetColor(new Color(1, 1, 1, alpha)).SetDepth(Z.P75).Fill(alpha);
                }

                Vector3 itemPos = jump.nextStick.Item.GetLagPos(jump.startTime + time);
                DRAW.Circle(itemPos, radius).SetColor(COLOR.yellow.fresh.A(alpha)).SetDepth(Z.P75).Fill(alpha);
            }
        }


        if (nearGrabs)
        {
            float depth = Level.GetPlaneDist(side);

            JumpInfo info = jump.info;
            
            for (int e = 0; e < info.proxCount; e++)
            {
                ProxItem proxItem = info.proxItems[e];
                Item item = proxItem.item;
                
                if(!Mask.CanBeGrabbed.Fits(item.elementType) || 
                   !proxItem.DebugIsGoingOn(GTime.Now))
                    continue;

                Color usecolor;
                switch (proxItem.Importance)
                {
                    default: usecolor = Color.Lerp(Color.white, COLOR.grey.light, .5f); break;
                     case 1: usecolor = COLOR.yellow.fresh;                             break;
                     case 2: usecolor = COLOR.red.tomato;                               break;
                     case 3: usecolor = COLOR.purple.maroon;                            break;
                }

                Vector2 stickPos = item.GetLagPos(GTime.Now);

                Vector2 lineStart = stickPos + proxItem.closestDir.SetLength(item.radius);
                Vector2 anchor    = lineStart + proxItem.closestDir * .5f;
                
                DRAW.DotVector(lineStart, proxItem.closestDir, .1f, .1f).SetColor(usecolor).SetDepth(depth).Fill(1);

                Vector2 motion = proxItem.closestMotion.normalized;
                DRAW.Arrow(anchor - motion, motion * 2, .3f).SetColor(usecolor).SetDepth(depth).Fill(1);
            }
        }


        if (nearGets)
        {
            float depth = Level.GetPlaneDist(side);

            JumpInfo info = jump.info;
            
            Vector2 charPos = jump.BasicPlacement(GTime.Now).pos.V2();
            float charRadius = jump.spinner.size.y * .5f;

            bool getNearBy = false;
            for (int e = 0; e < info.proxCount; e++)
            {
                ProxItem proxItem = info.proxItems[e];
                Item item = proxItem.item;
                
                if(!Mask.IsCollectable.Fits(item.elementType) ||
                   Collector.IsCollected(item) ||
                   !proxItem.DebugIsGoingOn(GTime.Now))
                   continue;
                
                Vector2 itemPos = item.GetPos(GTime.Now);
                float dist = (itemPos - charPos).sqrMagnitude;
                float checkDist = Mathf.Pow(item.radius + charRadius, 2);
                if(dist > checkDist)
                    continue;

                getNearBy = true;
                
                
                Color color = proxItem.upMotion ? Color.white : Color.green;
                DRAW.Circle(itemPos, item.radius, 20).SetColor(color).SetDepth(depth);

                Vector2 closestPos = jump.BasicPlacement(proxItem.closestTime).pos.V2();
                DRAW.DotVector(itemPos, closestPos - itemPos, .1f, .1f).SetColor(color).SetDepth(depth);
                
                //DRAW.Vector(closestPos, _jump.BasicPlacement(proxItem.startTime).pos.V2() - closestPos).SetColor(color).SetDepth(depth);
                //DRAW.Arrow(closestPos, _jump.BasicPlacement(proxItem.endTime).pos.V2() - closestPos, .2f).SetColor(color).SetDepth(depth);
               
                /*float end       = proxItem.closestTime + JumpInfo.StepLength;
                float checkTime = proxItem.closestTime - JumpInfo.StepLength;
                while (checkTime < end)
                {
                    Vector2 player = jump.BasicPlacement(checkTime).pos;
                    Vector2 getPos = item.GetPos(checkTime);
                    
                    float sqrDist = (player - getPos).sqrMagnitude;
                    Color c = sqrDist <= checkDist ? Color.green : Color.red;

                    DRAW.Circle(player, .2f, 10).SetColor(c).SetDepth(depth).Fill(1, true);
                    checkTime += Collector.StepLength;
                }*/
            }

            if (getNearBy)
            {
                
                DRAW.Circle(charPos, charRadius, 20).SetColor(Color.magenta).SetDepth(depth);
            }
        }


        if (showZappy && GTime.Now - clip.startTime < 1)
        {
            Vector2     stickPos    = jump.stick.Item.GetLagPos(GTime.Now);
            const int   nrOfLines   = 5;
            const float lineSpacing = 15;
            const float spread      = nrOfLines * lineSpacing;

            for (int i = 0; i < 7; i++)
            {
                Vector2 aimVector = jump.jumpV.normalized.Rot(spread * -.5f + i * lineSpacing, 4);
                Vector2 pointA    = Vector3.Lerp(stickPos, stickPos + aimVector, .2f);
                Vector2 pointB    = Vector3.Lerp(stickPos, stickPos + aimVector, .45f);

                DRAW.Zapp(pointA, pointB - pointA, 8, .2f, .3f).SetColor(Color.white).SetDepth(Level.GetPlaneDist(side));
            }
        }


        if (GTime.Paused && closeEnough && jump.WillConnect)
        {
            Vector2 stickPos = jump.nextStick.Item.GetLagPos(GTime.Now).V3();
            Vector2 playerPos = jump.BasicPlacement(GTime.Now).pos;

            float radius = jump.spinner.ConnectRadius(jump.nextStick.Item);
            if (Vector2.Distance(stickPos, playerPos) < radius)
                DRAW.Circle(stickPos, radius).SetColor(Color.magenta).SetDepth(Z.P).Fill(1);
        }

        
        if (GTime.Paused && searchBounds)
            ShowSearchBounds();
        
       
        //DRAW.Circle(_jump.BasicPlacement(GTime.Now).pos, _jump.spinner.size.y * .5f * 4).SetColor(COLOR.orange.coral).SetDepth(Z.Plane(0));
    }


    private static void ShowSearchBounds()
    {
        Side  side   = jump.startSide;
        float now    = Mathf.Floor((GTime.Now - jump.startTime) * GPhysics.StepsPerSecond) / GPhysics.StepsPerSecond;
        float before = Mathf.Clamp(now - GPhysics.TimeStep, 0, float.MaxValue);

        float radius    = clip.spinner.size.y * .5f;
        Vector2 posNow  = jump.flyPath.GetPos(now);
        Vector2 posThen = jump.flyPath.GetPos(before);
        
        Bounds2D stepSearch = new Bounds2D(posNow).Add(posThen).Pad(radius);
        
        DRAW.Rectangle(stepSearch.Center, stepSearch.Size).SetColor(COLOR.purple.maroon).SetDepth(Level.GetPlaneDist(side)).Fill(.1f);
        DRAW.Vector(posNow, posThen - posNow).SetColor(COLOR.purple.maroon).SetDepth(Level.GetPlaneDist(side));
    }

    
    private static void DebugSwing(Vector2 midPoint)
    {
        Side  side = _swing.GetSide(GTime.Now);
        float lerp = 1 - Mth.IntPow( 1 - Mathf.Min(1, (GTime.Now - _swing.startTime) * 3), 3);

        
        if (predictionLines)
        {
            DRAW.Arrow(midPoint,
                _swing.GetForceV(GTime.Now).SetLength(2.05f * lerp), .4f * lerp).
                SetColor(COLOR.grey.light).SetDepth(Level.GetPlaneDist(side, -.55f)).Fill(1);

            Vector2 swingForce = _swing.GetForceV(GTime.Now).normalized;
            
            DRAW.Shape.Get(4).Set(0, midPoint).
                              Set(1, midPoint + swingForce.Rot(Prediction.CheckAngle * -.5f, 1.5f * lerp)).
                              Set(2, midPoint + swingForce.Rot(Prediction.CheckAngle * .5f,  1.5f * lerp)).
                              Set(4, midPoint).
                              SetColor(COLOR.green.lime.A(.5f)).SetDepth(Level.GetPlaneDist(side, -.55f)).Fill();
        }
        
        
        if (predictionShapes && _swing.DebugCrash)
        {
            Vector3 stickPos = _swing.GetStick(GTime.Now).Item.GetLagPos(GTime.Now);
            Color c = Random.Range(0, 4) == 0 ? COLOR.red.hot : COLOR.red.firebrick;
            DRAW.ZappCircle(stickPos.V2() + V2.Random(.1f), .8f , .5f, 11, Time.realtimeSinceStartup * -180).
                SetColor(c).SetDepth(Level.GetPlaneDist(side, 1)).Fill();
        }
    }


    private static int ppDebug, arcDebug, circDebug;
}
