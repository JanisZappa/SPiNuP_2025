using System.Collections;
using GameModeStuff;
using UnityEngine;


public partial class Creator : IGameMode
{
    public void Load()
    {
        UI_Manager.ShowUI(UIType.Creator, true, true);
    }

    public void Restart(){}


    public void StateUpdate()
    {
        if (UI_Creator.SavingOrLoading || TappedUI)
        {
            TappedUI = false;
            return;
        }

        LevelCheck.CreatorUpdate();
        
        bool updateHouseGen = false;
        
        
    //  Functions  //
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                LinkEdit.Update();
                ElementEdit.Update(ref updateHouseGen);
            }

            if(currentElementType != 0)
                NewElement(ref updateHouseGen);
            
            if (LevelCheck.Highlighting && Mask.AnyThing.Fits(LevelCheck.ClosestElement.elementType))
                GetVariation();
            
            if (ElementEdit.element != null)
            {
                  DeleteSelection(ref updateHouseGen);
                        Duplicate(ref updateHouseGen);
                EditSelectionSide(ref updateHouseGen);
                CenterCam();

            //  If Item  //
                if (Mask.AnyThing.Fits(ElementEdit.elementType))
                    SetStartStick();
                
            //  If Track  //
                else
                    if (Input.GetKey(KeyCode.T))
                    {
                        AddTrackGap();
                        EditTrackSize(ref updateHouseGen);
                        EditTrackSpeed();
                        EditTrackOffset();
                        EditTrackAngle(ref updateHouseGen);
                        EditTrackCompletion(ref updateHouseGen);
                    }
            }

            Saving();
        } 
        
        LevelDebug.ShowHitPoint();
        
        if(updateHouseGen)
            HouseGen.GameStart();
    }


    public IEnumerator GameOver()     { yield break; }
}

