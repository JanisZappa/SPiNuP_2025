using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ThinkingList : ScriptableObject
{
	public List<string> todo;
	[Space(5)]
	public List<string> ideas;
	[Space(5)]
	public List<string> musings;
	[Space(15)]
	public List<string> warnings;
	[Space(5)]
	public List<string> bugs;
	[Space(5)]
	public List<string> build;
	
	
	public enum ThoughtProcess { TODO, Ideas, Musings, Warnings, Bugs, Build }

	[HideInInspector] 
	public ThoughtProcess current;
}
