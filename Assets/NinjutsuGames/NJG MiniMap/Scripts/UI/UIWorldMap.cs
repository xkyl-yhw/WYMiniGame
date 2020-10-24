//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game map expands the UIMap class to add target indicators for units.
/// </summary>

[AddComponentMenu("NJG MiniMap/UI/World Map")]
public class UIWorldMap : UIMap
{
	private static UIWorldMap mInst;

	/// <summary>
	/// Instance of this class.
	/// </summary>

	static public UIWorldMap map { 
		get 
		{
			if (mInst == null)
			{
				Object[] ints = GameObject.FindSceneObjectsOfType(typeof(UIWorldMap));
				for (int i = 0, imax = ints.Length; i < imax; i++)
				{
					UIWorldMap ix = ints[i] as UIWorldMap;
					if (ix.isMap) mInst = ix;
				}
			}
			return mInst; 
		}
		set
		{
			mInst = value;
		}
	}

	public virtual bool isMap { get { return true; } }

	public GameObject iconRoot
	{
		get
		{
			if (mIconRoot == null)
			{
				mIconRoot = NGUITools.AddChild(gameObject);
				if (uiTexture != null)
				{
					mIconRoot.transform.parent = uiTexture.cachedTransform.parent;
					mIconRoot.transform.localPosition = new Vector3(uiTexture.cachedTransform.localPosition.x, uiTexture.cachedTransform.localPosition.y, -(Mathf.Abs(uiTexture.cachedTransform.localPosition.z) + 12));
					mIconRoot.transform.localEulerAngles = uiTexture.cachedTransform.localEulerAngles;
				}
				if (usePanelForIcons)
				{
					if (mIconRoot.GetComponent<UIPanel>() == null) mIconRoot.AddComponent<UIPanel>();
				}
				else
				{
					if (mIconRoot.GetComponent<UIPanel>() != null) Destroy(mIconRoot.GetComponent<UIPanel>());
				}
				mIconRoot.name = "_MapIcons";
			}
			return mIconRoot;
		}
	}

	public bool usePanelForIcons;

	int mCount = 0;

	protected List<UIMapIcon> mList = new List<UIMapIcon>();
	List<UIMapIcon> mUnused = new List<UIMapIcon>();
	GameObject mIconRoot;

	/// <summary>
	/// Create and posionate the icon root.
	/// </summary>

	protected override void OnStart()
	{
		base.OnStart();

		if (isMap)
		{
			map = this;
			NGUITools.SetActive(gameObject, false);
		}
	}

	int mIconSize = 0;
	Vector3 mSize = Vector3.one;
	Vector3 iconScale
	{
		get
		{
			if (mIconSize != NJGMap.instance.iconSize)
			{
				mIconSize = NJGMap.instance.iconSize;
				mSize.x = mSize.y = NJGMap.instance.iconSize;
			}
			return mSize;
		}
	}
	
	/// <summary>
	/// Get the map icon entry associated with the specified unit.
	/// </summary>

	protected UIMapIcon GetEntry(NJGMapItem marker)
	{
		// Try to find an existing entry
		for (int i = 0, imax = mList.Count; i < imax; ++i) if (mList[i].marker == marker)
			{
				UIMapIcon ic = mList[i];
				ic.marker = marker;
				ic.sprite.spriteName = marker.sprite.name;
				ic.sprite.depth = marker.depth;
				ic.sprite.color = marker.color;
				ic.sprite.cachedTransform.localScale = iconScale;
				return ic;
			}

		// See if an unused entry can be reused
		if (mUnused.Count > 0)
		{
			UIMapIcon ent = mUnused[mUnused.Count - 1];
			ent.marker = marker;
			ent.sprite.spriteName = marker.sprite.name;
			ent.sprite.depth = marker.depth;
			ent.sprite.color = marker.color;
			ent.sprite.cachedTransform.localScale = iconScale;
			mUnused.RemoveAt(mUnused.Count - 1);
			NGUITools.SetActive(ent.gameObject, true);
			mList.Add(ent);
			return ent;
		}

		// Create this new icon
		GameObject go = NGUITools.AddChild(iconRoot);
		go.name = "Icon" + mCount;
		UISprite sprite = NGUITools.AddWidget<UISprite>(go);
		//sprite.type = UISprite.Type.Sliced;
		sprite.depth = marker.depth;
		sprite.atlas = NJGMap.instance.atlas;
		sprite.spriteName = marker.sprite.name;
		sprite.color = marker.color;
		sprite.cachedTransform.localScale = iconScale;

		UIMapIcon mi = go.AddComponent<UIMapIcon>();
		mi.marker = marker;
		mi.sprite = sprite;

		if (mi == null)
		{
			Debug.LogError("Expected to find a Game Map Icon on the prefab to work with", this);
			Destroy(go);
		}
		else
		{
			mCount++;
			mi.marker = marker;
			mList.Add(mi);
		}
		return mi;
	}

