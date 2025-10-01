using UnityEngine;


public class BotControll : MonoBehaviour {
    
   /* static GameObject spinnerCharacter;
    static SP_Main spMain;
    static SP_Animation spAnim;

    static SwingData swingData;

    static float fireTime;

    static int foundStick = 0;
    static float checkTime = 0;

    static int goalStick;
    static bool reachedGoal;
    static float lastFire;
    static float skill = 1;

   public void Reset()
    {
        skill = Random.Range(.3f, 1f);

        if(Debugger.showDebugLog)
            Debug.Log("BotSkill: " + skill);

        goalStick = (int)Random.Range(2, Mathf.Lerp(10,100,skill));

        if ( Debugger.showDebugLog )
            Debug.Log("BotGoal: " + goalStick);

        reachedGoal = false;
    }
    
    public void CreateBot()
    {
        spinnerCharacter = spinner;
        
        spMain = spinner.GetComponent<SP_Main>();
        spAnim = spinner.GetComponent<SP_Animation>();
    }

    public static void Connect(SwingData swingData)
    {
        BotControll.swingData = swingData;
        checkTime = 0;
        foundStick = 0;

       if (!reachedGoal && BotControll.swingData.stick == goalStick )
            reachedGoal = true;
    }

    
    public void UpdateBot()
    {
         if(foundStick == 0 && swingData != null)
        {
             if (!reachedGoal)
            {
                int check = 0;

                while (check < 3 && foundStick == 0 )
                {
                    check++;
                    checkTime += .01f;

                    float angle = swingData.Get_CleanAngle(swingData.startTime + checkTime);
                    Vector3 spinnerPos = SwingData.Get_CleanPosition(swingData.stick,swingData.charHeight, swingData.Get_CleanAngle(checkTime), checkTime);

                    foundStick = SP_Predict.JumpPrediction(CharacterControll.GetHeight(0), 1, spinnerPos, swingData.GetForceV(swingData.startTime + checkTime), swingData.stick, swingData.startTime + checkTime);
                    
                    if (foundStick.BiggerStickNrThan(swingData.stick))
                    {
                        fireTime = swingData.startTime + checkTime + Random.Range(Mathf.Lerp(-.04f, 0, skill), Mathf.Lerp(.04f, 0, skill));
                        swingData = null;
                        check = 3;
                    }
                    else
                        foundStick = 0;
                }
            }
            else
            {
                Debug.Log("Reached Goal");
                foundStick = 10000000;
                fireTime = swingData.startTime + Random.Range(0, 2f);
                swingData = null;
            }
        }

        lastFire += GameTime.delta;

        if (foundStick != 0 && GameTime.Now >= fireTime || lastFire >= 6)
        {
            spMain.Button(GameTime.Now);
            foundStick = 0;
            lastFire = 0;
        }
         
    }
     */
}
