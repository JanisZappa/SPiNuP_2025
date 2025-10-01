using System.Collections.Generic;
using ActorAnimation;
using LevelElements;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Actor : MonoBehaviour 
{
	public Transform pivot;
    public Item      item;

    private   bool       _front;
    private   GameObject _gameObject;
    protected Transform  _transform;

    
    [HideInInspector] public GameObject[] hidden;
    [HideInInspector] public ActorShadow[] shadows;

    protected ActorAnim anim;

    protected bool HasToUpdate()
    {
        anim = ActorAnim.GetAnim(item);
        return item.parent != null || anim != null || GameManager.IsCreator;
    }
   
    
    public virtual void Setup()
    {
        _gameObject = gameObject;
        _transform  = transform;

        _transform.rotation = Rot.Y(180);
        
        _gameObject.SetActive(false);
    }
    

    public virtual void SetItem(Item item)
    {
        this.item = item;
        
        if (item != null)
        {
            Profiler.BeginSample(item.elementType.Name());
            
            if(!_gameObject.activeInHierarchy)
                _gameObject.SetActive(true);

            if (_front != item.side.front)
            {
                _front = item.side.front;
                
                _transform.rotation = Rot.Y(item.side.front? 0: 180);
            }
            
            for (int i = 0; i < shadows.Length; i++)
                shadows[i].SetSide(_front);

            for (int i = 0; i < hidden.Length; i++)
                hidden[i].layer = _front ? Layers.MainA : Layers.MainB;

            Profiler.EndSample();
        }
        else
        {
            if(_gameObject.activeInHierarchy)
                _gameObject.SetActive(false);
        }
    }


    public virtual void SetTransform(bool forcedUpdate)
    {
        if (!HasToUpdate() && !forcedUpdate && false)
        {
            ShadowUpdate(false);
            return;
        }

        _transform.position = item.GetPos(GTime.Now).V3(Level.WallDepth * item.side.Sign);
        
        ShadowUpdate(true);
    }


    protected void ShadowUpdate(bool updateTransform)
    {
        for (int i = 0; i < shadows.Length; i++)
            shadows[i].DepthShadowUpdate(LightingSet.GetDepthOffset(item.depth * item.side.Sign));   
    }

    
    public Vector3 TipOffset { get; protected set; }
    

    public override bool Equals(object obj)
    {
        return obj != null && ((Actor) obj).compareID == compareID;
    }

    
    public override int GetHashCode()
    {
        return compareID;
    }


    private readonly int compareID;
    private static int staticID;

    public Actor()
    {
        compareID = staticID++;
    }
    
    
    protected void SetDepthPos(Vector2 pos)
    {
        _transform.position = pos.V3((Level.WallDepth + item.depth) * item.side.Sign);
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(Actor), true)]
public class ActorEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.color = COLOR.red.tomato;
        if(GUILayout.Button("Setup", GUILayout.Width(100)))
            Editor_PrepareActor((Actor)target);
        
        GUI.color = Color.white;
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }


    private static void Editor_PrepareActor(Actor actor)
    {
        Transform transform = actor.transform;
        
        actor.shadows = transform.GetComponentsInChildren<ActorShadow>();
        for (int i = 0; i < actor.shadows.Length; i++)
            actor.shadows[i].actor = actor;
        
        Debug.Log("Helping with " + actor.shadows.Length + " Shadows");

        MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>(true);

        List<GameObject> hidden = new List<GameObject>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].gameObject.layer = LayerMask.NameToLayer("Default");
            if(renderers[i].gameObject.CompareTag("Hide"))
                hidden.Add(renderers[i].gameObject);
        }

        actor.hidden = hidden.ToArray();
        
        Debug.Log("Helping with " + renderers.Length + " Renderers");
    }
}

#endif
