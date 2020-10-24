//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NJGEditorTools : MonoBehaviour
{
	const string contentFolder = "NinjutsuGames/NJG MiniMap";

	static public Material GetMaterial(UIMap m)
	{
		bool isMiniMap = m is UIMiniMap;

		GetPath();		

		//Material mat = (Material)AssetDatabase.LoadAssetAtPath(matFilename, typeof(Material));
		Material mat = (Material)AssetDatabase.LoadAssetAtPath(isMiniMap ? "Assets/" + contentFolder + "/Materials/MiniMap.mat" : "Assets/" + contentFolder + "/Materials/WorldMap.mat", typeof(Material)) as Material;
		if (mat == null)
		{
			mat = new Material(Shader.Find("NinjutsuGames/MapMask"));

			string[] arr = EditorApplication.currentScene.Split('/');
			string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);

			string matFilename = "Assets/"+contentFolder + "/_Generated Content/" + scene + "_" + (isMiniMap ? "MiniMap" : "WorldMap") + ".mat";

			AssetDatabase.CreateAsset(mat, matFilename);

			Debug.Log((isMiniMap ? "MiniMap" : "WorldMap") + " material generated at: " + matFilename);			

			if (m.uiTexture != null) m.uiTexture.material = mat;
			AssetDatabase.Refresh();

			EditorGUIUtility.PingObject(mat);
		}

		if (m.maskTexture != null)
		{
			if (m.uiTexture != null) if (m.uiTexture.material != null) m.uiTexture.material.SetTexture("_Mask", m.maskTexture);
		}
		else
		{
			m.maskTexture = AssetDatabase.LoadAssetAtPath("Assets/" + contentFolder + "/Mask Textures/" + (isMiniMap ? "CircleMask" : "ErodedMask") + ".png", typeof(Texture)) as Texture;
			if (m.maskTexture != null) if (m.uiTexture != null) if (m.uiTexture.material != null) m.uiTexture.material.SetTexture("_Mask", m.maskTexture);
		}

		if (!NJGMap.instance.generateMapTexture)
		{
			if (m.uiTexture != null)
				if(NJGMap.instance.userMapTexture != null) m.uiTexture.material.SetTexture("_Main", NJGMap.instance.userMapTexture);
		}
		else
		{
			if (m.uiTexture != null)
				if (NJGMap.instance.mapTexture != null) m.uiTexture.material.SetTexture("_Main", NJGMap.instance.mapTexture);
		}

		return mat;
	}

	static public UITexture CreateUIMapTexture(UIMap m)
	{
		bool isMiniMap = m is UIMiniMap;

		//m.mapMaterial = AssetDatabase.LoadAssetAtPath("Assets/NinjutsuGames/NG GameMap/Materials/WorldMap.mat", typeof(Material)) as Material;

		//m.mapMaterial = GenerateMaterial(m);

		m.uiTexture = NGUITools.AddWidget<UITexture>(m.gameObject);
		m.uiTexture.depth = 0;
		m.uiTexture.name = isMiniMap ? "_MiniMapTexture" : "_WorldMapTexture";
		m.uiTexture.material = GetMaterial(m);

		m.uiTexture.cachedTransform.localPosition = new Vector3(0, 0, 0);
		m.uiTexture.cachedTransform.localScale = isMiniMap ? new Vector3(200, 200, 1) : new Vector3(700, 400, 1);		

		return m.uiTexture;
	}

	static public void DrawEditMap()
	{
		EditorGUILayout.Separator();
		if (NJGMap.instance == null)
		{
			GUI.backgroundColor = Color.red;
			EditorGUILayout.HelpBox("No Instance of Map was found.\nPlease create a new one.", MessageType.Error);
			GUI.backgroundColor = Color.white;
		}

		GUI.backgroundColor = NJGMap.instance != null ? Color.cyan : Color.green;
		if (GUILayout.Button(new GUIContent(NJGMap.instance != null ? "Edit NJG MiniMap" : "Create NJG MiniMap", "Click to edit the NJG MiniMap")))
		{
			if (NJGMap.instance == null)
				NJGMenu.AddNJGMiniMap();
			else
				Selection.activeGameObject = NJGMap.instance.gameObject;
		}
		GUI.backgroundColor = Color.white;
		NGUIEditorTools.DrawSeparator();
	}

	static public void CreateMap(bool isMiniMap)
	{
		GameObject parent = NGUIEditorTools.SelectedRoot();
		if (parent == null) parent = NJGMap.instance.GetComponentInChildren<UIPanel>() != null ? NJGMap.instance.GetComponentInChildren<UIPanel>().gameObject : null;
		GameObject go = NGUITools.AddChild(parent);
		go.name = isMiniMap ? "MiniMap" : "WorldMap";

		UITexture ut = null;
		int depth = 0;

		if (isMiniMap)
		{
			// Create MiniMap
			UIMiniMap mp = go.AddComponent<UIMiniMap>();
			ut = NJGEditorTools.CreateUIMapTexture(mp);
			depth = (int)Mathf.Abs(ut.cachedTransform.localPosition.z) > 0 ? -(int)Mathf.Abs(ut.cachedTransform.localPosition.z) : -1;

			// Add Anchor
			UIAnchor a = go.AddComponent<UIAnchor>();
			a.side = UIAnchor.Side.TopRight;

			// Title
			UILabel title = CreateLabel(go, NJGMap.instance.worldName);
			title.gameObject.AddComponent<UILabelWorldName>();
			title.cachedTransform.localPosition = new Vector3(0, ut.cachedTransform.localScale.y / 2, depth);
		}
		else
		{
			// Create WorldMap
			UIWorldMap mp = go.AddComponent<UIWorldMap>();
			ut = NJGEditorTools.CreateUIMapTexture(mp);
			depth = (int)Mathf.Abs(ut.cachedTransform.localPosition.z) > 0 ? -(int)Mathf.Abs(ut.cachedTransform.localPosition.z) : -1;

			// Add Anchor
			UIAnchor a = go.AddComponent<UIAnchor>();
			a.side = UIAnchor.Side.Center;

			// Close button
			GameObject button = CreateButton(go, "Close", 100, 34);
			button.transform.parent = go.transform;
			button.transform.localScale = Vector3.one;
			button.transform.localPosition = new Vector3(0, -ut.cachedTransform.localScale.y / 2, depth);
			button.AddComponent<UIButtonCloseMap>();

			// Title
			UILabel title = CreateLabel(go, NJGMap.instance.worldName);
			title.gameObject.AddComponent<UILabelWorldName>();
			title.cachedTransform.localPosition = new Vector3(0, ut.cachedTransform.localScale.y / 2, depth);
		}

		Selection.activeGameObject = go;
	}	

	/// <summary>
	/// Save the generated texture.
	/// </summary>
	
	static public Texture2D SaveTexture(Texture2D tex)
	{
#if UNITY_EDITOR
		if (tex == null) return null;
		if (tex.GetInstanceID() > 0) return null;
		byte[] bytes = tex.EncodeToPNG();
		if(tex.GetInstanceID() < 0) NGUITools.DestroyImmediate(tex);

		string path = GetPath();

		string[] arr = EditorApplication.currentScene.Split('/');
		string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);

		string fileName = scene + ".png";
		//Debug.Log("path " + ("Assets/" + contentFolder + "/_Generated Content/" + fileName) + " - " + (path + fileName));
		System.IO.File.WriteAllBytes(path + fileName, bytes);
		AssetDatabase.Refresh();

		Texture2D tx = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + contentFolder + "/_Generated Content/" + fileName, typeof(Texture2D));
		//EditorUtility.CompressTexture(tx, TextureFormat.ATF_RGB_DXT1, TextureCompressionQuality.Fast);
		
		EditorGUIUtility.PingObject(tx);
		Debug.Log("Map texture generated at: " + contentFolder + "/" + fileName);
		//Debug.Log("tx " + tx);
		return tx;
