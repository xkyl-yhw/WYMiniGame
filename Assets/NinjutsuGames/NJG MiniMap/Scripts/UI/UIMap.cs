//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UIMap base class.
/// </summary>

public abstract class UIMap : MonoBehaviour
{
	/// <summary>
	/// Cache transform for speed.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	/// <summary>
	/// Texture used to display the map.
	/// </summary>

	public UITexture uiTexture;

	/// <summary>
	/// Texture material.
	/// </summary>

	//public Material mapMaterial;

	/// <summary>
	/// Map texture color.
	/// </summary>

	public Color mapColor = Color.white;

	/// <summary>
	/// Mask texture for the map.
	/// </summary>

	public Texture maskTexture;

	protected UIRoot mRoot;	

	Transform mTrans;	
	int mLastHeight = 0;
	float mNextUpdate = 0f;
	//Vector3 mLastPos = Vector3.zero;
	//float mLastSize = 0f;

	protected virtual void Awake ()
	{		
		mTrans = transform;

		if (uiTexture == null)
			uiTexture = GetComponentInChildren<UITexture>();

		if (maskTexture == null && uiTexture != null)
			maskTexture = uiTexture.material.GetTexture("_Mask");	

		mRoot = NGUITools.FindInParents<UIRoot>(gameObject);

		if (mRoot == null)
		{
			Debug.LogWarning("Expected to find a map renderer to work with", this);
			return;
		}

		if (uiTexture == null)
		{
			Debug.LogWarning("Expected to find a UITexture to work with", this);
			return;
		}		
	}

	/// <summary>
	/// Clean up. Destroy code created stuff only.
	/// </summary>

	/*void OnDestroy()
	{
		/*if (uiTexture != null)
		{
			if (uiTexture.material.GetInstanceID() < 0)
				NGUITools.Destroy(uiTexture.material);
		}*

		//if (maskTexture.GetInstanceID() < 0)
		//	NGUITools.Destroy(maskTexture);
	}*/

	/// <summary>
	/// Create the child object that will be used for map indicators.
	/// </summary>

	protected virtual void Start()
	{
		if (NJGMap.instance == null) return;

		if (uiTexture != null)
		{
			if (uiTexture.material == null)
			{
				Debug.LogWarning("The UITexture does not have a material assigned", this);
			}
			else
			{
				if (NJGMap.instance.generateMapTexture)
					uiTexture.material.mainTexture = NJGMap.instance.mapTexture;
				//else
				//	uiTexture.material.mainTexture = NJGMap.instance.userMapTexture;

				//if (maskTexture != null) uiTexture.material.SetTexture("_Mask", maskTexture);
			}

			uiTexture.color = mapColor;			
		}

		OnStart();
		Update();
	}

	/// <summary>
	/// Update what's necessary.
	/// </summary>

	protected virtual void Update()
	{
		if (uiTexture != null)
		{
			int height = (mRoot == null || mRoot.automatic) ? Screen.height : mRoot.manualHeight;
			bool heightChanged = (mLastHeight != height);

			if (heightChanged || mNextUpdate < Time.time)
			{
				mLastHeight = height;
				mNextUpdate = Time.time + NJGMap.instance.updateFrequency;

				bool posChanged = false;

				/*if (mLastPos != NJGMap.instance.mapRenderer.transform.position || mLastSize != NJGMap.instance.mapRenderer.cachedCamera.orthographicSize)
				{
					posChanged = true;
					mLastPos = NJGMap.instance.mapRenderer.transform.position;
					mLastSize = NJGMap.instance.mapRenderer.cachedCamera.orthographicSize;
				}*/
				if (OnUpdate(posChanged, heightChanged, height) || heightChanged || posChanged)
				{
					//if (uiTexture != null && uiTexture.material != null)
					//	NJGMap.instance.mapRenderer.Render();
				}
			}
		}
	}

	/// <summary>
	/// Anything you need to do in Start.
	/// </summary>

	protected virtual void OnStart () { }

	/// <summary>
	/// Anything else you might want to update (target indicators and such). Return 'true' if the map should be redrawn.
	/// </summary>

	protected virtual bool OnUpdate (bool posChanged, bool heightChanged, int height) { return false; }	
}