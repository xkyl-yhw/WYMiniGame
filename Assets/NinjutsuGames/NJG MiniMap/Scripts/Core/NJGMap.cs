//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NJGTextureFormat
{
	ARGB32 = TextureFormat.ARGB32,
	RGB24 = TextureFormat.RGB24
}

public enum NJGCameraClearFlags
{
	Skybox = CameraClearFlags.Skybox,
	Depth = CameraClearFlags.Depth,
	Color = CameraClearFlags.Color
}

[ExecuteInEditMode]
[AddComponentMenu("NJG MiniMap/Map")]
public class NJGMap : MonoBehaviour 
{
	[System.Serializable]
	public class MapItemType
	{
		public string type = "New Marker";
		public string sprite;
		public Color color = Color.white;
		public bool animateOnVisible = true;
		public bool loopAnimation = false;
		public float fadeOutAfterDelay = 0;
		public bool rotate = true;
		public bool updatePosition = true;
		public bool haveArrow = false;
		public string arrowSprite;
		public bool folded = true;
		public int depth = 0;
		public bool deleteRequest = false;
		public int arrowOffset = 20;
		public int arrowDepth = 5;

		public void OnSelectSprite(string spriteName)
		{
			sprite = spriteName;			
		}

		public void OnSelectArrowSprite(string spriteName)
		{
			arrowSprite = spriteName;
		}
	}

	[System.Serializable]
	public class MapLevel
	{
		public string level = "Level";
		public List<MapZone> zones = new List<MapZone>();
		public bool folded = true;
		public bool itemsFolded = true;		
		public bool deleteRequest = false;
	}

	[System.Serializable]
	public class MapZone
	{
		public string type = "New Zone";
		//public string sprite;
		public Color color = Color.white;		
		public float fadeOutAfterDelay = 3;
		public bool folded = true;
		public int depth = 0;
		public bool deleteRequest = false;

		/*public void OnSelectSprite(string spriteName)
		{
			sprite = spriteName;
		}*/
	}

	public enum Orientation
	{
		XZDefault,
		XYSideScroller
	}

	public delegate void OnWorldNameChanged(string name);

	/// <summary>
	/// Get instance.
	/// </summary>

	static NJGMap mInst;
	static public NJGMap instance { get { if (mInst == null) mInst = GameObject.FindObjectOfType(typeof(NJGMap)) as NJGMap; return mInst; } }

	static GameObject mZRoot;
	static public GameObject zonesRoot { get { if (mZRoot == null) mZRoot = GameObject.Find("_MapZones"); return mZRoot; } set { mZRoot = value; } }

	public OnWorldNameChanged onWorldNameChanged;

	/// <summary>
	/// The name of the world that is going to be displayed on World Map and Mini Map titles.
	/// </summary>

	public string worldName { 
		get { return mWorldName; } 
		set 
		{ 
			mWorldName = value;

			// Only trigger this event if the name is different than the last one
			if (onWorldNameChanged != null && mLastWorldName != mWorldName)
			{
				mLastWorldName = mWorldName;
				onWorldNameChanged(value);
			}
		} 
	}

	/// <summary>
	/// Current zone color.
	/// </summary>

	public Color zoneColor;

	/// <summary>
	/// List of map item types.
	/// </summary>

	public List<MapItemType> mapItems = new List<MapItemType>(new MapItemType[]{new MapItemType()});

	/// <summary>
	/// List of zones.
	/// </summary>

	public List<MapLevel> levels = new List<MapLevel>();

	/// <summary>
	/// Map orientation.
	/// </summary>

	public Orientation orientation = Orientation.XZDefault;

	/// <summary>
	/// Which layers is the map going to render.
	/// </summary>

	public LayerMask renderLayers = 1;

	/// <summary>
	/// Which layers are going to be used for bounds calculation.
	/// </summary>
	
	public LayerMask boundLayers = 1;

	/// <summary>
	/// Global size of the map icons.
	/// </summary>

	public int iconSize = 16;

