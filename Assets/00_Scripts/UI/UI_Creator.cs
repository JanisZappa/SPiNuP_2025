using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LevelElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Creator : ActiveUI
{
    public RectTransform parent;
    
//  Save  //
    [Space(10)]
    public GameObject savePrompt;
    public TextMeshProUGUI saveName;
    public GameObject saveButton, xButton, newButton;
    
    
//  Overwrite  //
    [Space(10)] 
    public GameObject overWritePrompt;
    public GameObject overwriteButton, returnButton;

    private static List<UI_Button> categorys;
    private static List<List<UI_Button>> categoryButtons;
    public static UI_LinkButton LinkButton;
    
    private readonly List<elementType> allElements = new List<elementType>();

    private static bool ShowingSavePrompt { get { return Inst != null && Inst.savePrompt.activeInHierarchy; }      set { Inst.savePrompt.SetActive(value); }}
    private static bool ShowingOverwrite  { get { return Inst != null && Inst.overWritePrompt.activeInHierarchy; } set { Inst.overWritePrompt.SetActive(value); }}
    public  static bool SavingOrLoading   { get { return ShowingSavePrompt || ShowingOverwrite; }}


    private static readonly ElementMask[] masks = { Mask.IsItem, Mask.IsCollectable, Mask.IsFluff, Mask.IsTrack };
    private static readonly string[]  names = { "Items", "Stuff", "Fluff", "Tracks"};
    
    private const int columns = 5, rowButtons = columns - 1;

    private static UI_Creator Inst;

    private static Group[] MaskGroups(ElementMask mask)
    {
        List<Group> returnGroups = new List<Group>();
        
        for (int i = 0; i < Groups.All.Length; i++)
        {
            Group group = Groups.All[i];
            if(mask.Fits(group.Get(0)))
                returnGroups.Add(group);
        } 

        return returnGroups.ToArray();
    }
    

    public override void OnEnable()
    {
        Inst = this;
        
        ShowingSavePrompt = ShowingOverwrite = false;


        categoryButtons = new List<List<UI_Button>>();

        Color[] active = { COLOR.blue.cornflower, COLOR.yellow.fresh, COLOR.green.spring };

        for (int i = 0; i < names.Length; i++)
        {
            List<UI_Button> buttonList = new List<UI_Button>();
            int buttonIndex = 0;
            
            Color colorA = active[i % 3];
            
        //  Create Groups  //
        Group[][] groups = new Group[4][];
        groups[0] = MaskGroups(Mask.IsItem);
        groups[1] = MaskGroups(Mask.IsCollectable);
        groups[2] = MaskGroups(Mask.IsFluff);
        groups[3] = MaskGroups(Mask.IsTrack);
            
        //  Group Buttons  //
            for (int e = 0; e < groups[i].Length; e++)
            {
                int x = buttonIndex % rowButtons;
                int y = 1 + Mathf.FloorToInt(buttonIndex / (float)rowButtons);
                
                buttonList.Add(new UI_GroupButton(new Vector2(x, y), colorA, groups[i][e]));

                buttonIndex++;
            }
            

            foreach (elementType elementT in Enum.GetValues(typeof(elementType)))
                if (Mask.CreatorButton.Fits(elementT) && masks[i].Fits(elementT))
                {
                //  Skip Grouped Items  //
                    bool skip = false;
                    for (int e = 0; e < groups[i].Length; e++)
                        if (groups[i][e].Contains(elementT))
                        {
                            skip = true;
                            break;
                        }

                    if(skip)
                        continue;
                    
                    int index = allElements.Count;
                    allElements.Add(elementT);

                    int x = buttonIndex % rowButtons;
                    int y = 1 + Mathf.FloorToInt(buttonIndex / (float)rowButtons);

                    buttonList.Add(new UI_ElementButton(new Vector2(x, y), colorA, elementT.ToString(),
                        () => { Creator.SetElementType(allElements[index]); }));

                    buttonIndex++;
                }

            categoryButtons.Add(buttonList);
        }


    //  Links  //
        LinkButton = new UI_LinkButton(Color.grey, "", new Vector2(rowButtons, 0));

        
        categorys = new List<UI_Button>();
        for (int i = 0; i < names.Length; i++)
        {
            int index = i;
            categorys.Add(new UI_ElementButton(new Vector2(i, 0), Color.grey, names[i], () => { SetCategory(index); }));
        }

        categorys[0].SetActive(true);
        
        base.OnEnable();
    }
    
    
    private void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.T))
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) SetCategory(0);
            if(Input.GetKeyDown(KeyCode.Alpha2)) SetCategory(1);
            if(Input.GetKeyDown(KeyCode.Alpha3)) SetCategory(2);
            if(Input.GetKeyDown(KeyCode.Alpha4)) SetCategory(3);  
        }
    }
    
    
    public override bool HitUI(int click)
    {
        if (Inst == null)
            return false;
        

        if (!SavingOrLoading && UI_Button.PointedAt(click == 1))
            return true;
        
        if (UI_Manager.ImPointedAt(saveButton) && Inst.saveName.text != "")
        {
            if (click == 1)
            {
                if (LevelSaveLoad.NewSaveFile(Inst.saveName.text))
                    LevelSaveLoad.SaveLevel(Inst.saveName.text);
                else
                {
                    ShowingOverwrite = true;
                    return true;
                }

                ShowingSavePrompt = false;
            }

            return true;
        }
        
        
        if (UI_Manager.ImPointedAt(newButton))
        {
            if (click == 1)
            {
                Creator.NewLevel();
                ShowingSavePrompt = false;
            }

            return true;
        }
        
        if (UI_Manager.ImPointedAt(xButton))
        {
            if (click == 1)
                ShowingSavePrompt = false;
            
            return true;
        }


        if (UI_Manager.ImPointedAt(overwriteButton))
        {
            if (click == 1)
            {
                LevelSaveLoad.SaveLevel(Inst.saveName.text);
                ShowingOverwrite = false;
                ShowingSavePrompt = false;
            }

            return true;
        }
        
        if (UI_Manager.ImPointedAt(returnButton))
        {
            if (click == 1)
            {
                ShowingOverwrite  = false;
                ShowingSavePrompt = true;
            }

            return true;
        }

        return false;
    }

    
    public static void SetCategory(int category)
    {
        for (int i = 0; i < categorys.Count; i++)
            categorys[i].StayActive(i == category);
        
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            bool visible = i == category;
            for (int e = 0; e < categoryButtons[i].Count; e++)
                categoryButtons[i][e].Show(visible);
        }

        Creator.currentFilter = category < 4 ? masks[category] : null;

        UI_Button.SetSelectedName(names[category]);
    }


    public static void SetCurrentElement(string name)
    {
        UI_Button.SetSelectedName(name);
    }

    
    public static void ShowSavePrompt(string currentLevelName)
    {
        ShowingSavePrompt  = true;
        Inst.saveName.text = currentLevelName != UI_LevelList.NewLevel ? currentLevelName : "";
    }
    
    
    [Serializable]
    private class UI_ElementButton : UI_Button
    {
        private readonly Action _clickAction;
        private bool stayActive;
        

        public UI_ElementButton(Vector2 offset, Color colorOn, string name, Action clickAction): base(colorOn, name, offset)
        {
            _clickAction = clickAction;
            
            text.text = Regex.Replace(name, "(\\B[A-Z])", " $1").Replace("_","").ToUpper();
        }


        public override void SetActive(bool active, bool fireAction = true)
        {
            image.color = active || stayActive? colorOn     : colorOff;
            text.color  = active || stayActive? Color.black : Color.white;

            if (active)
            {
                if(fireAction)
                    _clickAction();

                for (int i = 0; i < allButtons.Count; i++)
                    if(allButtons[i] != this)
                        allButtons[i].SetActive(false, false);
            } 
        }


        public override void StayActive(bool stayActive)
        {
            this.stayActive = stayActive;
        }
        
        
        protected override bool PointingAtMe(bool click)
        {
            if (visible && UI_Manager.ImPointedAt(gameObject))
            {
                if(click)
                    SetActive(true);
                
                return true;
            }

            return false;
        }


        protected override bool CheckNameMatch(string name)
        {
            if (this.name == name)
            {
                SetActive(true, false);
                return true;
            }
            
            return false;
        }
    }
    

    [Serializable]
    private class UI_GroupButton : UI_Button
    {
        private readonly Group group;
        private int pick;
        private float scroll;


        private void UpdateText()
        {
            string t = Regex.Replace(group.Get(pick).ToString(), "(\\B[A-Z])", " $1").Replace("_","").ToUpper();
            text.text = t + " " + (pick + 1) + "/" + group.Length;
        }
        
        
        public UI_GroupButton(Vector2 offset, Color colorOn, Group group): base(colorOn, "", offset)
        {
            this.group = group;
            UpdateText();
        }
        
        
        public override void SetActive(bool active, bool fireAction = true)
        {
            image.color = active? colorOn     : colorOff;
            text.color  = active? Color.black : Color.white;

            if (active)
            {
                if(fireAction)
                    Creator.SetElementType(group.Get(pick));
                
                for (int i = 0; i < allButtons.Count; i++)
                    if(allButtons[i] != this)
                        allButtons[i].SetActive(false, false);
            } 
        }

        
        protected override bool PointingAtMe(bool click)
        {
            if (visible && UI_Manager.ImPointedAt(gameObject))
            {
                if(click)
                    SetActive(true);
                else
                {
                    scroll += Controll.ScrollWheelDelta * 1000;
                    if (Mathf.Abs(scroll) >= 1)
                    {
                        pick = (pick + (int)Mathf.Sign(scroll)).Repeat(group.Length);
                        UpdateText();
                        Creator.SetElementType(group.Get(pick));
                        scroll = 0;
                    }
                }
                
                return true;
            }

            return false;
        }

        
        protected override bool CheckNameMatch(string name)
        {
            for (int i = 0; i < group.Length; i++)
                if (group.Get(i).ToString().Contains(name))
                {
                    pick = i;
                    UpdateText();
                    SetActive(true, false);
                    return true;
                }
            
            return false;
        }
    }


    [Serializable]
    public class UI_LinkButton : UI_Button
    {
        private int pick;
        private float scroll;
        private static readonly linkType[] validLinks = { linkType.Jump, linkType.Path, linkType.Action, linkType.Look };
        private bool active;
        
        public UI_LinkButton(Color colorOn, string name, Vector2 offset) : base(colorOn, name, offset)
        {
            UpdateText();
        }
        
        
        private void UpdateText()
        {
            text.text = active? validLinks[pick].ToString().ToUpper() + " " + (pick + 1) + "/" + validLinks.Length : "EDIT LINKS";
            LinkEdit.linkType = validLinks[pick];
            
            image.color = active? validLinks[pick].Color().Multi(.75f) : colorOff;
            text.color  = active? Color.black : Color.white;
        }
        

        public override void SetActive(bool active, bool fireAction = true)
        {
            if (fireAction)
            {
                this.active = active;
                LinkEdit.LinkMode = active;
                UpdateText();
            }
        }


        public void NextPick()
        {
            pick = (pick + (int)Mathf.Sign(scroll)).Repeat(validLinks.Length);
            UpdateText();
        }

        
        protected override bool PointingAtMe(bool click)
        {
            if (UI_Manager.ImPointedAt(gameObject))
            {
                if(click)
                    SetActive(!active);
                else
                    if(active)
                    {
                        scroll += Controll.ScrollWheelDelta * 1000;
                        if (Mathf.Abs(scroll) >= 1)
                        {
                            NextPick();
                            scroll = 0;
                        }
                    }
                
                return true;
            }

            return false;
        }

        protected override bool CheckNameMatch(string name)
        {
            return false;
        }
    }
    
    
    [Serializable]
    public abstract class UI_Button
    {
        protected readonly string name;

        protected readonly TextMeshProUGUI text;
        protected readonly Image image;
        protected readonly Color colorOn, colorOff;

        private static GameObject prefab;
        private static Transform parent;
        
        protected readonly GameObject gameObject;
        protected bool visible = true;

        protected UI_Button(Color colorOn, string name, Vector2 offset)
        {
            this.name = name;
            
            if(prefab == null)
                prefab = Resources.Load("UI/Assets/Button") as GameObject;
            if (parent == null)
                parent = Inst.parent;
            
            gameObject = Instantiate(prefab, parent, false).SetName(name).SetScale(1, 1, 1);
            
            this.colorOn  = colorOn;
                 colorOff = colorOn.Multi(.75f);
            
            text = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            image = gameObject.GetComponent<Image>();
            
            Vector2 size = image.rectTransform.sizeDelta;
            
            const float gap = 5;
            Vector2 pos = new Vector2( gap + (size.x + gap) * (offset.x - (columns - 1) * .5f), 
                                      -gap + (size.y + gap) * -offset.y);
                        
            gameObject.GetComponent<RectTransform>().anchoredPosition = pos;

            
            image.color = colorOff;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Midline;
            
            
            allButtons.Add(this);
        }
        
        
        protected static readonly List<UI_Button> allButtons = new List<UI_Button>();
        
        public abstract void SetActive(bool active, bool fireAction = true);

        public void Show(bool visible)
        {
            this.visible = visible;

            image.enabled = visible;
            text.enabled  = visible;
        }
        
        public virtual void StayActive(bool stayActive){}
        protected abstract bool PointingAtMe(bool click);
        protected abstract bool CheckNameMatch(string name);
        
        public static bool PointedAt(bool click)
        {
            for (int i = 0; i < allButtons.Count; i++)
                if (allButtons[i].PointingAtMe(click))
                    return true;
            
            return false;
        }

        public static void SetSelectedName(string name)
        {
            for (int i = 0; i < allButtons.Count; i++)
                if (allButtons[i].CheckNameMatch(name))
                    return;
        }
    }
}
