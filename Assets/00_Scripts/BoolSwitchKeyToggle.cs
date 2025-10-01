using UnityEngine;


public class BoolSwitchKeyToggle : MonoBehaviour
{
	private void Update ()
	{
		switch (KeyMap.GetDownKey)
		{
			case KeyCode.T:	
				BoolSwitch.ToggleValue("Anim/Turn Test");
				break;
			
			case KeyCode.M:	
				BoolSwitch.ToggleValue("Audio/Mute");
				break;
			
			case KeyCode.R:		 
				BoolSwitch.ToggleValue("Anim/Wiggle Test");
				break;
			
			case KeyCode.V:		 
				BoolSwitch.ToggleValue("Dev/Replay Dir");
				break;
			
			case KeyCode.Equals: 
				BoolSwitch.ToggleValue("Visuals/Wireframe");
				break;
			
			case KeyCode.RightBracket: 
				BoolSwitch.ToggleValue("Visuals/ShadowTest");
				break;
			
			case KeyCode.Insert: 
				BoolSwitch.ToggleValue("Dev/Keycodes");
				break;
			
			case KeyCode.F1: 
				BoolSwitch.ToggleValue("Jump/Jump Lines");
				BoolSwitch.ToggleValue("Jump/Connection Shapes");
				BoolSwitch.ToggleValue("Jump/Zappy");
				break;
			
			case KeyCode.F2: 
				BoolSwitch.ToggleValue("Elements/Tracks");
				BoolSwitch.ToggleValue("Elements/StartStick");
				BoolSwitch.ToggleValue("Elements/Links");
				break;
			
			case KeyCode.F3: 
				BoolSwitch.ToggleValue("Bounds/Cells");
				BoolSwitch.ToggleValue("Bounds/Elements");
				BoolSwitch.ToggleValue("Bounds/Player");
				BoolSwitch.ToggleValue("Jump/Bounds");
				break;
		}
	}
}
