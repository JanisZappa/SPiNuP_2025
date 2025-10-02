using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class BoolSwitchList : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    private readonly StringBuilder builder = new();
    private int pick;
    private bool show;
    private GameObject panel;

    private void Start()
    {
        panel = tmp.transform.parent.gameObject;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            show = !show;
            panel.SetActive(show);
        }
        
        if (show)
        {
            builder.Length = 0;

            List<BoolSwitch> switches = BoolSwitch.SortedSwitches;
            int count = switches.Count;

            pick = (pick + (Input.GetKeyDown(KeyCode.Alpha6) ? 1 : 0) + (Input.GetKeyDown(KeyCode.Alpha5) ? -1 : 0) + count) % count;

            if (Input.GetKeyDown(KeyCode.Alpha7))
                switches[pick].Toggle();

            for (int i = 0; i < count; i++)
            {
                BoolSwitch s = switches[i];
                string n =
                    (i == pick
                        ? ("<allcaps>" + (s ? "<color=#ffd630>" : "<color=#ef4a6d>") + "</allcaps>")
                        : (s ? "<color=#d6d6d6>" : "<color=#343434>")) +
                    s.id.PadRight(35) + "</color>";

                if (i % 3 == 2)
                    builder.AppendLine(n);
                else
                    builder.Append(n);
            }

            tmp.text = builder.ToString();
        }
    }
}
