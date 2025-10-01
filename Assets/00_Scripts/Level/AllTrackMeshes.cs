using System.Collections.Generic;
using LevelElements;
using UnityEngine;

namespace Test
{
	public class AllTrackMeshes : MonoBehaviour
	{
		public GameObject trackMeshPrefab;

		private List<TrackMesh> meshes;


		private void Update()
		{
			if (meshes == null && GameManager.Running)
			{
				meshes = new List<TrackMesh>();

				for (int i = 0; i < Track.active.Count; i++)
				{
					GameObject gO = Instantiate(trackMeshPrefab, transform);
					TrackMesh tM = gO.GetComponent<TrackMesh>();
					tM.trackToSkin = Track.active[i].ID;

					gO.name = Track.active[i].ID.ToString();

					meshes.Add(tM);
				}
			}
		}
	}
}