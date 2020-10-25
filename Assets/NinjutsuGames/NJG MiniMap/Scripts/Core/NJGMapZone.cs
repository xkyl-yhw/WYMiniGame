//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Map Zone")]
[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class NJGMapZone : MonoBehaviour 
{
	static public BetterList<NJGMapZone> list = new BetterList<NJGMapZone>();
	static public int id = 0;	

	public Color color { get { return NJGMap.instance.GetZoneColor(level, zone); } }
	public string triggerTag = "Player";
	public string zone;
	public string level;
	public int colliderRadius = 10;
	public int mId = 0;

	SphereCollider mCollider;

	void Awake()
	{
		id++;
		mId = id;
		mCollider = GetComponent<SphereCollider>();

		// make sure our collider is a trigger
		mCollider.isTrigger = true;
		mCollider.radius = colliderRadius;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag(triggerTag))
		{
			NJGMap.instance.zoneColor = color;
			NJGMap.instance.worldName = zone;
		}
	}

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

	void OnDestroy()
	{
		id--;
	}

#if UNITY_EDITOR
	void Update()
	{
		if (mCollider.radius != colliderRadius)
			mCollider.radius = colliderRadius;
	}
#endif
}