#endif
	}

	static public Texture2D GetTexture()
	{
		string[] arr = EditorApplication.currentScene.Split('/');
		string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);
		string fileName = scene + ".png";
		return (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/"+contentFolder + "/_Generated Content/" + fileName, typeof(Texture2D));
	}

	static public Vector2 GetGameViewSize()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
		return (Vector2)Res;
	}

	static public EditorWindow GetMainGameView()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null, null);
		return (EditorWindow)Res;
	}

	static string GetPath()
	{
		string[] arr = EditorApplication.currentScene.Split('/');
		string path = Application.dataPath + "/" + contentFolder + "/_Generated Content/";
		// + arr[arr.Length - 1].Replace(".unity", "/")

		//Debug.Log("path " + path);

		if (!System.IO.Directory.Exists(path))
			System.IO.Directory.CreateDirectory(path);

		return path;
	}

	/// <summary>
	/// Button creation function.
	/// </summary>

	static string mButton = "Button";
	static void OnButton(string val) { mButton = val; }

	static public GameObject CreateButton(GameObject go, string name, int w, int h)
	{
		if (NJGMap.instance.atlas != null)
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Button - " + name;

			UISlicedSprite bg = NGUITools.AddWidget<UISlicedSprite>(go);
			//bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NJGMap.instance.atlas;
			bg.spriteName = mButton;
			bg.transform.localScale = new Vector3(w, h, 1f);
			bg.MakePixelPerfect();

			if (NGUISettings.font != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = NGUISettings.font;
				lbl.text = name;
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>();
			go.AddComponent<UIButtonOffset>();
			go.AddComponent<UIButtonSound>();
		}

		return go;
	}

	/// <summary>
	/// Label creation function.
	/// </summary>

	static public UILabel CreateLabel(GameObject go, string content)
	{
		if (NGUISettings.font != null)
		{
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.font = NGUISettings.font;
			lbl.text = content;
			lbl.color = Color.white;
			lbl.MakePixelPerfect();
			Selection.activeGameObject = lbl.gameObject;
			return lbl;
		}
		return null;
	}
}
