//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A game mini map that display icons and scroll UITexture when target moves.
/// </summary>

[AddComponentMenu("NJG MiniMap/UI/Minimap")]
public class UIMiniMap : UIWorldMap
{
	private static UIMiniMap mInst;
	static public UIMiniMap instance { get { if (mInst == null) mInst = GameObject.FindObjectOfType(typeof(UIMiniMap)) as UIMiniMap; return mInst; } }

	public override bool isMap { get { return false; } }

	public GameObject arrowRoot
	{
		get 
		{
			if (mArrowRoot == null)
			{
				mArrowRoot = NGUITools.AddChild(gameObject);
				if (uiTexture != null)
				{
					mArrowRoot.transform.parent = uiTexture.cachedTransform.parent;
					mArrowRoot.transform.localPosition = new Vector3(uiTexture.cachedTransform.localPosition.x, uiTexture.cachedTransform.localPosition.y, -(Mathf.Abs(uiTexture.cachedTransform.localPosition.z) + 12));

				}
				if (usePanelForIcons)
				{
					if (mArrowRoot.GetComponent<UIPanel>() == null) mArrowRoot.AddComponent<UIPanel>();
				}
				else
				{
					if (mArrowRoot.GetComponent<UIPanel>() != null) Destroy(mArrowRoot.GetComponent<UIPanel>());
				}
				mArrowRoot.name = "_MapArrows";
			}
			return mArrowRoot; 
		}
	}

	/// <summary>
	/// The target that Minimap is going to follow.
	/// </summary>
	
	public Transform target;

	/// <summary>
	/// The target can be found using this tag.
	/// </summary>
	
	public string targetTag = "Player";

	/// <summary>
	/// Current level of zoom of the Minimap.
	/// </summary>

	public int zoom;

	/// <summary>
	/// Minimun level of zoom.
	/// </summary>

	public float minZoom = 0;

	/// <summary>
	/// Maximun level of zoom.
	/// </summary>

	public float maxZoom = 10f;

	/// <summary>
	/// Limit bounds
	/// </summary>

	public bool limitBounds;	

	/// <summary>
	/// Limit bounds
	/// </summary>
	
	public bool rotateWithPlayer = false;

	/// <summary>
	/// Optional north icon. Will be automatically placed if its assigned.
	/// </summary>

	public GameObject northIcon;

	/// <summary>
	/// North icon offset.
	/// </summary>

	public int northIconOffset = 10;

	/// <summary>
	/// Key to toggle the world map.
	/// </summary>

	public KeyCode mapKey = KeyCode.M;

	/// <summary>
	/// If icons get farther this radius they will dissapear.
	/// </summary>

	public float mapBorderRadius = 50;

	Quaternion targetRotation;
	Vector3 mapPosition = Vector3.zero;
	Vector3 rotationPivot = new Vector3(0.5f, 0.5f);
	Vector2 scrollPosition = Vector2.zero;
	//int mZoom;
	float targetAngle;
	bool isZooming;
	bool worldMapVisible;
	GameObject northRoot;
	Transform mTarget;
	GameObject mArrowRoot;

	protected List<NJGMapItem> mPingList = new List<NJGMapItem>();
	protected List<NJGMapItem> mPingUnused = new List<NJGMapItem>();

	NJGMapItem pingMarker;
	int mArrowCount = 0;

	protected override void OnStart()
	{
		base.OnStart();

		// Calculate the border radius for icon culling.
		mapBorderRadius = (uiTexture.cachedTransform.localScale.x / 2f) / 4f;

		BoxCollider col = uiTexture.GetComponent<BoxCollider>();
		if (col == null)
			col = NGUITools.AddWidgetCollider(uiTexture.gameObject);

		UIForwardEvents fe = uiTexture.GetComponent<UIForwardEvents>();
		if (fe == null)
			fe = uiTexture.gameObject.AddComponent<UIForwardEvents>();

		fe.onClick = true;
		fe.target = gameObject;

		// Create north root and place icon north properly
		northRoot = NGUITools.AddChild(iconRoot);
		northRoot.name = "North";
		northRoot.transform.localPosition = Vector3.zero;

		northIcon.transform.parent = northRoot.transform;
		northIcon.transform.localPosition = new Vector3(0, uiTexture.cachedTransform.localScale.y / 2 - northIconOffset, 0);
		northIcon.transform.localRotation = Quaternion.identity;

		// For some reason when this get repositioned and reparented it doesnt update its position when its moved.
		// So disable and enable makes it work.
		NGUITools.SetActive(northIcon, false);
		NGUITools.SetActive(northIcon, true);

		// Create a gameObject for the ping marker
		/*GameObject go = NGUITools.AddChild(iconRoot);
		go.name = "_Ping";

		// Assign Ping type to this marker, 'Ping' type most be created on the editor
		pingMarker = go.AddComponent<NJGMapItem>();
		pingMarker.type = "Ping";*/
	}

