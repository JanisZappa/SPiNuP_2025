using LevelElements;
using UnityEngine;


public partial class Creator
{
    private bool  dragAdjusting, rotate, adjustOffset;
    private float startAngle, startOffset, mouseStartX;
    

    private static void EditTrackSize(ref bool updateHouseGen)
    {
        if (!KeyMap.Hold(KeyCode.Keypad4, KeyCode.Alpha3) && !KeyMap.Hold(KeyCode.Keypad6, KeyCode.Alpha4))
            return;

        if (KeyMap.Hold(KeyCode.Keypad4, KeyCode.Alpha3))
        {
            ElementEdit.track.SetSize(ElementEdit.track.size - Time.deltaTime * .6f).Refresh();
            updateHouseGen = true;
        }

        if (KeyMap.Hold(KeyCode.Keypad6, KeyCode.Alpha4))
        {
            ElementEdit.track.SetSize(ElementEdit.track.size + Time.deltaTime * .6f).Refresh();
            updateHouseGen = true;
        }

        GTime.Now = GTime.LastFrame;
    }


    private static void EditTrackSpeed()
    {
        if (KeyMap.Down(KeyCode.Keypad2, KeyCode.Alpha1))
            ElementEdit.track.ShiftSpeed(false);

        if (KeyMap.Down(KeyCode.Keypad8, KeyCode.Alpha2))
            ElementEdit.track.ShiftSpeed(true);
    }


    private void EditTrackOffset()
    {
        if (Input.GetMouseButtonDown(2) && !dragAdjusting)
        {
            dragAdjusting = adjustOffset = true;
            startOffset   = ElementEdit.track.offset;
            mouseStartX   = Input.mousePosition.x;
        }


        if (Input.GetMouseButton(2) && adjustOffset)
        {
            ElementEdit.track.SetOffset(Mathf.Repeat(startOffset + (Input.mousePosition.x - mouseStartX) / Screen.width * 5, 1));
            GTime.Now = GTime.LastFrame;
        }


        if (Input.GetMouseButtonUp(2))
        {
            if (adjustOffset && dragAdjusting)
            {
                if (f.Same(ElementEdit.track.offset, startOffset))
                    ElementEdit.track.SetOffset(0);

                dragAdjusting = false;
            }

            adjustOffset = false;
        }
    }
    
    
    private static void EditTrackCompletion(ref bool updateHouseGen)
    {
        if (!KeyMap.Hold(KeyCode.Keypad7, KeyCode.Alpha5) && !KeyMap.Hold(KeyCode.Keypad9, KeyCode.Alpha6))
            return;

        if (KeyMap.Hold(KeyCode.Keypad7, KeyCode.Alpha5))
        {
            ElementEdit.track.SetCompletion(ElementEdit.track.growth - Time.deltaTime * .6f).Refresh();
            updateHouseGen = true;
        }
            
        if (KeyMap.Hold(KeyCode.Keypad9, KeyCode.Alpha6))
        {
            ElementEdit.track.SetCompletion(ElementEdit.track.growth + Time.deltaTime * .6f).Refresh();
            updateHouseGen = true;
        }
            

        GTime.Now = GTime.LastFrame;
    }


    private void EditTrackAngle(ref bool updateHouseGen)
    {
        if (Input.GetMouseButtonDown(0) && !dragAdjusting)
        {
            dragAdjusting = true;
            rotate        = Mask.TrackCanTurn.Fits(ElementEdit.elementType);
            startAngle    = ElementEdit.track.angle;
            mouseStartX   = Input.mousePosition.x;
        }


        if (Input.GetMouseButton(0) && rotate)
        {
            ElementEdit.track.SetAngle(startAngle + (Input.mousePosition.x - mouseStartX) / Screen.width * 250).Refresh();
            updateHouseGen = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (rotate && dragAdjusting)
                dragAdjusting = false;

            rotate = false;
        }
    }


    private static void EditSelectionSide(ref bool updateHouseGen)
    {
        if (!Input.GetKeyDown(KeyCode.Keypad3))
            return;

        
        ElementEdit.element.SetSide(!ElementEdit.side).Refresh();

        Element buddy = Link.GetWarpBuddy(ElementEdit.element);
        buddy?.SetSide(!buddy.side).Refresh();

        updateHouseGen = true;
    }


    private static void AddTrackGap()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Item gap = Item.GetFreeItem;
            gap.SetType(elementType.TrackGap).SetSide(ElementEdit.track.side);
            ElementEdit.track.ParentThis(gap).ScanAndRefesh();
        }
    }
}
