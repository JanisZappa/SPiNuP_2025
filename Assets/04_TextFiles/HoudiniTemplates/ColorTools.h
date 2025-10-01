

float Dist(vector colorA, colorB)
{
	return abs(colorA.x - colorB.x) + abs(colorA.y - colorB.y) + abs(colorA.z - colorB.z);
}


int GetColorIndex(vector color)
{
	float bestDist = 1000000;
	int pick = 0;

	int count = count();

	for(int i = 0; i < count; i++)
	{
		float dist = Dist(color, color(i));

		if(dist < bestDist)
		{
			bestDist = dist;
			pick = i;
		}
	}

	return pick;
}


int GetSimpleColorIndex(vector color)
{
	int r = (int)rint(color.x * 63);
	int g = (int)rint(color.y * 63);
	int b = (int)rint(color.z * 63);

	return colorID(r + g * 64 + b * 64 * 64);
}


vector ClosestGameColor(vector color)
{
	return color(GetColorIndex(color));
}