	#region Ping (This feature is not fullly working yet)

	void OnClick()
	{
		/*NJGMap.MapItemType mi = NJGMap.instance.GetItem("Ping");
		if (mi != null)
		{
			UIMapIcon icon = GetEntry(pingMarker);
			//Debug.Log("CLick " + UICamera.lastHit.textureCoord + " - " + UICamera.lastHit.textureCoord2 + " / " + UICamera.lastHit.point);
			icon.cachedTransform.localPosition = new Vector3(UICamera.lastHit.point.x, UICamera.lastHit.point.y, 0);
		}
		else
		{
			Debug.LogWarning("There is no 'Ping' icon type defined");
		}*/
	}

	#endregion

	/// <summary>
	/// Update what's necessary.
	/// </summary>

	override protected bool OnUpdate (bool posChanged, bool heightChanged, int height)
	{
		UpdateIcons();
		CleanIcons();
	
		return base.OnUpdate(posChanged, heightChanged, height);
	}

	/// <summary>
	/// Update Minimap scroll position.
	/// </summary>
	
	private void UpdateScrollPosition()
	{
		Bounds bounds = NJGMap.instance.bounds;

		// If there is no target defined lets use the mainCamera
		if (target == null && Camera.main != null)
		{
			if (mTarget != Camera.main.transform) mTarget = Camera.main.transform;
		}
		else if (target != null && mTarget != target) mTarget = target;

		if (mTarget == null) return;

		Vector3 vector = mTarget.position - bounds.center;

		scrollPosition = new Vector2(vector.x * (0.5f / bounds.extents.x), vector.z * (0.5f / bounds.extents.z));

		// Limit minimap position
		if (limitBounds)
		{
			scrollPosition.x = Mathf.Max(-((1f - (1f / zoom)) / 2f), scrollPosition.x);
			scrollPosition.x = Mathf.Min((1f - (1f / zoom)) / 2f, scrollPosition.x);
			scrollPosition.y = Mathf.Max(-((1f - (1f / zoom)) / 2f), scrollPosition.y);
			scrollPosition.y = Mathf.Min((1f - (1f / zoom)) / 2f, scrollPosition.y);
		}

		mapPosition.x = ((1f - (1f / zoom)) / 2f) + scrollPosition.x;
		mapPosition.y = ((1f - (1f / zoom)) / 2f) + scrollPosition.y;
		mapPosition.z = 0;

		// Get target angle.
		targetAngle = ((Vector3.Dot(mTarget.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f) ? 1f : -1f) * Vector3.Angle(mTarget.forward, Vector3.forward);
		targetRotation = Quaternion.Euler(0, 0, targetAngle);

		// Relative zoom.
		Vector3 nZoom = new Vector3(1f / zoom, 1f / zoom, 1f / zoom);			

		// Move and scale matrix
		Matrix4x4 m = Matrix4x4.TRS(mapPosition, Quaternion.identity, nZoom);

		if (uiTexture != null)
		{
			if (uiTexture.material != null)
			{
				if (rotateWithPlayer)
				{
					// Rotation matrix
					Matrix4x4 t = Matrix4x4.TRS(-rotationPivot, Quaternion.identity, Vector3.one);
					Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, targetRotation, Vector3.one);
					Matrix4x4 tInv = Matrix4x4.TRS(rotationPivot, Quaternion.identity, Vector3.one);

					uiTexture.material.SetMatrix("_Matrix", m * tInv * r * t);

					if(iconRoot != null) iconRoot.transform.localEulerAngles = new Vector3(0, 0, -targetAngle);
				}
				else
				{
					uiTexture.material.SetMatrix("_Matrix", m);
					if (iconRoot != null) if (iconRoot.transform.localEulerAngles != uiTexture.cachedTransform.localEulerAngles) iconRoot.transform.localEulerAngles = uiTexture.cachedTransform.localEulerAngles;
				}
			}
		}			
	}

	/// <summary>
	/// Update what's necessary.
	/// </summary>

	protected override void Update()
	{
		if (target == null && !string.IsNullOrEmpty(targetTag))
		{
			if (GameObject.FindGameObjectWithTag(targetTag) != null) target = GameObject.FindGameObjectWithTag(targetTag).transform;
		}

		UpdateScrollPosition();

		if (UIWorldMap.map != null)
		{
			if (Input.GetKeyDown(mapKey))
				ToggleWorldMap();
		}

		base.Update();
	}	

	/// <summary>
	/// Update the coordinates and colors of map indicators.
	/// </summary>

