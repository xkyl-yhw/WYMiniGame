//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Game map can have icons on it -- this class takes care of animating them when needed.
/// </summary>

[ExecuteInEditMode]
public class UIMapIcon : MonoBehaviour
{
	public static BetterList<UIMapIcon> list = new BetterList<UIMapIcon>();

	public static UIMapIcon FindByMarker(NJGMapItem m)
	{
		for (int i = 0, imax = list.size; i < imax; i++)		
			if (list[i].marker == m) return list[i];		

		return null;
	}

	[SerializeField]
	public NJGMapItem marker;
	[HideInInspector] public UISprite sprite;
	[HideInInspector]
	public bool isValid = false;
	[HideInInspector]
	public bool isMapIcon = true;

	/// <summary>
	/// Cache transform for speed.
	/// </summary>
	
	private Transform mTrans;
	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	protected bool isLooping;
	protected bool isScaling;
	protected Vector3 onHoverScale = new Vector3(1.3f, 1.3f, 1.3f);

	TweenScale ts;

	/// <summary>
	/// Triggered when the icon is visible on the map.
	/// </summary>

	void Start() { CheckAnimations(); }

	void CheckAnimations()
	{
		if (marker != null)
		{
			if (marker.fadeOutAfterDelay > 0)
				OnFadeOut();
			else
				sprite.alpha = 1;

			if (marker.animateOnVisible && !isMapIcon)
				StartCoroutine(OnVisible());

			if (marker.loopAnimation)
				OnLoop();
			else
				isLooping = false;
		}
	}

	/// <summary>
	/// Add this unit to the list of in-game units.
	/// </summary>

	void OnEnable()
	{
		cachedTransform.localScale = Vector3.one;
		CheckAnimations();
		list.Add(this);
	}

	/// <summary>
	/// Remove this unit from the list.
	/// </summary>

	void OnDisable()
	{
		list.Remove(this);
	}

	/// <summary>
	/// Add a collider in order to make hover state work.
	/// </summary>

	void Awake()
	{
		if(gameObject.GetComponent<Collider>() == null) 
			NGUITools.AddWidgetCollider(gameObject);
	}

	/// <summary>
	/// Triggered when the icon is visible on the map.
	/// </summary>

	protected virtual IEnumerator OnVisible()
	{
		if (isScaling)
			yield return null;

		isScaling = true;
		ts = TweenScale.Begin(gameObject, 0.5f, Vector3.one);
		ts.steeperCurves = true;
		ts.from = Vector3.one * 2f;
		ts.method = UITweener.Method.EaseOut;

		yield return new WaitForSeconds(ts.duration);

		cachedTransform.localScale = Vector3.one;
		isScaling = false;
	}

	protected virtual void OnLoop()
	{
		isLooping = true;
		ts = TweenScale.Begin(gameObject, 0.5f, Vector3.one);
		ts.steeperCurves = true;
		ts.from = Vector3.one * 1.5f;
		ts.method = UITweener.Method.EaseOut;
		ts.style = UITweener.Style.PingPong;
	}

	protected virtual void OnFadeOut()
	{
		/*TweenAlpha ts = TweenAlpha.Begin(gameObject, 1, 0);
		ts.delay = marker.fadeOutAfterDelay;
		ts.from = 1;
		ts.method = UITweener.Method.Linear;*/
	}

	/// <summary>
	/// Display a tooltip with the appropiate content.
	/// </summary>

	protected virtual void OnTooltip(bool show)
	{
		if (!string.IsNullOrEmpty(marker.content))
		{
			if (show)
				UICustomTooltip.Show(marker.content);
			else
				UICustomTooltip.Hide();
		}
	}

	/// <summary>
	/// Triggered when mouse is over this icon.
	/// </summary>
	
	protected virtual void OnHover(bool isOver)
	{
		TweenScale ts = null;
		if (isOver)
		{
			if (!isLooping)
			{
				ts = TweenScale.Begin(gameObject, 0.1f, onHoverScale);
				ts.from = Vector3.one;
				ts.method = UITweener.Method.EaseOut;
			}
			
		}
		else
		{
			if (!isLooping)
			{
				ts = TweenScale.Begin(gameObject, 0.3f, Vector3.one);
				ts.method = UITweener.Method.EaseOut;
			}			
		}
	}
}