	/// <summary>
	/// Global size of the map arrows.
	/// </summary>

	public int arrowSize = 16;

	/// <summary>
	/// How often the map will be updated.
	/// </summary>

	public float updateFrequency = 0.15f;

	/// <summary>
	/// World bounds.
	/// </summary>
	
	public Bounds bounds;

	/// <summary>
	/// Atlas we are going to use for icon sprites.
	/// </summary>

	public UIAtlas atlas;

	/// <summary>
	/// Default sprite.
	/// </summary>

	public UIAtlas.Sprite defaultSprite;

	/// <summary>
	/// Internally used by the inspector to save fold state of map item types.
	/// </summary>

	public bool typesFolded;

	/// <summary>
	/// Internally used by the inspector to save fold state of zones.
	/// </summary>

	public bool zonesFolded;

	/// <summary>
	/// Texture of the map.
	/// </summary>
	
	public Texture2D mapTexture;

	/// <summary>
	/// User defined texture of the map.
	/// </summary>

	public Texture2D userMapTexture;

	/// <summary>
	/// If true the map texture will be generated.
	/// </summary>

	public bool generateMapTexture;
	public bool useTextureGenerated;

	public FilterMode mapFilterMode = FilterMode.Bilinear;
	public TextureWrapMode mapWrapMode = TextureWrapMode.Clamp;
	public NJGTextureFormat textureFormat = NJGTextureFormat.ARGB32;
	public NJGCameraClearFlags cameraClearFlags = NJGCameraClearFlags.Depth;
	public bool compressTexture = true;
	public bool generateMipmaps = true;
	public int renderOffset = 10;
	public int layer = 0;

	public Vector3 mapOrigin;
	public Vector3 mapEulers;
	public float ortoSize;

	/// <summary>
	/// Map renderer.
	/// </summary>

	public NJGMapRenderer mapRenderer;

	/// <summary>
	/// Map size.
	/// </summary>

	public Vector2 mapSize { 
		set { mSize = value; } 
		get {
			if (Application.isPlaying)
			{
				mSize.x = Screen.width;
				mSize.y = Screen.height;
			}

			return mSize;
		}
	}

	Vector2 mSize = Vector2.zero;
	UICamera uiCam;
	Camera mCam;
	Bounds mBounds;
	string mWorldName = "My Epic World";
	string mLastWorldName;

	/// <summary>
	/// Get a string list of map item types.
	/// </summary>
	
	static public string[] mapItemTypes
	{
		get
		{
			List<string> types = new List<string>();

			if (instance != null)
			{
				if (instance.mapItems != null)
				{
					for (int i = 0, imax = instance.mapItems.Count; i < imax; i++)
						types.Add(instance.mapItems[i].type);
				}
			}
			
			return types.Count == 0 ? new string[]{"No types defined"} : types.ToArray();
		}
	}

	/// <summary>
	/// Setup map renderer.
	/// </summary>

	void Awake()
	{
		/*if (instance != null)
		{
			Debug.LogWarning("Can't have more than 1 instance of NJGMap on the scene.");
			NGUITools.Destroy(gameObject);
			return; 
		}*/

		if (Application.isPlaying)
		{
			if (mapTexture != null) NGUITools.Destroy(mapTexture);
			GenerateMap();
		} 
	}

	/// <summary>
	/// Generate map texture.
	/// </summary>

	public void GenerateMap()
	{
		if (GameObject.Find("NJGMapRenderer")) NGUITools.DestroyImmediate(GameObject.Find("NJGMapRenderer"));
		GameObject go = NGUITools.AddChild(gameObject);
		go.name = "NJGMapRenderer";
		go.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		go.transform.localPosition = new Vector3(bounds.center.x, (bounds.center.y + bounds.extents.y) + 1f, bounds.center.z);	

		mapRenderer = go.AddComponent<NJGMapRenderer>();
	}

	/// <summary>
	/// Update bounds on start.
	/// </summary>
	
