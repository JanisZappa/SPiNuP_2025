using LevelElements;
using UnityEngine;


public partial class Creator
{
    private static void NewElement(ref bool updateHouseGen)
    {
        if (!Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
            return;

        if(Mask.AnyThing.Fits(currentElementType))
            CreateNewItem();
        else
            ElementEdit.Select(CreateNewTrack());
        
        updateHouseGen = true;
    }
    
    
    private static void CreateNewItem()
    {
        bool addToTrack = ElementEdit.element != null && Mask.IsTrack.Fits(ElementEdit.elementType);
        if (addToTrack && !Mask.CanMove.Fits(currentElementType))
            return;

        
        Item itemA = Item.GetFreeItem;
             itemA.SetType(currentElementType).SetSide(GameCam.CurrentSide);

        if (!addToTrack)
        {
            itemA.SetRootPos(Level.HitPoint).Refresh();

            if (Mask.Warp.Fits(currentElementType))
            {
                bool warpIsExit   =  Mask.Warp_Start.Fits(currentElementType);
                bool sameSideWarp = !Mask.Warp_SideSwitch.Fits(currentElementType);
                
                Item itemB = Item.GetFreeItem;
                     itemB.SetType(currentElementType.GetOtherEnd()).SetSide(sameSideWarp? itemA.side : !itemA.side);

                if (sameSideWarp)
                    itemB.SetRootPos(itemA.rootPos + new Vector2(0, Level.PlacementRadius * 2 + itemA.radius * 2));
                else
                    itemB.SetRootPos(itemA.rootPos);
                
                itemB.Refresh();
                
                Link.Create(linkType.Warp,  warpIsExit? itemA : itemB, warpIsExit? itemB : itemA);
            }
        }
        else
            ElementEdit.track.ParentThis(itemA).ScanAndRefesh();
        

        if (itemA.ID == 1 && Item.Count == 1)
            Level.StartStick = itemA;
    }
    
    
    private static Track CreateNewTrack()
    {
        Track track = Track.GetNewTrack(currentElementType);
              track.SetSide(new Side(GameCam.CurrentSide.front)).
                    SetRootPos(Level.HitPoint).Refresh();

        return track;
    }
    
    
    private static void DeleteSelection(ref bool updateHouseGen)
    {
        if (!Input.GetKeyDown(KeyCode.Delete))
            return;

        if (Mask.AnyThing.Fits(ElementEdit.elementType))
        {
            if (Item.Count == 1 || Level.StartStick.Equals(ElementEdit.element))
                Level.StartStick = null;
            
            ((Item)ElementEdit.element).Delete();
        }
        else
            ElementEdit.track.Reset();
        
        updateHouseGen = true;

        ElementEdit.Select(null);
    }
    
    
    private static void Duplicate(ref bool updateHouseGen)
    { 
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl))
        {
        //  Item  //
            if (Mask.AnyThing.Fits(ElementEdit.elementType))
            {
                if (ElementEdit.item.parent == null)
                {
                    Item oldItem = (Item)ElementEdit.element;

                    Item newItem = Item.GetFreeItem;
                         newItem.SetType(oldItem.elementType).
                                 SetRootPos(Level.HitPoint).
                                 SetSide(oldItem.side).
                                 Refresh();
                         
                    ElementEdit.Select(newItem);

                    Element oldBuddy = Link.GetWarpBuddy(oldItem);

                    if (oldBuddy != null)
                    {
                        bool warpIsExit = Mask.Warp_Start.Fits(oldItem.elementType);

                        Item newOtherItem = Item.GetFreeItem;
                             newOtherItem.SetType(oldBuddy.elementType).
                                          SetRootPos(Level.HitPoint).
                                          SetSide(oldBuddy.side).
                                          Refresh();

                        Link.Create(linkType.Warp, warpIsExit ? newItem : newOtherItem, warpIsExit ? newOtherItem : newItem);
                    }
                }
            }
        //  Track  //
            else
            {
                Track newTrack = Track.GetNewTrack(ElementEdit.elementType);
                      newTrack.SetRootPos(Level.HitPoint).SetSide(ElementEdit.side);
                      
                      newTrack.offset = ElementEdit.track.offset;
                      newTrack.speed  = ElementEdit.track.speed;
                      newTrack.angle  = ElementEdit.track.angle;
                      newTrack.size   = ElementEdit.track.size;
                
                if (Mask.TrackCanGrow.Fits(newTrack.elementType))
                    ((Track_Arc) newTrack).growth = ((Track_Arc) ElementEdit.track).growth;

                for (int i = 0; i < ElementEdit.track.itemCount; i++)
                {
                    Item newItem = Item.GetFreeItem;
                         newItem.SetType(ElementEdit.track.items[i].elementType);
                         
                    newTrack.ParentThis(newItem);
                }
                
                newTrack.ScanAndRefesh();

                ElementEdit.Select(newTrack);
            }
            
            updateHouseGen = true;
        }
    }
    

    private static void GetVariation()
    {
        Item item = (Item)LevelCheck.ClosestElement;
        
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            elementType newType = item.elementType;
            
                for (int i = 0; i < Groups.All.Length; i++)
                    if (Groups.All[i].Contains(item.elementType))
                    {
                        newType = Groups.All[i].Next(item.elementType);
                        break;
                    }
            
            if (newType != item.elementType)
            {
                Item newItem = Item.GetFreeItem;
                     newItem.SetType(newType);
                    
                if (item.parent == null)
                {
                    newItem.SetRootPos(item.rootPos).SetSide(item.side).Refresh();
                    item.Delete();
                }
                else
                    item.parent.ReplaceAt((Item) LevelCheck.ClosestElement, newItem).ScanAndRefesh();

                UI_Creator.SetCurrentElement(newType.ToString());
            }
        }
    }
}
