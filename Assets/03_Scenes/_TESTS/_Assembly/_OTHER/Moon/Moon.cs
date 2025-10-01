using System;
using TMPro;
using UnityEngine;


public class Moon : MonoBehaviour
{
	public GameObject fullMoonText;
	public Material moonMat;
	public TextMeshProUGUI dateText;
	private static readonly int SunOnMoon = Shader.PropertyToID("SunOnMoon");

	private static int dayOffset;
	private static DateTime checkTime;

	private Vector3 smoothDir;
	private static readonly int DotAdd = Shader.PropertyToID("dotAdd");


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetMouseButtonDown(0) && Input.mousePosition.x <= Screen.width * .5f)
			dayOffset--;
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetMouseButtonDown(0) && Input.mousePosition.x > Screen.width * .5f)
			dayOffset++;

		checkTime = DateTime.Now + new TimeSpan(dayOffset, 0, 0, 0);

		Vector3 dir = Quaternion.AngleAxis(CycleLerp * 360, Vector3.up) * Vector3.forward;

		smoothDir = smoothDir == Vector3.zero ? dir : Vector3.Slerp(smoothDir, dir, Time.deltaTime * 16);
		
		moonMat.SetVector(SunOnMoon, Quaternion.Inverse(transform.rotation) * (Quaternion.AngleAxis(12, Vector3.forward) * smoothDir));
		
		fullMoonText.SetActive(IsFull);

		float lerp = Vector3.Dot(smoothDir, -Vector3.forward) * .5f + .5f;
		
		dateText.text = checkTime.Month.ToString("D2") + "/" + checkTime.Day.ToString("D2") + "/" + checkTime.Year;
		dateText.color = Color.Lerp(new Color(.3f, .3f, .3f, 1), Color.white, lerp);
		moonMat.SetFloat(DotAdd, lerp * .04f - .02f);
	}

	
	public static float CycleLerp
	{
		get
		{
			DateTime now = checkTime; //DateTime.Now;
			return (float)GetMoonAge(now.Day, now.Month, now.Year);
		}
	}

	
	public static float Age
	{
		get { return CycleLerp * (float)MoonCycleLength; }
	}


	public static bool IsFull
	{
		get { return /*Mathf.FloorToInt(Age) == 15 || */Mathf.CeilToInt(Age) == 15; }
	}
	
	
	private const double MoonCycleLength = 29.53059;

	
	private static int GetJulianDate(int day, int month, int year)
	{
		year = year - (12 - month) / 10;

		month = month + 9;

		if (month >= 12)
			month = month - 12;

		int k1 = (int)(365.25 * (year + 4712));
		int k2 = (int)(30.6001 * month + 0.5);
            
		// 'j' for dates in Julian calendar:
		int julianDate = k1 + k2 + day + 59;

		//Gregorian calendar
		if (julianDate > 2299160)
		{
			int k3 = (int)((year / 100 + 49) * 0.75) - 38;
			julianDate = julianDate - k3; //at 12h UT (Universal Time)
		}

		return julianDate;
	}

	
	private static double GetMoonAge(int day, int month, int year)
	{
		int julianDate = GetJulianDate(day, month, year);

		double ip = (julianDate + 4.867) / 29.53059;
		ip = ip - Math.Floor(ip);

		double age = (ip * MoonCycleLength + MoonCycleLength / 2);

		if (age > MoonCycleLength)
			age -= MoonCycleLength;

		return age / MoonCycleLength;
	}
	
//  SOURCE:
//  https://gist.github.com/adrianstevens/776530e198734b34a9c8a43aaf880041
}