	void Start()
	{		
		if (onWorldNameChanged != null) onWorldNameChanged(worldName);
		UpdateBounds();
	}

	/// <summary>
	/// Clean up.
	/// </summary>

	void OnDestroy()
	{
		if (mapTexture != null) NGUITools.Destroy(mapTexture);
		if (mapRenderer != null) 
			if (mapRenderer.gameObject != null) 
				NGUITools.Destroy(mapRenderer.gameObject);
	}

	public void SetTexture(Texture2D tex)
	{
		if (UIMiniMap.instance != null) UIMiniMap.instance.uiTexture.material.mainTexture = tex;
		if (UIWorldMap.map != null) UIWorldMap.map.uiTexture.material.mainTexture = tex;
	}

	/// <summary>
	/// Checks if GameObject is within render layers range.
	/// </summary>

	public static bool IsInRenderLayers(GameObject obj, LayerMask mask)
	{
		return (mask.value & (1 << obj.layer)) > 0;
	}

	/// <summary>
	/// Create bounding box and scale it to contain all scene game objects, if terrain is found it is used
	/// </summary>
	
	public void UpdateBounds()
	{
		// First lets see if there is any active terrain
		if (Terrain.activeTerrain != null)
		{
			GameObject mTerrain = Terrain.activeTerrain.gameObject;
			MeshRenderer mMeshRenderer = mTerrain.GetComponent<MeshRenderer>();
			if (mMeshRenderer != null)
				mBounds = mMeshRenderer.bounds;			
			else
			{
				TerrainCollider mTerrainCollider = mTerrain.GetComponent<TerrainCollider>();
				if (mTerrainCollider != null)
					mBounds = mTerrainCollider.bounds;				
				else
				{
					Debug.LogError("Could not get measure bounds of terrain.", this);
					return;
				}
			}
		}

		// If there is no active terrain lets measure the whole scene getting all renderers bounds
		Renderer[] aListRenderers = UnityEngine.Object.FindObjectsOfType(typeof(Renderer)) as Renderer[];
		if (aListRenderers != null)
		{
			foreach (Renderer aRenderer in aListRenderers)
			{				
				// Dont consider this game object
				if (aRenderer.gameObject.layer == gameObject.layer)
					continue;

				// Only use objects from the layer mask
				if (!IsInRenderLayers(aRenderer.gameObject, boundLayers))
					continue;
				{
					if (string.IsNullOrEmpty(mBounds.ToString()))					
						mBounds = aRenderer.bounds;					
					else
					{
						Bounds aLocalBounds = mBounds;
						aLocalBounds.Encapsulate(aRenderer.bounds);
						mBounds = aLocalBounds;
					}
				}
			}
		}

		if(string.IsNullOrEmpty(mBounds.ToString()))
		{
			// Could not measure bounds
			Debug.LogError("Could not find terrain nor any other bounds in scene", this);
			return;
		}	

		// Set bounds
		bounds = mBounds;
	}

	#region Getters

	static public string[] GetZones(string level)
	{
		List<string> list = new List<string>();

		if (instance != null)
		{
			if (instance.levels != null)
			{
				for (int i = 0, imax = instance.levels.Count; i < imax; i++)
				{
					if (instance.levels[i].level == level)
					{
						for (int e = 0, emax = instance.levels[i].zones.Count; e < emax; e++)
						{
							list.Add(instance.levels[i].zones[e].type);
						}
					}
				}
			}
		}

		return list.Count == 0 ? new string[] { "No Zones defined" } : list.ToArray();
	}

	/// <summary>
	/// Get zone by scene.
	/// </summary>

	static public string[] GetLevels()
	{
		List<string> list = new List<string>();

		if (instance != null)
		{
			if (instance.levels != null)
			{
				for (int i = 0, imax = instance.levels.Count; i < imax; i++)				
					list.Add(instance.levels[i].level);				
			}
		}

		return list.Count == 0 ? new string[] { "No Levels defined" } : list.ToArray();
	}

	/// <summary>
	/// Get color from zone.
	/// </summary>

