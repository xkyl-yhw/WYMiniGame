//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 Ninjutsu Games LTD.
//----------------------------------------------

using System.Collections;
using UnityEngine;

/// <summary>
/// Very basic game map -- renderer component. It's able to draw a map into a 2D texture.
/// </summary>

[ExecuteInEditMode]
public class NJGMapRenderer : MonoBehaviour
{
	/// <summary>
	/// Get instance.
	/// </summary>

	//static NJGMapRenderer mInst;
	static public NJGMapRenderer instance;

	/// <summary>
	/// Cached camera for speed.
	/// </summary>

	public Camera cachedCamera { get { if (mCam == null) mCam = GetComponent<Camera>(); return mCam; } }

	/// <summary>
	/// Cached transform for speed.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	Transform mTrans;
	Camera mCam;
	int mWidth = 0;
	int mHeight = 0;
	bool hasRendered;
	float renderedTime;

	void Awake()
	{
		if (NJGMap.instance == null)
		{
			Debug.LogWarning("Can't render map photo. NJGMiniMap instance not found.");
			NGUITools.Destroy(gameObject);
			return;
		}			
	}

	void Start()
	{
		if (NJGMapRenderer.instance != null)
		{
			Debug.Log("Cant have more than one NJGMapRenderer on the scene, destroying this...");
			NGUITools.DestroyImmediate(gameObject);
			return;
		}
		instance = this;

		if (gameObject.GetComponent<Camera>() == null)
			gameObject.AddComponent<Camera>();	

		if (NJGMap.instance.boundLayers.value == 0)
		{
			Debug.LogError("Can't render map photo. You have not choosen any layer for bounds calculation. Go to the NJGMiniMap inspector.", NJGMap.instance);
			NGUITools.DestroyImmediate(gameObject);
			return;
		}

		if (NJGMap.instance.renderLayers.value == 0)
		{
			Debug.LogWarning("Can't render map photo. You have not choosen any layer for rendering. Go to the NJGMiniMap inspector.", NJGMap.instance);
			NGUITools.DestroyImmediate(gameObject);
			return;
		}

		mWidth = (int)NJGMap.instance.mapSize.x;
		mHeight = (int)NJGMap.instance.mapSize.y;

		NJGMap.instance.UpdateBounds();
		Bounds bounds = NJGMap.instance.bounds;

		cachedCamera.depth = -100;

		cachedCamera.cullingMask = NJGMap.instance.renderLayers;
		cachedCamera.clearFlags = (CameraClearFlags)NJGMap.instance.cameraClearFlags;
		cachedCamera.orthographic = true;
		cachedCamera.nearClipPlane = -10;
		cachedCamera.farClipPlane = 300;

		//if (NJGMap.instance.orientation == NJGMap.Orientation.XZDefault)
		//{
		//float z;
		cachedCamera.farClipPlane = 2.0f + bounds.size.y * 5.0f;

		/*Camera camera = Object.FindObjectOfType(typeof(Camera)) as Camera;
		float num = bounds.extents.x / bounds.extents.z;
		if (num <= camera.aspect)
			z = bounds.extents.z;
		else
			z = bounds.extents.x / camera.aspect;*/

		cachedCamera.orthographicSize = bounds.extents.z + NJGMap.instance.renderOffset;
		cachedCamera.aspect = bounds.size.x / bounds.size.z;

		NJGMap.instance.mapOrigin = transform.position;
		NJGMap.instance.mapEulers = transform.localRotation.eulerAngles;
		NJGMap.instance.ortoSize = GetComponent<Camera>().orthographicSize;
	//}

		//if (!NJGMap.instance.generateMapTexture)
		//{			
		StartCoroutine(DelayedDestroy(gameObject, 2));
			Render();			
		//}
	}

	IEnumerator DelayedDestroy(UnityEngine.Object obj, float delay)
	{
		yield return new WaitForSeconds(delay);

		NGUITools.DestroyImmediate(obj);
	}

	/// <summary>
	/// Render method for ingame. Waits for postrender.
	/// </summary>
	
