using GameModeStuff;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour 
{
    public InputField inputField;
    
    
    private void OnEnable()
    {
        inputField.text = UserSave.Name;
    }

    
	public void SelectSpinUp()
    {
        GameManager.LoadGameMode(Mode.SpinUp);
    }
}
