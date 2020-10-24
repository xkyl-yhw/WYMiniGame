//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright ?2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("NJG MiniMap/Map Item")]
public class NJGMapItem : MonoBehaviour 
{
	static public BetterList<NJGMapItem> list = new BetterList<NJGMapItem>();

	public string content;    
	public Color color { get { return NJGMap.instance.GetColor(type); } }
	public UIAtlas.Sprite sprite { get { return NJGMap.instance.GetSprite(type); } }
	public UIAtlas.Sprite arrowSprite { get { return NJGMap.instance.GetArrowSprite(type); } }
	public bool rotate { get { return NJGMap.instance.GetRotate(type); } }
	public bool updatePosition { get { return NJGMap.instance.GetUpdatePosition(type); } }
    public string type;
	public bool animateOnVisible { get { return NJGMap.instance.GetAnimateOnVisible(type); } }
	public bool loopAnimation { get { return NJGMap.instance.GetLoopAnimation(type); } }
	public bool haveArrow { get { return NJGMap.instance.GetHaveArrow(type); } }
	public float fadeOutAfterDelay { get { return NJGMap.instance.GetFadeOutAfter(type); } }
	public int depth { get { return NJGMap.instance.GetDepth(type); } }
	public int arrowDepth { get { return NJGMap.instance.GetArrowDepth(type); } }
	public int arrowOffset { get { return NJGMap.instance.GetArrowOffset(type); } }
	public UIMapArrow arrow;

	/// <summary>
	/// Cache transform for speed.
	/// </summary>

    private Transform mTrans;
    public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    /// <summary>
    /// Add this unit to the list of in-game units.
    /// </summary>
	
    void OnEnable()
    {
        list.Add(this);
    }

    /// <summary>
    /// Remove this unit from the list.
    /// </summary>
	
    void OnDisable()
    {
        list.Remove(this);
    }
}
