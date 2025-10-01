using UnityEngine;


public class OldRig : MonoBehaviour {

	public Animator animator;

	public AnimationClip[] clips;

	private int anim;
	


	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			anim = (anim + 1) % clips.Length;
			animator.SetFloat("Anim", anim / (clips.Length - 1f));
			animator.Play(0);
			Debug.Log(clips[anim].name);
		}
	}
}
