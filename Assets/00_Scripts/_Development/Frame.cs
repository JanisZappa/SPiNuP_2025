using UnityEngine;


public class Frame : MonoBehaviour {

    public RectTransform[] horizontalLines;
    public RectTransform[] verticalLines;
    
    
    private void OnEnable()
    {
        ScreenControll.onOrientationChange += Update_3Split;
        Update_3Split();
    }

    
    private void OnDisable()
    {
        ScreenControll.onOrientationChange -= Update_3Split;
    }

    
    private void Update_3Split()
    {
        
        float width  = ScreenControll.Width;
        float height = ScreenControll.Height;
        
        horizontalLines[0].anchoredPosition = new Vector2(0,height / 3f);
        horizontalLines[1].anchoredPosition = new Vector2(0,height / 3f * 2);
        
        verticalLines[0].anchoredPosition = new Vector2(width / 3f,0);
        verticalLines[1].anchoredPosition = new Vector2(width / 3f * 2,0);
    }
}
