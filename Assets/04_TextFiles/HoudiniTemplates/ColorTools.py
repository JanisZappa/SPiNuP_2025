

def Dist(colorA, colorB):

	return abs(colorA[0] - colorB[0]) + abs(colorA[1] - colorB[1]) + abs(colorA[2] - colorB[2])


def getColorIndex(color):

	bestDist = 100000.0
	pick = 0

	for i in range(len(colors)):
		dist = Dist(color, colors[i])
		if dist < bestDist:
			bestDist = dist
			pick = i

	return pick


def getColor(index):
	
	return colors[index]


def getClosestColor(color):

	return getColor(getColorIndex(color))