	IEnumerator OnPostRender()
	{
		if (Application.isPlaying && NJGMap.instance.generateMapTexture)
		{
			NJGMap.instance.mapTexture.ReadPixels(new Rect(0f, 0f, NJGMap.instance.mapSize.x, NJGMap.instance.mapSize.y), 0, 0, NJGMap.instance.generateMipmaps);			
		}
		else
		{
			if (NJGMap.instance.userMapTexture != null && !Application.isPlaying) 
				NJGMap.instance.userMapTexture.ReadPixels(new Rect(0f, 0f, NJGMap.instance.mapSize.x, NJGMap.instance.mapSize.y), 0, 0, NJGMap.instance.generateMipmaps);
		}

		yield return new WaitForEndOfFrame();

		cachedCamera.enabled = false;

		if (!hasRendered)
		{
			hasRendered = true;
			renderedTime = Time.time + 1;
			if (NJGMap.instance.generateMapTexture)
			{
				Debug.Log("NJGMap.instance.compressTexture " + NJGMap.instance.compressTexture);
				if (NJGMap.instance.compressTexture) NJGMap.instance.mapTexture.Compress(true);
				NJGMap.instance.mapTexture.Apply(NJGMap.instance.generateMipmaps, true);
			}			
		}

		NJGMap.instance.SetTexture(NJGMap.instance.generateMapTexture ? NJGMap.instance.mapTexture : NJGMap.instance.userMapTexture);

		if (!NJGMap.instance.generateMapTexture && !Application.isPlaying)
			if (NJGMap.instance.userMapTexture != null) NJGMap.instance.userMapTexture.Apply(NJGMap.instance.generateMipmaps, false);

		NGUITools.Destroy(gameObject);
	}

	/// <summary>
	/// Redraw the map's texture.
	/// </summary>
	
	public void Render()
	{
		cachedCamera.enabled = true;
		if (mWidth != NJGMap.instance.mapSize.x || mHeight != NJGMap.instance.mapSize.y)
		{
			mWidth = (int)NJGMap.instance.mapSize.x;
			mHeight = (int)NJGMap.instance.mapSize.y;
		}

		if (NJGMap.instance.generateMapTexture)
		{
			if (NJGMap.instance.mapTexture != null)
			{
				NGUITools.Destroy(NJGMap.instance.mapTexture);
				NJGMap.instance.mapTexture = null;
			}			

			if (NJGMap.instance.userMapTexture != null)
			{
				NGUITools.DestroyImmediate(NJGMap.instance.userMapTexture);
				NJGMap.instance.userMapTexture = null;
			}

			NJGMap.instance.mapTexture = new Texture2D(mWidth, mHeight, (TextureFormat)NJGMap.instance.textureFormat, NJGMap.instance.generateMipmaps);
			NJGMap.instance.mapTexture.name = "_NJGMapTexture";
			NJGMap.instance.mapTexture.hideFlags = HideFlags.DontSave;
			NJGMap.instance.mapTexture.filterMode = NJGMap.instance.mapFilterMode;
			NJGMap.instance.mapTexture.wrapMode = NJGMap.instance.mapWrapMode;
		}
		else if(!Application.isPlaying)
		{
			if (NJGMap.instance.mapTexture != null)
			{
				NGUITools.Destroy(NJGMap.instance.mapTexture);
				NJGMap.instance.mapTexture = null;
			}

			/*if (NJGMap.instance.userMapTexture != null)
			{
				NGUITools.DestroyImmediate(NJGMap.instance.userMapTexture);
				NJGMap.instance.userMapTexture = null;
			}*/

			NJGMap.instance.userMapTexture = new Texture2D(mWidth, mHeight, (TextureFormat)NJGMap.instance.textureFormat, NJGMap.instance.generateMipmaps);
			NJGMap.instance.userMapTexture.name = "_NJGTempTexture";
			NJGMap.instance.userMapTexture.hideFlags = HideFlags.DontSave;
			NJGMap.instance.userMapTexture.filterMode = NJGMap.instance.mapFilterMode;
			NJGMap.instance.userMapTexture.wrapMode = NJGMap.instance.mapWrapMode;
		}

		cachedCamera.Render();		
	}

#if UNITY_EDITOR
	void Update()
	{
		if (Time.time > renderedTime && !Application.isPlaying)
		{
			NGUITools.Destroy(gameObject);
		}
	}
#endif
}