	/// <summary>
	/// Delete the specified entry, adding it to the unused list.
	/// </summary>

	void Delete(UIMapIcon ent)
	{
		mList.Remove(ent);
		mUnused.Add(ent);
		NGUITools.SetActive(ent.gameObject, false);
	}

	/// <summary>
	/// Update the icon icon for the specified unit, assuming it's visible.
	/// </summary>

	protected virtual void UpdateIcon(NJGMapItem item, float x, float y)
	{
		// If the unit is not visible, don't consider it
		if (x < -0.5f || x > 0.5f || y < -0.5f || y > 0.5f) return;

		UIMapIcon icon = GetEntry(item);

		if (icon != null && !icon.isValid)
		{
			Vector3 scale = uiTexture.cachedTransform.localScale;
			icon.isValid = true;

			Transform t = icon.cachedTransform;
			t.localPosition = new Vector3(Mathf.Round(x * scale.x), Mathf.Round(y * scale.y), 0f);

			if (item.rotate)
			{
				float angle = ((Vector3.Dot(item.cachedTransform.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f) ? 1f : -1f) * Vector3.Angle(item.cachedTransform.forward, Vector3.forward);
				t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, angle);
			}
			else
				if (t.localEulerAngles != Vector3.zero) t.localEulerAngles = Vector3.zero;	
		}
	}

	/// <summary>
	/// Update the coordinates and colors of map indicators.
	/// </summary>

	protected virtual void UpdateIcons()
	{
		// Correct this values using bounds instead when the system is using a predefined texture
		if (NJGMap.instance.ortoSize == 0) NJGMap.instance.ortoSize = NJGMap.instance.bounds.extents.x;
		if (NJGMap.instance.mapOrigin == Vector3.zero) NJGMap.instance.mapOrigin = new Vector3(NJGMap.instance.bounds.extents.x, NJGMap.instance.bounds.size.y + 1f, NJGMap.instance.bounds.extents.z);
		if (NJGMap.instance.mapEulers == Vector3.zero) NJGMap.instance.mapEulers = new Vector3(90, 0, 0);

		Quaternion invRot = Quaternion.Euler(0f, -NJGMap.instance.mapEulers.y, 0f);
		float camRange = NJGMap.instance.ortoSize * 2f;

		// Mark all entries as invalid
		for (int i = mList.Count; i > 0; ) mList[--i].isValid = false;

		// Update all entries, marking them as valid in the process
		for (int i = 0; i < NJGMapItem.list.size; ++i)
		{
			NJGMapItem item = NJGMapItem.list.buffer[i];

			// Update the icon for this unit
			Vector3 off = invRot * (item.cachedTransform.position - NJGMap.instance.mapOrigin);
			Vector2 pos = Vector2.zero;
			pos.x = (off.x / camRange);
			pos.y = (off.z / camRange);

			UpdateIcon(item, pos.x, pos.y);
		}		
	}

	/// <summary>
	/// Update what's necessary.
	/// </summary>

	override protected bool OnUpdate (bool posChanged, bool heightChanged, int height)
	{	
		UpdateIcons();
		CleanIcons();
		return false;
	}

	protected void CleanIcons()
	{
		// Remove invalid entries
		for (int i = mList.Count; i > 0; )
		{
			UIMapIcon icon = mList[--i];

			if (!icon.isValid)
				Delete(icon);			
		}
	}
}