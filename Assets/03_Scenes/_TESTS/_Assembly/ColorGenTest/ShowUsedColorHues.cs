using UnityEngine;

namespace Test
{
	public class ShowUsedColorHues : MonoBehaviour
	{
		private static readonly int[] notUsed = { 5, 7, 17, 26, 27, 28, 29 };
		private static bool[] skip;

		[Range(0, 1)] public float min, max;


		private void Start()
		{
			GeneratePaletteTex();
		}


		private void OnValidate()
		{
			float center = (max + min) * .5f;
			min = Mathf.Max(0, Mathf.Min(min, center - .025f));
			max = Mathf.Min(1, Mathf.Max(max, center + .025f));

			if (Application.isPlaying)
				GeneratePaletteTex();
		}


		private void GeneratePaletteTex()
		{
			if (!Palette.Initialized)
				Palette.Load();

			if (skip == null)
			{
				skip = new bool[Palette.Colors.Length];
				for (int i = 0; i < notUsed.Length; i++)
					skip[notUsed[i]] = true;
			}


			const int resolution = 256;

			HLS baseColor = new HLS(1, .5f, 1);
			Color[] compareColors = new Color[resolution * 3];
			for (int i = 0; i < resolution; i++)
			{
				compareColors[i] = baseColor.ShiftHue(i * (1f / resolution));
				compareColors[i + resolution] = new Color(.05f, .05f, .05f, 1);
				compareColors[i + resolution * 2] = baseColor.ShiftHue(Mathf.Floor(i * (1f / resolution) * 16) / 16);
			}

			for (int i = 0; i < Palette.Colors.Length; i++)
				if (!skip[i])
				{
					HLS hls = Palette.Colors[i].ToHLS();

					if (hls.s < min || hls.s > max)
						continue;

					float hueLerp = hls.h;
					compareColors[Mathf.FloorToInt(hueLerp * resolution) + resolution] = Palette.Colors[i];
				}


			Texture2D tex = new Texture2D(resolution, 3)
			{
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};

			tex.SetPixels(compareColors);
			tex.Apply();
			GetComponent<MeshRenderer>().material.mainTexture = tex;
		}
	}
}