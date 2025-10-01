using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
	public GameObject nameBttn, emailBttn, passwordBttn, loginBttn;

	
	[Space(10)] 
	public GameObject abcButton;
	public GameObject numberButton;

	[Space(10)] 
	public GameObject screenA; 
	public GameObject screenB;
	public TextMeshProUGUI wait;
	

	private TextMeshProUGUI name, email, password;
	
	
	private EventSystem system;
	private PointerEventData pointer;
	private List<RaycastResult> Hits;

	private TextMeshProUGUI editText;
	private string editString;


	private KeyboardButton[] buttons;

	private KeyboardButton space, back;

	private KeyboardButton login;

	
	public enum editMode { None, Name, Email, Password }
	private editMode mode;

	private bool loggingIn;

	private int playerID;

	
	[Serializable]
	public class KeyboardButton
	{
		public GameObject button;

		private readonly TextMeshProUGUI text;
		public string upper, lower;
		public string addString;

		private readonly Image image;
		private bool active;

		
		public KeyboardButton(GameObject button, string character)
		{
			this.button = button;
			upper = character;
			lower = upper == "@" || upper == "." ? upper : upper.ToLower();

			text = button.GetComponentInChildren<TextMeshProUGUI>();

			text.text = upper;

			image = button.GetComponent<Image>();
		}


		public bool GotHit(GameObject hitObject)
		{
			return active && button == hitObject;
		}


		public void SetActive(editMode mode, string editString)
		{
			switch (mode)
			{
				default:
					active = false;
					break;
				
				case editMode.Name:
					active = upper != "." && upper != "@";
					break;
				
				case editMode.Email:
					active = true;
					break;
				
				case editMode.Password:
					active = upper != "." && upper != "@";
					break;
			}

			if (upper == "Back")
				active = mode != editMode.None && editString.Length > 0;
			
			if (upper == "Space")
				active = mode != editMode.None && editString.Length > 0;
			
			
			SetActive(active);
		}


		public void SetActive(bool active)
		{
			this.active = active;
			
			image.color = this.active ? Color.white : new Color(.75f, .75f, .75f, 1);
		}


		public void SetCaps(bool big)
		{
			addString = big ? upper : lower;
			text.text = addString;
		}
	}

	
	private void Start ()
	{
		screenB.SetActive(false);
		
		
		name     =     nameBttn.GetComponentInChildren<TextMeshProUGUI>();
		email    =    emailBttn.GetComponentInChildren<TextMeshProUGUI>();
		password = passwordBttn.GetComponentInChildren<TextMeshProUGUI>();
		
		
		LoadLocalLoginFile();
		
		
		system  = EventSystem.current;
		pointer = new PointerEventData(system);
		Hits    = new List<RaycastResult>();


		const string leftButtons  = "ABCDEFGHIJKLMNOPQRSTUVWXYZ@.";
		const string rightButtons = "3216549870?!";
		
		buttons = new KeyboardButton[leftButtons.Length + rightButtons.Length];
		
		Transform parent = abcButton.transform.parent;

		const int buttonsPerRow = 7, buttonsPerRow2 = 3, 
			                gap = 3, 
			              width = 25,
			             height = 35;

		
		for (int i = 0; i < leftButtons.Length; i++)
		{
			GameObject newButton = Instantiate(abcButton, parent);

			int y = Mathf.FloorToInt((float) i / buttonsPerRow);
			int x = i % buttonsPerRow;
			newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(x *  (width + gap), 
				                                                                   y * -(height + gap));
			
			buttons[i] = new KeyboardButton(newButton, leftButtons[i].ToString());
			buttons[i].SetActive(mode, editString);
		}
		
		for (int e = 0; e < rightButtons.Length; e++)
		{
			GameObject newButton = Instantiate(numberButton, parent);

			int y = Mathf.FloorToInt((float) e / buttonsPerRow2);
			int x = e % buttonsPerRow2;
			newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(x * -(width  + gap), 
				                                                                   y * -(height + gap));
			
			int i = e + leftButtons.Length;
			buttons[i] = new KeyboardButton(newButton, rightButtons[e].ToString());
			buttons[i].SetActive(mode, editString);
		}
		
	//  Space and Return  //
		{
			GameObject newButton = Instantiate(abcButton, parent);
			
			RectTransform rect = newButton.GetComponent<RectTransform>();
			              rect.anchoredPosition = new Vector3(0,  4 * -(height + gap));
			              rect.sizeDelta        = new Vector2(buttonsPerRow * width + (buttonsPerRow - 1) * gap, height);
			
			space = new KeyboardButton(newButton, "Space");
			space.SetActive(mode, editString);
		}
		{
			GameObject newButton = Instantiate(numberButton, parent);
			
			RectTransform rect = newButton.GetComponent<RectTransform>();
			              rect.anchoredPosition = new Vector3(0,  4 * -(height + gap));
			              rect.sizeDelta        = new Vector2(buttonsPerRow2 * width + (buttonsPerRow2 - 1) * gap, height);
			
			back = new KeyboardButton(newButton, "Back");
			back.SetActive(mode, editString);
		}
		
		Destroy(abcButton);
		Destroy(numberButton);
		
		
	//  Cancel if no Internet  //
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			screenA.SetActive(false);
			
			if(Application.isEditor)
				SceneManager.LoadScene("main");
			else
				SceneManager.LoadScene(1);
			
			return;
		}
		
		
	//  Login Button  //
		login = new KeyboardButton(loginBttn, "Login");

		if (name.text.Length > 0 && password.text.Length > 0)
		{
			login.SetActive(true);
			
			LogIn();
		}
		else
			login.SetActive(false);
	}
	
	
	private void Update ()
	{
		if (loggingIn)
			return;
		
		if (Input.GetMouseButtonDown(0))
		{
			Hits.Clear();
			pointer.position = Input.mousePosition;
			system.RaycastAll(pointer, Hits);


			for (int i = 0; i < Hits.Count; i++)
			{
				GameObject hitObject = Hits[i].gameObject;

				if (login.GotHit(hitObject))
				{
					if (name.text.Length > 0 && password.text.Length > 0)
						LogIn();
					break;
				}
				
				if (hitObject == nameBttn)
				{
					editText   = editText == name? null : name;
					editString = name.text;
					
					SetEditMode(editText == name ? editMode.Name : editMode.None);
					break;
				}
				if (hitObject == emailBttn)
				{
					editText   = editText == email? null : email;
					editString = name.text;
					
					SetEditMode(editText == email ? editMode.Email : editMode.None);
					break;
				}
				if (hitObject == passwordBttn)
				{
					editText   = editText == password? null : password;
					editString = password.text;
					
					SetEditMode(editText == password ? editMode.Password : editMode.None);
					break;
				}

				bool hitKeyboard = false;
				if (editText != null)
				{
					for (int e = 0; e < buttons.Length; e++)
						if (buttons[e].GotHit(hitObject))
						{
							editString += buttons[e].addString;
							editText.text = editString;
							
							hitKeyboard = true;
							break;
						}

					if (!hitKeyboard && space.GotHit(hitObject))
					{
						editString += mode == editMode.Name ? " " : "_";
						editText.text = editString;
						hitKeyboard = true;
					}
					
					if (!hitKeyboard && back.GotHit(hitObject))
					{
						editString = editString.Substring(0, editString.Length - 1);
						editText.text = editString;
						hitKeyboard = true;
					}
				}


				if (hitKeyboard)
				{
					SetEditMode(mode);
					break;
				}	
			}
		}
	}


	private void SetEditMode(editMode mode)
	{
		this.mode = mode;
		
		
		for (int i = 0; i < buttons.Length; i++)
			buttons[i].SetActive(mode, editString);
		
		space.SetActive(mode, editString);
		back.SetActive(mode, editString);
		
		login.SetActive(name.text.Length > 0 && password.text.Length > 0);
		
		
	//  Upper or Lower  //
		switch (mode)
		{
			case editMode.Name:
				for (int i = 0; i < buttons.Length; i++)
					buttons[i].SetCaps(editString.Length == 0 || editString.Length >= 2 && editString[editString.Length - 1] == ' ');
				break;
			
			case editMode.Password:
				for (int i = 0; i < buttons.Length; i++)
					buttons[i].SetCaps(true);
				break;
			
			case editMode.Email:
				for (int i = 0; i < buttons.Length; i++)
					buttons[i].SetCaps(false);
				break;
		}
	}


	private void LogIn()
	{
		StartCoroutine(LoggingIn(name.text, password.text, accountID =>
		{
			if (accountID != 0)
			{
				SaveLocalLoginFile(name.text, password.text, accountID);
				
				if(Application.isEditor)
					SceneManager.LoadScene("main");
				else
					SceneManager.LoadScene(1);
			}
			else
			{
				screenA.SetActive(true);
				screenB.SetActive(false);
				
				DeleteLocalLoginFile();
			}
		}));
	}


	private IEnumerator LoggingIn(string name, string password, Action<int> accountIDCallback)
	{
		screenA.SetActive(false);
		screenB.SetActive(true);
		loggingIn = true;

		StartCoroutine(LogInAnim());
		
		WWWForm form = new WWWForm();
		        form.AddField("n", name);
		        form.AddField("p", password);

		bool loginFailed = false;
		UnityWebRequest www = UnityWebRequest.Post(Database.LoginPHP, form);
		yield return www.SendWebRequest();

		float t = 0;
		while (!www.isDone)
		{
			t += Time.deltaTime;
			if (t >= 3)
			{
				loginFailed = true;
				break;
			}

			yield return null;
		}
		
		if(!loginFailed)
			accountIDCallback(!www.isNetworkError && !www.isHttpError? int.Parse(www.downloadHandler.text) : 0);
		else
			accountIDCallback(1);
		
		www.Dispose();
		
		loggingIn = false;
	}


	private IEnumerator LogInAnim()
	{
		int pick = 0;

		while (loggingIn)
		{
			wait.text = waitTexts[pick];

			pick = (pick + 1) % waitTexts.Length;
			yield return new WaitForSeconds(.08f);
		}
	}


	private static readonly string[] waitTexts =
	{
		"Wait",
		"Wait .",
		"Wait ..",
		"Wait ..."
	};
	
	
	
//  Local Save File  //
	private static string Path { get { return Application.persistentDataPath + "/player.lgn"; }}

	
	private void LoadLocalLoginFile()
	{
		if (File.Exists(Path))
			using (MemoryStream m = new MemoryStream(File.ReadAllBytes(Path).UnJumble()))
				using (BinaryReader r = new BinaryReader(m))
				{
					    name.text = r.ReadString();
					password.text = r.ReadString();
					playerID      = r.ReadInt32();
				}
	}


	private static void SaveLocalLoginFile(string name, string password, int accountID)
	{
		using (MemoryStream m = new MemoryStream())
		using (BinaryWriter r = new BinaryWriter(m))
		{
			r.Write(name);
			r.Write(password);
			r.Write(accountID);

			File.WriteAllBytes(Path, m.GetBuffer().Jumble());
		}
	}


	private static void DeleteLocalLoginFile()
	{
		if (File.Exists(Path))
			File.Delete(Path);
	}
}
