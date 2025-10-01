using System.Collections;
using LevelElements;
using UnityEngine;


public static class ElementEdit
{
    public static Element element;
    public static Item    item;
    public static Track   track;
    
    public static elementType elementType;
    public static Side        side;
    
    private static Vector2 placementOffset, mouseStart, oldPos, hitStart;
    private static bool    movingElement;
    

    public static void Select(Element element)
    {
        ElementEdit.element = element;

        if (ElementEdit.element != null)
        {
            elementType = element.elementType;
            side        = element.side;

            bool isItem = Mask.AnyThing.Fits(element.elementType);
        
            item  =  isItem?  (Item) element : null;
            track = !isItem? (Track) element : null;
        
            UI_Creator.SetCurrentElement(element.elementType.ToString());
        }
        else
        {
            item  = null;
            track = null;
            elementType = 0;
        }
    }


    public static void Update(ref bool updateHouseGen)
    {
        bool active = element != null;
        
        if (LinkEdit.LinkMode)
        {
            if(active)
                Select(null);
            
            return;
        }
        
        
        if (!UI_Manager.HitUI)
        {
            if (Input.GetMouseButtonDown(1) && active)
            {
                Select(null);
                return;
            }
            
            if (Input.GetKey(KeyCode.T))
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                hitStart = Level.HitPoint;
                
                Vector2 closestPoint;
                Element newElement = Search.ClosestElement(hitStart, GTime.Now, GameCam.CurrentSide, Creator.currentFilter, out closestPoint);
                
                if (newElement == null)
                    Select(null);
                else 
                    if (newElement == item && item.parent != null)
                    {
                        Select(item.parent);
                        UI_Creator.SetCategory(3);
                    }
                    else
                        Select(newElement);

                mouseStart = Input.mousePosition;
                
                return;
            }


            if (Input.GetMouseButton(0))
            {
                Vector2 hitNow = Level.HitPoint;
                Vector2 min    = new Vector2(Mathf.Min(hitStart.x, hitNow.x), Mathf.Min(hitStart.y, hitNow.y));
                Vector2 max    = new Vector2(Mathf.Max(hitStart.x, hitNow.x), Mathf.Max(hitStart.y, hitNow.y));
                
                Vector2 p = (min + max) * .5f, s = max - min;
                
                DRAW.Rectangle(p, s).SetColor(Color.yellow).SetDepth(Z.W05);
                DRAW.Rectangle(p, s).SetColor(Color.cyan  ).SetDepth(Z.W05).Fill(.1f, true);
                
                if (active)
                {
                    if (!movingElement && Vector2.Distance(Input.mousePosition, mouseStart) >= 7)
                    {
                        movingElement = true;
                        oldPos = element.rootPos;
                        placementOffset = oldPos - Level.HitPoint;
                    }


                    if (!movingElement)
                        return;

                    element.SetRootPos(Level.HitPoint + placementOffset).Refresh();
                    GTime.Now = GTime.LastFrame;

                //  Moving Other Side Warp Item as well  //
                    if (Mask.Warp_SideSwitch.Fits(elementType))
                    {
                        Element buddy = Link.GetWarpBuddy(element);

                        buddy?.SetRootPos(element.rootPos).Refresh();
                    }
                }
            }
        }


        if (Input.GetMouseButtonUp(0) && movingElement)
        {
            movingElement = false;
            if (element != null)
                if(LevelCheck.Overlapping(element))
                    Run.Inst.StartCoroutine(MoveBack());
                else
                    updateHouseGen = true;
        }
        

        if (Input.GetKeyDown(KeyCode.Backspace) && active)
            Select(null);
    }


    private static IEnumerator MoveBack()
    {
        Vector3 start = element.rootPos;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4;
            element.SetRootPos(Vector2.Lerp(start, oldPos, Mathf.SmoothStep(0, 1, t))).Refresh();
            yield return null;
        }
    }
}
