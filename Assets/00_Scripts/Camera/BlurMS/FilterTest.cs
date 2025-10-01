﻿using UnityEngine;


[ExecuteInEditMode]
public class FilterTest : MonoBehaviour
{
	public enum DownSampleMode { Off, Half, Quarter }

	public Shader _shader;

	public DownSampleMode _downSampleMode = DownSampleMode.Quarter;

	[Range(0, 8)]
	public int _iteration = 4;

	public Material _material;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (_material == null)
		{
			_material = new Material(_shader);
			_material.hideFlags = HideFlags.HideAndDontSave;
		}

		RenderTexture rt1, rt2;

		if (_downSampleMode == DownSampleMode.Half)
		{
			rt1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2);
			rt2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2);
			Graphics.Blit(source, rt1);
		}
		else if (_downSampleMode == DownSampleMode.Quarter)
		{
			rt1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
			rt2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
			Graphics.Blit(source, rt1, _material, 0);
		}
		else
		{
			rt1 = RenderTexture.GetTemporary(source.width, source.height);
			rt2 = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, rt1);
		}

		for (var i = 0; i < _iteration; i++)
		{
			Graphics.Blit(rt1, rt2, _material, 1);
			Graphics.Blit(rt2, rt1, _material, 2);
		}

		Graphics.Blit(rt1, destination);

		RenderTexture.ReleaseTemporary(rt1);
		RenderTexture.ReleaseTemporary(rt2);
	}
}