	public Color GetZoneColor(string level, string zone)
	{
		Color c = Color.white;
		for (int i = 0, imax = instance.levels.Count; i < imax; i++)
		{
			if (instance.levels[i].level == level)
			{
				for (int e = 0, emax = instance.levels[i].zones.Count; e < emax; e++)
				{
					if (instance.levels[i].zones[e].type == zone) return instance.levels[i].zones[e].color;
				}
			}
		}

		return c;
	}

	/// <summary>
	/// Get color from type.
	/// </summary>

	public Color GetColor(string type) { return Get(type) == null ? Color.white : Get(type).color; }

	/// <summary>
	/// Get sprite from type.
	/// </summary>

	public UIAtlas.Sprite GetSprite(string type)
	{
		if (atlas == null)
		{
			Debug.LogWarning("You need to assign an atlas", this);
			return null;
		}
		return Get(type) == null ? defaultSprite : atlas.GetSprite(Get(type).sprite);
	}

	/// <summary>
	/// Get arrow sprite from type.
	/// </summary>

	public UIAtlas.Sprite GetArrowSprite(string type)
	{
		if (atlas == null)
		{
			Debug.LogWarning("You need to assign an atlas", this);
			return null;
		}
		return Get(type) == null ? defaultSprite : atlas.GetSprite(Get(type).arrowSprite);
	}

	/// <summary>
	/// Get animate from type.
	/// </summary>

	public bool GetAnimateOnVisible(string type) { return Get(type) == null ? false : Get(type).animateOnVisible; }

	/// <summary>
	/// Get animate from type.
	/// </summary>

	public bool GetLoopAnimation(string type) { return Get(type) == null ? false : Get(type).loopAnimation; }

	/// <summary>
	/// Get have arrow from type.
	/// </summary>

	public bool GetHaveArrow(string type) { return Get(type) == null ? false : Get(type).haveArrow; }

	/// <summary>
	/// Get animate from type.
	/// </summary>

	public float GetFadeOutAfter(string type) { return Get(type) == null ? 0 : Get(type).fadeOutAfterDelay; }

	/// <summary>
	/// Get rotate flag.
	/// </summary>

	public bool GetRotate(string type) { return Get(type) == null ? false : Get(type).rotate; }

	/// <summary>
	/// Get update position flag.
	/// </summary>

	public bool GetUpdatePosition(string type) { return Get(type) == null ? false : Get(type).updatePosition; }

	/// <summary>
	/// Get depth.
	/// </summary>

	public int GetDepth(string type) { return Get(type) == null ? 0 : Get(type).depth; }

	/// <summary>
	/// Get arrow depth.
	/// </summary>

	public int GetArrowDepth(string type) { return Get(type) == null ? 0 : Get(type).arrowDepth; }

	/// <summary>
	/// Get arrow offset.
	/// </summary>

	public int GetArrowOffset(string type) { return Get(type) == null ? 0 : Get(type).arrowOffset; }

	/// <summary>
	/// Get map item type.
	/// </summary>

	public MapItemType Get(string type)
	{
		for (int i = 0, imax = mapItems.Count; i < imax; i++)
		{
			if (mapItems[i].type == type)
				return mapItems[i];
		}

		return null;
	}

	#endregion

#if UNITY_EDITOR

	/// <summary>
	/// Update layer mask for UICamera and Camera inside
	/// </summary>
	
	void Update()
	{
		if (!Application.isPlaying)
		{
			if (uiCam == null) uiCam = GetComponentInChildren<UICamera>();
			if (uiCam != null) if (uiCam.eventReceiverMask != 1 << gameObject.layer) uiCam.eventReceiverMask = 1 << gameObject.layer;

			if (mCam == null) mCam = GetComponentInChildren<Camera>();
			if (mCam != null) if (mCam.cullingMask != 1 << gameObject.layer) mCam.cullingMask = 1 << gameObject.layer;

			if (mapTexture != null) NGUITools.DestroyImmediate(mapTexture);
		}
	}
#endif
}