	protected override void UpdateIcons()
	{
		// Mark all entries as invalid
		for (int i = mList.Count; i > 0; ) mList[--i].isValid = false;

		// Update all entries, marking them as valid in the process
		for (int i = 0; i < NJGMapItem.list.size; ++i)
		{
			NJGMapItem item = NJGMapItem.list.buffer[i];
			Vector2 pos = WorldToMap(item.cachedTransform.position);
			UpdateIcon(item, pos.x, pos.y);
		}
	}

	/// <summary>
	/// Update the icon icon for the specified unit, assuming it's visible.
	/// </summary>

	protected override void UpdateIcon(NJGMapItem item, float x, float y)
	{		
		// If the unit is not visible, don't consider it
		Vector2 v = new Vector2(uiTexture.cachedTransform.localScale.x / 2f, uiTexture.cachedTransform.localScale.y / 2f);
		bool isVisible = (((x - mapBorderRadius) >= -v.x) && ((x + mapBorderRadius) <= v.x)) && (((y - mapBorderRadius) >= -v.y) && ((y + mapBorderRadius) <= v.y));

		if (!isVisible && item.haveArrow)
		{
			if (item.arrow == null) item.arrow = GetArrow(item);

			if(item.arrow != null)
			{
				if(!item.arrow.gameObject.activeInHierarchy) NGUITools.SetActive(item.arrow.gameObject, true);
				item.arrow.UpdatePosition();
			}
		}
		else if (isVisible && item.haveArrow)
		{
			if (item.arrow != null) if (item.arrow.gameObject.activeInHierarchy) NGUITools.SetActive(item.arrow.gameObject, false);
		}

		if (!isVisible) return;

		UIMapIcon icon = GetEntry(item);
		icon.isMapIcon = false;

		if (icon != null && !icon.isValid)
		{
			icon.isValid = true;
			Transform t = icon.cachedTransform;
			if(item.updatePosition) t.localPosition = new Vector3(x, y, 0f);

			if (item.rotate)
			{
				float angle = ((Vector3.Dot(item.cachedTransform.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f) ? 1f : -1f) * Vector3.Angle(item.cachedTransform.forward, Vector3.forward);
				t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, angle);
			}
			else if (!item.rotate && rotateWithPlayer)
			{
				Vector3 eu = iconRoot.transform.localEulerAngles;
				eu.z = -iconRoot.transform.localEulerAngles.z;
				if (t.localEulerAngles != eu) t.localEulerAngles = eu;
			}
			else
				if (t.localEulerAngles != Vector3.zero) t.localEulerAngles = Vector3.zero;
		}
	}

	#region Arrows

	BetterList<UIMapArrow> mListArrow = new BetterList<UIMapArrow>();
	BetterList<UIMapArrow> mUnusedArrow = new BetterList<UIMapArrow>();

	int mArrowSize = 0;
	Vector3 mASize = Vector3.one;
	Vector3 arrowScale
	{
		get
		{
			if (mArrowSize != NJGMap.instance.arrowSize)
			{
				mArrowSize = NJGMap.instance.arrowSize;
				mASize.x = mASize.y = NJGMap.instance.arrowSize;
			}
			return mASize;
		}
	}

	/// <summary>
	/// Get the map icon entry associated with the specified unit.
	/// </summary>

	public UIMapArrow GetArrow(NJGMapItem item)
	{
		// Try to find an existing entry
		for (int i = 0, imax = mListArrow.size; i < imax; ++i) if (mListArrow[i].item == item)
			{
				UIMapArrow ic = mListArrow[i];
				ic.item = item;
				ic.sprite.spriteName = item.arrowSprite.name;
				ic.sprite.depth = item.depth;
				ic.sprite.color = item.color;
				ic.sprite.cachedTransform.localScale = arrowScale;
				ic.sprite.cachedTransform.localPosition = new Vector3(0, UIMiniMap.instance.uiTexture.cachedTransform.localScale.y / 2 - item.arrowOffset, 0);
				return ic;
			}

		// See if an unused entry can be reused
		if (mUnusedArrow.size > 0)
		{
			UIMapArrow ent = mUnusedArrow[mUnusedArrow.size - 1];
			ent.item = item;
			ent.sprite.spriteName = item.arrowSprite.name;
			ent.sprite.depth = item.depth;
			ent.sprite.color = item.color;
			ent.sprite.cachedTransform.localScale = arrowScale;
			ent.sprite.cachedTransform.localPosition = new Vector3(0, UIMiniMap.instance.uiTexture.cachedTransform.localScale.y / 2 - item.arrowOffset, 0);
			mUnusedArrow.RemoveAt(mUnusedArrow.size - 1);
			NGUITools.SetActive(ent.gameObject, true);
			mListArrow.Add(ent);
			return ent;
		}

		// Create this new icon
		GameObject go = NGUITools.AddChild(UIMiniMap.instance.uiTexture.cachedTransform.parent.gameObject);
		go.name = "Arrow" + mArrowCount;
		go.transform.parent = UIMiniMap.instance.arrowRoot.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;

		UISprite sprite = NGUITools.AddWidget<UISlicedSprite>(go);
		//sprite.type = UISprite.Type.Sliced;
		sprite.depth = item.depth;
		sprite.atlas = NJGMap.instance.atlas;
		sprite.spriteName = item.arrowSprite.name;
		sprite.color = item.color;
		sprite.cachedTransform.localScale = arrowScale;
		sprite.cachedTransform.localPosition = new Vector3(0, UIMiniMap.instance.uiTexture.cachedTransform.localScale.y / 2 - item.arrowOffset, 0);

		UIMapArrow mi = go.AddComponent<UIMapArrow>();
		mi.item = item;
		mi.sprite = sprite;

		if (mi == null)
		{
			Debug.LogError("Expected to find a UIMapArrow on the prefab to work with");
			Destroy(go);
		}
		else
		{
			mArrowCount++;
			mi.item = item;
			mListArrow.Add(mi);
		}
		return mi;
	}

