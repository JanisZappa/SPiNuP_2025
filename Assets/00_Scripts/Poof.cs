using System.Collections.Generic;
using LevelElements;
using UnityEngine;


public class Poof : MonoBehaviour
{
	public Mesh[] poofs;
	
    private static Stack<PoofTransform> pool;
    private static List<PoofTransform> active;

    private class PoofTransform
    {
	    private const float speed = 2.4f, duration = 1f / speed;
	    private float start, end;

	    private readonly Transform trans;
	    private readonly MeshRenderer mR;
	    private readonly MeshFilter mF;

	    private Spinner spinner;
	    private Item item;
	    private Quaternion rot;
	    private float spin;

	    public static Mesh[] poofMeshes;

	    
	    public PoofTransform(Transform trans)
	    {
		    this.trans = trans;
		    mF = trans.GetComponent<MeshFilter>();
		    mR = trans.GetComponent<MeshRenderer>();
		    mR.enabled = false;
	    }
	    

	    public void Setup(Spinner spinner, Item item, Vector3 dir, float spin, float start)
	    {
		    this.spinner = spinner;
		    this.item    = item;
		    this.spin    = spin.SignAdd(1) * 15;

		    this.start = start;
		           end = start + duration;
		           
		    rot = Quaternion.LookRotation(Vector3.forward, dir);

		    mF.mesh = GetMesh();
	    }


	    public PoofTransform Disable()
	    {
		    mR.enabled = false;
		    return this;
	    }


	    public bool Update(float time)
	    {
		    if (time - GTime.RewindTime > end)
		    {
			    mR.enabled = false;
			    return false;
		    }
			    
		    float lerp = (time - start) / duration;

		    mR.enabled = lerp > 0 && lerp < 1;

			lerp = Mathf.Clamp01(lerp);

			
		    float angle = (1 - Mathf.Pow(1 - lerp, 1.5f)) * 20;

		    Quaternion spinRot = Quaternion.AngleAxis(lerp * spin, Vector3.forward) * rot;
		    
		    trans.rotation = spinRot * Quaternion.AngleAxis(angle, Vector3.up);
		    trans.position = item.GetLagPos(time).V3(Level.GetPlaneDist(item.side)) + spinRot * Vector3.up * (.7f +  (1 - Mathf.Pow(1 - lerp * .75f, 4)) * 1.4f);

		    const float first = .75f, multi = 1 / first, multi2 = 1 / (1 - first);
		    float scaleLerp;
		    float yScale;
		    if (lerp < first)
		    {
			    float l = lerp * multi;
			    scaleLerp = 1 - Mathf.Pow(1 - l, 8);
			    yScale = .4f * Mathf.Pow(1 - l, 4) + 1f;
		    }
		    else
		    {
			    float l = (lerp - first) * multi2;
			    scaleLerp = Mathf.Pow(1 - l, 3.2f);
			    yScale = 1f - .1f * Mathf.Pow(1 - l, 2);
		    }
		    

		    trans.localScale = new Vector3(scaleLerp, yScale * scaleLerp, scaleLerp) * .8f;

		    return true;
	    }


	    public bool ClearedAfter(Spinner spinner, float time)
	    {
		    return this.spinner == spinner && time <= start;
	    }


	    private Mesh GetMesh()
	    {
		    switch (item.elementType)
		    {
			    default:                     return poofMeshes[0]; 
			    case elementType.WarpStickA: return poofMeshes[1];
			    case elementType.WarpStickB: return poofMeshes[2];
			    case elementType.WarpStickC: return poofMeshes[3];
			    case elementType.WarpStickD: return poofMeshes[4];
		    }
	    }
    }
    

    private void Awake()
    {
	    pool = new Stack<PoofTransform>();
	    
	    Transform dummy = transform.GetChild(0);
	    pool.Push(new PoofTransform(dummy));

	    for (int i = 0; i < 19; i++)
		    pool.Push(new PoofTransform(dummy.gameObject.CopyAndParent(true).transform));
	    
	    active = new List<PoofTransform>(20);

	    PoofTransform.poofMeshes = poofs;
    }

    
    private void OnEnable()
    {
	    GameManager.OnGameStart += GameManagerOnOnGameStart;
    }
    
    
    private void OnDisable()
    {
	    GameManager.OnGameStart -= GameManagerOnOnGameStart;
    }
    

    private static void GameManagerOnOnGameStart()
    {
	    while (active.Count > 0)
		    pool.Push(active.GetRemoveLast().Disable());
    }


    private void LateUpdate ()
    {
	    int count = active.Count;
	    for (int i = 0; i < count; i++)
		    if (!active[i].Update(GTime.Now))
		    {
			    pool.Push(active.GetRemoveAt(i));
			    
			    i--;
			    count--;
		    }
    }


    public static void Show(Spinner spinner, Item item, Vector3 dir, float spin, float start)
    {
	    PoofTransform poof = pool.Pop();
	    if (poof != null)
	    {
		    poof.Setup(spinner, item, dir, spin, start);
		    active.Add(poof);
	    }  
    }


    public static void ClearAfter(Spinner spinner, float time)
    {
	    int count = active.Count;

	    for (int i = 0; i < count; i++)
		    if (active[i].ClearedAfter(spinner, time))
		    {
			    pool.Push(active.GetRemoveAt(i).Disable());
			    count--;
			    i--;
		    }
    }
}
