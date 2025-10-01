using System.Collections.Generic;
using Clips;
using GameModeStuff;
using GeoMath;
using LevelElements;
using UnityEngine;


public enum linkType { Jump, Path, Action, Warp, Look = 5, Goal }

public struct Link
{
    public readonly linkType linkType;
    public readonly Element a, b;

    private Link(linkType linkType, Element a, Element b)
    {
        this.linkType   = linkType;
        this.a = a;
        this.b = b;
    }


    public static void Create(linkType linkType, Element a, Element b)
    {
        (a.side == Side.Front ? frontLinks : backLinks).Add(new Link(linkType, a, b));
    }


    public static void Delete(Link link)
    {
        for (int i = 0; i < frontLinks.Count; i++)
            if (frontLinks[i].Equals(link))
            {
                frontLinks.RemoveAt(i);
                i--;
            }
        
        for (int i = 0; i < backLinks.Count; i++)
            if (backLinks[i].Equals(link))
            {
                backLinks.RemoveAt(i);
                i--;
            }
    }


    public static void ResetAll()
    {
         backLinks.Clear();
        frontLinks.Clear();
    }


    public static List<Link> GetAllToSave
    {
        get
        {
            List<Link> all = new List<Link>();
            int fCount = frontLinks.Count;
            for (int i = 0; i < fCount; i++)
                all.Add(frontLinks[i]);
            int bCount = backLinks.Count;
            for (int i = 0; i < bCount; i++)
                all.Add(backLinks[i]);
            
            return all;
        }
    }


    public bool Equals(Link other)
    {
        return a == other.a && b == other.b && linkType == other.linkType;
    }
    
    
    public void Draw(Item swingItem)
    {
        if(a.side != b.side)
            return;
        
        Vector2 pA = a.GetPos(GTime.Now);
        Vector2 pB = b.GetPos(GTime.Now);
        
        if(new Bounds2D(pA).Add(pB).Intersects(a.side.front? GameCam.frustum.frontBounds : GameCam.frustum.backBounds))
        {
            if (linkType == linkType.Jump && !DrumRoll.IsActive(a))
                return;
            
            bool highlighted = GameManager.IsCreator && LevelCheck.ClosestLink.Equals(this) || 
                               !GameManager.IsCreator && swingItem == a;
            
            DrawArrow(pA, pB, linkType, highlighted, b as Item);
        }
    }


    public static void DrawArrow(Vector2 pA, Vector2 pB, linkType linkType, bool highLight, Item item = null)
    {
        Color c = linkType.Color().Multi(highLight? 1.75f : 1);
        
        
        Vector2 dir = pB - pA;
        if (item != null)
        {
            float dir_M = dir.magnitude;
            dir *= 1f / dir_M * (dir_M - item.radius);
        }
            
        
        float arrowTip = highLight ? Mth.SmoothPP(.5f, .8f, Time.realtimeSinceStartup * 6) : .5f;
            
        switch (linkType)
        {
            default: 
                DRAW.ZappArrow(pA, dir, arrowTip, 14, .65f, 4).SetColor(c).SetDepth(Z.W05).Fill(.5f);
                break;
               
            case linkType.Path:
                DRAW.ZappArrow(pA, dir, arrowTip, 24, .45f, 4).SetColor(c).SetDepth(Z.W05).Fill(.5f);
                break;
               
            case linkType.Look:
                DRAW.ZappArrow(pA, dir, arrowTip, 6, 1, 4).SetColor(c).SetDepth(Z.W05).Fill(.5f);
                break;
               
            case linkType.Warp:
                DRAW.Arrow(pA, dir, arrowTip).SetColor(c).SetDepth(Z.W05).Fill(.5f);
                break;
            
            case linkType.Action:
                DRAW.ZappArrow(pA, dir, arrowTip, 10, .3f, 4).SetColor(c).SetDepth(Z.W05).Fill(.5f);
                break;
        }
    }


    public static Link None;
    public static readonly List<Link> frontLinks, backLinks;


    public static void SwingResponse(Swing swing)
    {
        Item swingItem = swing.startStick.Item;
        
        List<Link> linkList = swing.startSide.front? frontLinks : backLinks;
        int count = linkList.Count;
        for (int i = 0; i < count; i++)
        {
            Link link = linkList[i];
            
            if(link.a != swingItem)
                continue;
            
            if(swing.spinner.isPlayer)
                switch (link.linkType)
                {
                    case linkType.Jump:        
                        if(DrumRoll.Start(link.a, link.b))    
                            MoveCam.LookAt(link);
                        break;
                    
                    case linkType.Action: 
                        ActorAnimator.ActorAction(link.b); 
                        break;
                    
                    case linkType.Goal: 
                        GameManager.ChangeState(GameState.GameWon); 
                        break;
                }
        
            switch (link.linkType)
            {
                case linkType.Warp:
                    swing.SetWarp((Item)link.b); 
                    break;
            }
        }
    }


    public static Element GetWarpBuddy(Element item)
    {
        List<Link> linkList = item.side.front? frontLinks : backLinks;
        int count = linkList.Count;
        for (int i = 0; i < count; i++)
        {
            Link link = linkList[i];

            if (link.linkType == linkType.Warp)
            {
                if(link.a == item)
                    return link.b;
                if(link.b == item)
                    return link.a;
            }
        }
        
        return null;
    }
    
    static Link()
    {
        None = new Link();
        
        frontLinks = new List<Link>(10000);
        backLinks  = new List<Link>(10000);
    }
}




public static class DrumRoll
{
    static DrumRoll()
    {
        GameManager.OnGameStart += GameManagerOnOnGameStart;
    }
    
    private static Element jumpingFrom, jumpingTo;
    private static Sound.SoundObject rollSound;
    
    
    private static readonly int[] active = new int[Item.TotalCount + Track.TotalCount];
    private static int gameID;

    
    private static void GameManagerOnOnGameStart()
    {
        gameID++;
    }
    

    public static bool Start(Element jumpFrom, Element jumpTo)
    {
        if (active[jumpFrom.ID] != gameID)
        {
            jumpingFrom = jumpFrom;
            jumpingTo   = jumpTo;
            
            if (rollSound == null)
                rollSound = Sound.Get(Audio.Reaction.DrummRoll).Loop().Fade(.9f).Play();
            
            return true;
        }
        
        return false;
    }

    
    public static void Jump()
    {
        if (rollSound != null)
        {
            rollSound.Stop();
            rollSound = null;
            
            Sound.Get(Audio.Reaction.SnareHit).Play();
        }
    }


    public static void Swing(Element jumpTo)
    {
        if (jumpTo == jumpingTo)
        {
            Sound.Get(Audio.Reaction.CymbalHit).Volume(1, .125f).Pitch(1, .01f).Play();
            active[jumpingFrom.ID] = gameID;
        }

        jumpingFrom = null;
        jumpingTo   = null;
    }


    public static bool IsActive(Element jumpFrom)
    {
        return active[jumpFrom.ID] != gameID;
    }
}




public static class Warp
{
    public const float warpTime = .5f;
}


public static class LinkTypeExt
{
    public static Color Color(this linkType linkType)
    {
        switch (linkType)
        {
            default:              return COLOR.blue.cornflower;
            case linkType.Warp:   return COLOR.orange.coral;
            case linkType.Look:   return COLOR.green.spring;
            case linkType.Path:   return COLOR.purple.maroon;
            case linkType.Action: return COLOR.red.tomato;
        }
    }
}