	/// <summary>
	/// Delete the specified entry, adding it to the unused list.
	/// </summary>

	void DeleteArrow(UIMapArrow ent)
	{
		mListArrow.Remove(ent);
		mUnusedArrow.Add(ent);
		NGUITools.SetActive(ent.gameObject, false);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Transform world coords to map.
	/// </summary>

	public Vector2 WorldToMap(Vector3 worldPos)
	{
		Bounds bounds = NJGMap.instance.bounds;
		Vector3 v = worldPos - bounds.center;

		float x = (uiTexture.cachedTransform.localScale.x / 2f) / bounds.extents.x;
		float z = (uiTexture.cachedTransform.localScale.y / 2f) / bounds.extents.z;
		x *= zoom;
		z *= zoom;

		Vector3 v2 = WorldScrollPosition();

		return new Vector2((v.x - v2.x) * x, (v.z - v2.z) * z);
	}

	public Vector3 WorldScrollPosition()
	{
		return new Vector3(scrollPosition.x * (NJGMap.instance.bounds.extents.x / 0.5f), 0f, scrollPosition.y * (NJGMap.instance.bounds.extents.z / 0.5f));
	}

	/// <summary>
	/// Zoom in the minimap
	/// </summary>

	public void ZoomIn()
	{
		zoom = Mathf.Clamp(zoom + 1, (int)minZoom, (int)maxZoom);
	}

	/// <summary>
	/// Zoom out the minimap
	/// </summary>

	public void ZoomOut()
	{
		zoom = Mathf.Clamp(zoom - 1, (int)minZoom, (int)maxZoom);
	}

	/// <summary>
	/// Toggle Wold map if the instance its found
	/// </summary>

	public void ToggleWorldMap()
	{
		if (UIWorldMap.map != null)
		{
			if (worldMapVisible)
				HideWorldMap();
			else
				ShowWorldMap();
		}
	}

	/// <summary>
	/// Show Wold map if the instance its found
	/// </summary>

	public void ShowWorldMap()
	{
		if (UIWorldMap.map != null)
		{
			if (!worldMapVisible)
			{
				Animation anim = UIWorldMap.map.GetComponent<Animation>();
				if (anim != null)
				{
					if (anim.clip != null)
						ActiveAnimation.Play(anim, anim.clip.name, AnimationOrTween.Direction.Forward, AnimationOrTween.EnableCondition.EnableThenPlay, AnimationOrTween.DisableCondition.DoNotDisable);
					else NGUITools.SetActive(UIWorldMap.map.gameObject, true);
				}
				else NGUITools.SetActive(UIWorldMap.map.gameObject, true);
				worldMapVisible = true;
			}
		}
	}

	/// <summary>
	/// Hide Wold map if the instance its found
	/// </summary>

	public void HideWorldMap()
	{
		if (UIWorldMap.map != null)
		{
			if (worldMapVisible)
			{
				Animation anim = UIWorldMap.map.GetComponent<Animation>();
				if (anim != null)
				{
					if (anim.clip != null)
						ActiveAnimation.Play(anim, anim.clip.name, AnimationOrTween.Direction.Reverse, AnimationOrTween.EnableCondition.DoNothing, AnimationOrTween.DisableCondition.DisableAfterReverse);
					else NGUITools.SetActive(UIWorldMap.map.gameObject, false);
				}
				else NGUITools.SetActive(UIWorldMap.map.gameObject, false);
				worldMapVisible = false;
			}
		}
	}

	#endregion
}