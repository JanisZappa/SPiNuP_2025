using TMPro;
using UnityEngine;


public class UI_SpinUp : ActiveUI 
{
    public GameObject[] buttons;
    
    [Space(10)]
    public TimeSliderUI slider;
    public TextMeshProUGUI pauseButtonText;
    

    public override bool HitUI(int click)
    {
        for (int e = 0; e < buttons.Length; e++)
            if (UI_Manager.ImPointedAt(buttons[e]))
            {
                if(click == 1)
                    ButtonPress(buttons[e].name);
                
                return true;
            }
        
        return false;
    }


    private void ButtonPress(string buttonName)
    {
        if (Input.GetMouseButtonDown(0))
            switch (buttonName)
            {
                case "PauseButton":
                    GTime.SetPause(!GTime.Paused);
                    pauseButtonText.text = GTime.Paused ? "#" : "II";
                    break;
                    
                case "HandleCast":
                    slider.GrabbingHandle();
                    break;
                
                case "PlayButton":
                    TimeSliderUI.PlayButtonPress();
                    break;
            }
    }
}
