using UnityEngine;


public class CircleAllign : MonoBehaviour
{
	[Range(.75f, 5f)] public float radius;

	[Space(10)] public float[] lineLengths;
	public int lineNumber;
	[Range(0, 1)] public float offsetOnCircle;

	private const int numberOfPoints = 100;

	public bool fit;
	public bool draw;
	public bool move;
	[Space(10)] public float speed;

	public float animRadius;

	
	private void OnEnable()
	{
		if (draw)
			DRAW.Enabled = DRAW.EditorDraw = true;

		animRadius = radius;
	}

	
	private void Update()
	{
		animRadius = Mathf.Lerp(animRadius, radius, Time.deltaTime * 8);

		if (move)
			offsetOnCircle += (Time.deltaTime / (Mathf.PI * 2 * radius)) * speed;

		float useRadius = animRadius;

		if (draw)
			DRAW.Line(numberOfPoints, i => V3.up.RotZ(360f / (numberOfPoints - 1) * i, useRadius)).SetColor(COLOR.red.tomato.A(.1f));

		Vector3 pointA = V3.up.RotZ(360f * offsetOnCircle, useRadius);
		float degree = 0;

		for (int i = 0; i < lineNumber; i++)
		{
			if (draw)
				DRAW.Circle(pointA, .07f).SetColor(COLOR.blue.cornflower);

			float segmentLength = lineLengths[i % lineLengths.Length];
			float arcLength = 2 * Mathf.Asin(segmentLength / 2 / useRadius);

			degree += arcLength * Mathf.Rad2Deg;

			Vector3 pointB = V3.up.RotZ(360f * offsetOnCircle + degree, useRadius);
			if (draw)
				DRAW.Circle(pointB, .03f).SetColor(COLOR.green.spring);
			if (draw)
				DRAW.Vector(pointA, pointB - pointA).SetColor(COLOR.orange.coral);

			pointA = pointB;
		}
	}


	public Vector3[] GetPointsOnCircle(int number, float segmentLength, float extraRadius, float startoffset = 0, float shift = 0)
	{
		float off = offsetOnCircle + shift;
		float r = animRadius + extraRadius;

		float degree = GetAngleForCirclePoint(startoffset, r);

		Vector3 pointA = V3.up.RotZ(360f * off + degree, r);
		Vector3[] returnPoints = new Vector3[number];
		returnPoints[0] = pointA;


		for (int i = 0; i < number; i++)
		{
			returnPoints[i] = pointA;

			degree += GetAngleForCirclePoint(segmentLength, r);

			pointA = Rot.Z(360f * off + degree) * V3.up * r;
		}
		
		return returnPoints;
	}


	public static float GetAngleForCirclePoint(float offsetAlongCircle, float radius)
	{
		return 2 * Mathf.Asin(offsetAlongCircle / 2 / radius) * Mathf.Rad2Deg;
	}
}