using LevelElements;
using UnityEngine;


public static class LinkEdit
{
    public  static bool LinkMode, Creating;
    
    private static Element a;
    public  static linkType linkType;


    public static void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LinkMode = !LinkMode;
            UI_Creator.LinkButton.SetActive(LinkMode);
        }

        if (!LinkMode)
        {
            if (Creating)
            {
                a = null;
                Creating = false;
            }
            
            return;
        }   

        if (Input.GetKeyDown(KeyCode.Delete))
            if (Creating)
            {
                a = null;
                Creating = false;
            }
            else
            if(!LevelCheck.ClosestLink.Equals(Link.None))
                Link.Delete(LevelCheck.ClosestLink);
            
        if(Input.GetKeyDown(KeyCode.O))
            UI_Creator.LinkButton.NextPick();
        
        
        Element b = Creating? LevelCheck.ClosestElement : null;
        if (b != null && (a == b || a.side != b.side || linkType == linkType.Action && !Mask.Action.Fits(b.elementType)))
            b = null;
        
        
        if (!UI_Manager.HitUI && Input.GetMouseButtonDown(0))
        {
            if (!Creating)
            {
                a = LevelCheck.ClosestElement;
                Creating = a != null;
            }
            else
                if (b != null)
                {
                    Creating = false;
                    Link.Create(linkType, a, b);
                }
        }
        

        if (Creating)
        {
            Color c = linkType.Color();
            
            float   radius = Mask.AnyThing.Fits(a.elementType) ? ((Item) a).radius : .2f;
            Vector2 pos    = a.GetPos(GTime.Now);
            float   pump   = Mth.SmoothPP(.8f, 1, Time.realtimeSinceStartup * 2);
            DRAW.ZappCircle(pos, radius + pump, .6f, 12, Time.realtimeSinceStartup * 45).SetColor(c).SetDepth(Z.W05).Fill();

            
            Vector2 posB  = b != null? LevelCheck.ElementPoint : Level.HitPoint;
            Link.DrawArrow(pos, posB, linkType, false, b as Item);
        }
    }
}
