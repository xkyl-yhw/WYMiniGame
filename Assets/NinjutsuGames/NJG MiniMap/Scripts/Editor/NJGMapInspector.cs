//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(NJGMap))]
public class NJGMapInspector : Editor
{
	static NJGMap m;
	string mSpriteName;
	Texture2D mTex;
	Texture2D mUserTex;
	int mLayer;
	bool saveTexture;
	bool mSet;
	//LayerMask mBoundLayers;

	SerializedProperty renderLayers;
	SerializedProperty boundLayers;

	int selGridInt = 0;
	string[] selStrings = new string[] { "General Settings", "Icons Settings", "Zones Settings" };

	void OnEnable()
	{
		renderLayers = serializedObject.FindProperty("renderLayers");
		boundLayers = serializedObject.FindProperty("boundLayers");
	}	

	/// <summary>
	/// Draw the inspector.
	/// </summary>

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUIUtility.LookLikeControls(120f);
		m = target as NJGMap;		

		if (string.IsNullOrEmpty(LayerMask.LayerToName(m.gameObject.layer)))
		{
			GUI.backgroundColor = Color.red;
			EditorGUILayout.HelpBox("You need to create a special layer for this game object\nPlease create or assign a layer for this gameObject.\nYou can use the same layer of your NGUI UI", MessageType.Error);
			GUI.backgroundColor = Color.white;
		}

		if (UIMiniMap.instance == null && UIWorldMap.map == null)
		{
			GUI.backgroundColor = Color.red;
			EditorGUILayout.HelpBox("Could not found any UIMiniMap or UIWorldMap instance.\nClick create buttons below", MessageType.Error);
			GUI.backgroundColor = Color.white;
		}

		/*GUILayout.BeginHorizontal();
		ComponentSelector.Draw<UIFont>(NGUISettings.font, OnSelectFont, GUILayout.Width(140f));
		GUILayout.Label("Font used by labels");
		GUILayout.EndHorizontal();*/

		EditorGUILayout.Separator();
		EditorGUILayout.BeginHorizontal();
		
		GUI.backgroundColor = UIMiniMap.instance != null ? Color.cyan : Color.green;

		if (GUILayout.Button(new GUIContent(UIMiniMap.instance != null ? "Edit Mini Map" : "Create Mini Map", "Click to edit the Mini Map")))
		{
			if (UIMiniMap.instance != null)
			{
				Selection.activeGameObject = UIMiniMap.instance.gameObject;
			}
			else
			{
				NJGEditorTools.CreateMap(true);
			}	
		}

		GUI.backgroundColor = UIWorldMap.map != null ? Color.cyan : Color.green;

		if (GUILayout.Button(new GUIContent(UIWorldMap.map != null ? "Edit World Map" : "Create World Map", "Click to edit the World Map")))
		{
			if (UIWorldMap.map != null)
			{
				Selection.activeGameObject = UIWorldMap.map.gameObject;
			}
			else
			{
				NJGEditorTools.CreateMap(false);
			}		
		}

		if (!mSet)
		{
			m.SetTexture(m.generateMapTexture ? null : m.userMapTexture);
			mSet = true;
			if (UIWorldMap.map != null)
			{
				if (UIWorldMap.map.uiTexture != null)
				{
					//if(UIWorldMap.map.uiTexture.material == null) 
					//	UIWorldMap.map.uiTexture.material = NJGEditorTools.GetMaterial(UIWorldMap.map);
				}
			}

			if (UIMiniMap.instance != null)
			{
				if (UIMiniMap.instance.uiTexture != null)
				{
					//if (UIMiniMap.instance.uiTexture.material == null) 
					//	UIMiniMap.instance.uiTexture.material = NJGEditorTools.GetMaterial(UIMiniMap.instance);
				}
			}
		}

		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;

		//NGUIEditorTools.DrawSeparator();

		GUILayout.BeginVertical("AppToolbar");
		GUILayout.Space(10);
		selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3, GUILayout.Height(30));
		GUILayout.Space(10);
		GUILayout.EndVertical();

		if (selGridInt == 0)
			DrawGeneralSettingsUI();
		else if (selGridInt == 1)
			DrawIconsUI();
		else if (selGridInt == 2)
			DrawLevelsUI();		

		EditorGUILayout.Separator();

		serializedObject.ApplyModifiedProperties();
		Save(false);
	}

	#region General Settings
	void DrawGeneralSettingsUI()
	{
		
		EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
		EditorGUILayout.Separator();

		m.worldName = EditorGUILayout.TextField(new GUIContent("World Name", "The name of the world that is going to be displayed on World Map and Mini Map titles."), m.worldName);

		mLayer = EditorGUILayout.LayerField(new GUIContent("Layer", "The layer of the whole map system"), m.layer);

		if (m.layer != mLayer)
		{
			m.layer = mLayer;
			SetLayerRecursively(m.transform, m.layer);
			m.boundLayers = LayerMaskExtensions.RemoveFromMask(m.boundLayers, new string[] { LayerMask.LayerToName(m.layer) });
		}

		EditorGUILayout.LabelField(new GUIContent("Scene Bounds", "Scene bounds calculated automatically"));

		string boundsText = "Center: X:" + System.Math.Round(m.bounds.center.x, 2) + ", Y:" + System.Math.Round(m.bounds.center.y, 2) + ", Z:" + System.Math.Round(m.bounds.center.z, 2);
		boundsText += "\nExtents: X:" + System.Math.Round(m.bounds.extents.x, 2) + ", Y:" + System.Math.Round(m.bounds.extents.y, 2) + ", Z:" + System.Math.Round(m.bounds.extents.z, 2);
		boundsText += "\nSize: X:" + System.Math.Round(m.bounds.size.x, 2) + ", Y:" + System.Math.Round(m.bounds.size.y, 2) + ", Z:" + System.Math.Round(m.bounds.size.z, 2);

		GUI.backgroundColor = Color.black;
		EditorGUILayout.HelpBox(boundsText, MessageType.Info);
		GUI.backgroundColor = Color.white;

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.PropertyField(boundLayers, new GUIContent("Boundary Layers", "Which layers are going to be used for bounds calculation."));

		if ((m.boundLayers.value & 1 << m.layer) != 0)
		{
			m.boundLayers = LayerMaskExtensions.RemoveFromMask(m.boundLayers, new string[] { LayerMask.LayerToName(m.layer) });
		}

		//if (m.boundLayers != mBoundLayers) m.boundLayers = mBoundLayers;

		if (m.boundLayers.value == 0)
		{
			m.UpdateBounds();
			GUI.backgroundColor = Color.red;
			EditorGUILayout.HelpBox("Need a layer in order to get objects for bounds calculation.", MessageType.Error);
			GUI.backgroundColor = Color.white;
		}
		else
		{
			if (GUILayout.Button(new GUIContent("Update Bounds", "Click to recalculate scene bounds"), GUILayout.Width(120f)))
			{
				Resources.UnloadUnusedAssets();
				m.UpdateBounds();
			}
		}

		EditorGUILayout.EndHorizontal();
		NGUIEditorTools.DrawSeparator();
		//m.orientation = (NJGMap.Orientation) EditorGUILayout.EnumPopup(new GUIContent("Orientation", "Wheter if its a normal 3d game or side scroller"), m.orientation);

		EditorGUIUtility.LookLikeControls(200f);
		GUI.enabled = !Application.isPlaying;
		m.generateMapTexture = EditorGUILayout.Toggle("Use Automatic Map Generation", m.generateMapTexture);
		GUI.enabled = true;
		if (m.generateMapTexture)
		{
			if (m.useTextureGenerated)
			{
				mTex = m.mapTexture = m.userMapTexture = null;
				m.useTextureGenerated = false;
				m.SetTexture(null);
				Save(true);
			}
		}
		else
		{
			if (!m.useTextureGenerated)
			{
				if (mUserTex != null) m.userMapTexture = mUserTex;
				if (m.userMapTexture == null && mUserTex == null) m.userMapTexture = mUserTex = NJGEditorTools.GetTexture();
				m.useTextureGenerated = true;
				m.SetTexture(m.userMapTexture);
				Save(true);
			}
		}

		EditorGUILayout.BeginHorizontal();
		GUI.enabled = !m.generateMapTexture && !Application.isPlaying;
		if (m.generateMapTexture)
		{
			mTex = (Texture2D)EditorGUILayout.ObjectField("Map Texture", m.mapTexture, typeof(Texture2D), false);
			if (m.mapTexture != mTex)
			{
				Resources.UnloadUnusedAssets();
				m.mapTexture = mTex;
			}
		}
		else
		{
			if (saveTexture)
			{
				if (NJGEditorTools.GetTexture() != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(NJGEditorTools.GetTexture()));
				if (GameObject.Find("NJGMapRenderer")) NGUITools.DestroyImmediate(GameObject.Find("NJGMapRenderer"));
				Resources.UnloadUnusedAssets();
				m.userMapTexture = mUserTex = NJGEditorTools.SaveTexture(m.userMapTexture);
				m.SetTexture(m.userMapTexture);
				if (NJGMapRenderer.instance != null) NGUITools.DestroyImmediate(NJGMapRenderer.instance);
				saveTexture = false;
				Save(true);
				//ToggleCameras(true);
			}

			mUserTex = (Texture2D)EditorGUILayout.ObjectField("Choose your Texture", m.userMapTexture, typeof(Texture2D), false, GUILayout.Width(60f));

			if (m.userMapTexture != mUserTex)
			{
				m.useTextureGenerated = true;
				Resources.UnloadUnusedAssets();
				m.userMapTexture = mUserTex;
				m.SetTexture(m.userMapTexture);
				Save(true);
			}							
		}		

		EditorGUILayout.BeginVertical();

		GUILayout.Space(20f);

		EditorGUILayout.BeginHorizontal();
		//GUILayout.Space(-40f);
		//EditorGUILayout.LabelField("Or", GUILayout.Width(20f));
		//GUILayout.Space(-80f);
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(-20f);		

		GUI.backgroundColor = Color.green;
		//GUI.enabled = !Application.isPlaying || m.generateMapTexture;
		if (GUILayout.Button(new GUIContent("Generate New Map Texture", "Click to generate map texture"), GUILayout.Height(40f)))
		{
			//ToggleCameras(false);
			Resources.UnloadUnusedAssets();
			if (m.mapTexture != null) NGUITools.DestroyImmediate(m.mapTexture);		

			//Debug.Log("Generating texture...");
			if (NJGEditorTools.GetTexture() != null)
			{
				//Debug.Log("Destroying current texture...");
				AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(NJGEditorTools.GetTexture()));
			}
			
			//Debug.Log("Swith to Game View...");	
			NJGEditorTools.GetMainGameView().Focus();
			//Debug.Log("Lets take the photo...");
			m.GenerateMap();
			saveTexture = true;
			Save(true);
		}
		GUI.backgroundColor = Color.white;	
		EditorGUILayout.HelpBox("Game view must be active to generate a texture", MessageType.None);
		//EditorGUILayout.LabelField("Game view must be active to generate a texture");
		EditorGUILayout.EndVertical();
		GUI.enabled = true;
		
		
		EditorGUILayout.EndHorizontal();
		EditorGUIUtility.LookLikeControls(160f);
		GUI.enabled = true;

		GUI.backgroundColor = Color.black;

		if (!Application.isPlaying)		
			m.mapSize = new Vector2(NJGEditorTools.GetGameViewSize().x, NJGEditorTools.GetGameViewSize().y);		
		else		
			m.mapSize = new Vector2(Screen.width, Screen.height);

		GUI.backgroundColor = Color.white;

		EditorGUILayout.LabelField("Texture Information");

		string info = "Screen Size width:" + m.mapSize.x + " height:" + m.mapSize.y;
		float memSize = 0f;

		if (m.mapTexture != null)
		{			
			memSize = (float)CalculateTextureSizeBytes(m.mapTexture);
			info += "\nTexture Memory: " + System.Math.Round(((memSize / 1024) / 1024), 1) + " MB";
			info += "\nTexture Size: " + m.mapTexture.width + " x " + m.mapTexture.height;			
		}
		else if (m.userMapTexture != null && !m.generateMapTexture)
		{
			memSize = (float)CalculateTextureSizeBytes(m.userMapTexture);
			info += "\nTexture Memory: " + System.Math.Round(((memSize / 1024) / 1024), 1) + " MB";
			info += "\nTexture Size: " + m.userMapTexture.width + " x " + m.userMapTexture.height;
		}

		GUI.backgroundColor = Color.black;
		EditorGUILayout.HelpBox(info, MessageType.Info);
		GUI.backgroundColor = Color.white;
		EditorGUILayout.Separator();

		EditorGUILayout.BeginVertical("Box");
		//m.cameraClearFlags = (NJGCameraClearFlags)EditorGUILayout.EnumPopup("Camera Clear Flags", m.cameraClearFlags);
		m.mapFilterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", m.mapFilterMode);
		m.mapWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Texture Wrap Mode", m.mapWrapMode);
		//m.textureFormat = (NJGTextureFormat)EditorGUILayout.EnumPopup("Texture Format", m.textureFormat);
		m.compressTexture = EditorGUILayout.Toggle("Compress Texture", m.compressTexture);
		m.generateMipmaps = EditorGUILayout.Toggle("Generate Mipmaps", m.generateMipmaps);
		m.renderOffset = EditorGUILayout.IntField("Border Offset", m.renderOffset);

		EditorGUILayout.PropertyField(renderLayers, new GUIContent("Render Layers", "Which layers are going to be used for rendering."));

		if ((m.renderLayers.value & 1 << m.layer) != 0)
		{
			m.renderLayers = LayerMaskExtensions.RemoveFromMask(m.renderLayers, new string[] { LayerMask.LayerToName(m.layer) });
		}

		EditorGUILayout.EndVertical();	
		NGUIEditorTools.DrawSeparator();		

		EditorGUIUtility.LookLikeControls(120f);

		m.updateFrequency = EditorGUILayout.Slider(new GUIContent("Update Frequency", "How often the map will be updated"), m.updateFrequency, 0.01f, 1f);

		ComponentSelector.Draw<UIAtlas>(m.atlas, OnSelectAtlas);
		if (m.atlas == null)
		{
			EditorGUILayout.HelpBox("You need to select an atlas first", MessageType.Warning);			
		}

		NGUIEditorTools.DrawSeparator();
		GUILayout.BeginHorizontal();

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Add New Zone Game Object"))
		{
			NJGMenu.AddMapZone();
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();

		if (m.atlas == null)
			return;		
	}
	#endregion

	#region Icons

	bool iconsToggle;
	void DrawIconsUI()
	{
		bool errorName = false;
		
		EditorGUILayout.LabelField("Map Icon Settings", EditorStyles.boldLabel);

		m.iconSize = (int)EditorGUILayout.Slider(new GUIContent("Icon Size", "Global size of the map icons"), (float)m.iconSize, 1f, 128f);
		m.arrowSize = (int)EditorGUILayout.Slider(new GUIContent("Arrow Size", "Global size of the map arrows"), (float)m.arrowSize, 1f, 128f);

		GUILayout.BeginHorizontal();

		EditorGUILayout.LabelField(new GUIContent("Map Marker Types", "Create as many marker types as you want."));

		//var collapseIcon = '\u2261'.ToString();
		//bool collapse = GUILayout.Button(new GUIContent("Collapse All", "Click to collapse all"));

		//var expandIcon = '\u25A1'.ToString();
		//bool expand = GUILayout.Button(new GUIContent("Expand All", "Click to expand all"));

		if (GUILayout.Button(new GUIContent("Add New", "Click to add")))
			m.mapItems.Add(new NJGMap.MapItemType());

		bool iconsToggle = GUILayout.Button(new GUIContent(!m.typesFolded ? "Expand All" : "Collapse All", "Click to expand all"));

		if(iconsToggle)
		{
			m.typesFolded = !m.typesFolded;
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginVertical("AppToolbar");
		for (int i = 0; i < m.mapItems.Count; ++i)
		{
			NJGMap.MapItemType mapItem = m.mapItems[i];

			GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
			//GUILayout.Space(10f);
			mapItem.folded = EditorGUILayout.Foldout(mapItem.folded, mapItem.type);
			if (iconsToggle) mapItem.folded = m.typesFolded;
			//if (toggle) mapItem.folded = true;

			//GUI.enabled = i > 1;
			var upArrow = '\u25B2'.ToString();
			if (GUILayout.Button(new GUIContent(upArrow, "Click to shift up"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				if (i > 0)
				{
					NJGMap.MapItemType shiftItem = m.mapItems[i];
					m.mapItems.RemoveAt(i);
					m.mapItems.Insert(i - 1, shiftItem);
				}
			}
			GUI.enabled = i > 0;
			var dnArrow = '\u25BC'.ToString();
			if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift down"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				if (i + 1 < m.mapItems.Count)
				{
					NJGMap.MapItemType shiftItem = m.mapItems[i];
					m.mapItems.RemoveAt(i);
					m.mapItems.Insert(i + 1, shiftItem);
				}
			}
			GUI.enabled = true;

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button(new GUIContent("+", "Click to add"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
				m.mapItems.Add(new NJGMap.MapItemType());

			GUI.backgroundColor = Color.white;

			//GUI.enabled = i > 0;
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button(new GUIContent("-", "Click to remove"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				mapItem.deleteRequest = true;
			}
			GUI.backgroundColor = Color.white;
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			if (mapItem.deleteRequest)
			{
				// Show the confirmation dialog
				GUILayout.Label("Are you sure you want to delete '" + mapItem.type + "'?");
				NGUIEditorTools.DrawSeparator();

				GUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Cancel")) mapItem.deleteRequest = false;
					GUI.backgroundColor = Color.red;

					if (GUILayout.Button("Delete"))
					{
						m.mapItems.RemoveAt(i);
						--i;
						mapItem.deleteRequest = false;
					}
					GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.Separator();

			}
			else if (mapItem.folded)
			{
				GUILayout.BeginVertical();

				//GUI.enabled = i > 0;
				//if (i == 0) mapItem.type = "Default";
				mapItem.type = EditorGUILayout.TextField("Marker Type", mapItem.type);
				GUI.enabled = true;
				if (m.atlas != null)
				{
					if (string.IsNullOrEmpty(mSpriteName)) mSpriteName = m.atlas.spriteList[0].name;
					string spr = string.IsNullOrEmpty(mapItem.sprite) ? mSpriteName : mapItem.sprite;

					SpriteField("Icon Sprite", m.atlas, spr, delegate(string sp)
					{
						mapItem.OnSelectSprite(sp);
						Save(true);
					});

					float extraSpace = 0;

					// Draw sprite preview.					
					Material mat = m.atlas.spriteMaterial;

					if (mat != null)
					{
						Texture2D tex = mat.mainTexture as Texture2D;

						if (tex != null)
						{
							UIAtlas.Sprite mSprite = m.atlas.GetSprite(spr);

							if (mSprite != null)
							{
								Rect rect = mSprite.outer;
								if (m.atlas.coordinates == UIAtlas.Coordinates.Pixels)
								{
									rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
								}

								GUILayout.Space(4f);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Space((Screen.width - 220) - m.iconSize);
									GUI.color = mapItem.color;
									DrawSprite(tex, rect, null, false, m.iconSize);
									GUI.color = Color.white;
								}
								GUILayout.EndHorizontal();

								extraSpace = m.iconSize * (float)mSprite.outer.height / mSprite.outer.width;
							}
						}

						extraSpace = Mathf.Max(0f, extraSpace - 10f);
						GUILayout.Space(extraSpace);
					}
				}
				// Depth
				GUILayout.Space(2f);
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel("Depth");

					int depth = mapItem.depth;
					if (GUILayout.Button("Back", GUILayout.Width(60f))) --depth;
					depth = EditorGUILayout.IntField(depth);
					if (GUILayout.Button("Forward", GUILayout.Width(60f))) ++depth;

					if (mapItem.depth != depth)
					{
						mapItem.depth = depth;
					}
				}
				GUILayout.EndHorizontal();

				mapItem.color = EditorGUILayout.ColorField("Color", mapItem.color);
				mapItem.updatePosition = EditorGUILayout.Toggle("Update Position", mapItem.updatePosition);
				mapItem.animateOnVisible = EditorGUILayout.Toggle("Animate On Visible", mapItem.animateOnVisible);
				mapItem.loopAnimation = EditorGUILayout.Toggle("Loop Animation", mapItem.loopAnimation);
				mapItem.fadeOutAfterDelay = EditorGUILayout.FloatField("Fade Out Delay", mapItem.fadeOutAfterDelay);
				mapItem.rotate = EditorGUILayout.Toggle("Rotate", mapItem.rotate);
				mapItem.haveArrow = EditorGUILayout.Toggle("Display Arrow", mapItem.haveArrow);

				if (m.atlas != null && mapItem.haveArrow)
				{
					GUILayout.BeginVertical("Box");

					if (string.IsNullOrEmpty(mSpriteName)) mSpriteName = m.atlas.spriteList[0].name;
					string spr = string.IsNullOrEmpty(mapItem.arrowSprite) ? mSpriteName : mapItem.arrowSprite;

					mapItem.arrowOffset = EditorGUILayout.IntField("Arrow Offset", mapItem.arrowOffset);

					SpriteField("Arrow Sprite", m.atlas, spr, delegate(string sp)
					{
						mapItem.OnSelectArrowSprite(sp);
						Save(true);
					});					

					float extraSpace = 0;

					// Draw sprite preview.					
					Material mat = m.atlas.spriteMaterial;

					if (mat != null)
					{
						Texture2D tex = mat.mainTexture as Texture2D;

						if (tex != null)
						{
							UIAtlas.Sprite mSprite = m.atlas.GetSprite(spr);

							if (mSprite != null)
							{
								Rect rect = mSprite.outer;
								if (m.atlas.coordinates == UIAtlas.Coordinates.Pixels)
								{
									rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
								}

								GUILayout.Space(4f);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Space((Screen.width - 220) - m.arrowSize);
									GUI.color = mapItem.color;
									DrawSprite(tex, rect, null, false, m.arrowSize);
									GUI.color = Color.white;
								}
								GUILayout.EndHorizontal();

								extraSpace = m.arrowSize * (float)mSprite.outer.height / mSprite.outer.width;
							}
						}

						extraSpace = Mathf.Max(0f, extraSpace - 10f);
						GUILayout.Space(extraSpace);
					}
					// Depth
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Arrow Depth");

						int depth = mapItem.arrowDepth;
						if (GUILayout.Button("Back", GUILayout.Width(60f))) --depth;
						depth = EditorGUILayout.IntField(depth);
						if (GUILayout.Button("Forward", GUILayout.Width(60f))) ++depth;

						if (mapItem.arrowDepth != depth)
						{
							mapItem.arrowDepth = depth;
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}

				GUI.contentColor = Color.white;
				GUI.backgroundColor = Color.white;

				GUILayout.EndVertical();
			}

			if (string.IsNullOrEmpty(mapItem.sprite))
				EditorGUILayout.HelpBox("You need to assign a sprite name", MessageType.Error);

			errorName = NameIsDifferent(mapItem.type, i);

			if (errorName)
				EditorGUILayout.HelpBox("Type names must be different", MessageType.Error);
		}
		GUILayout.EndVertical();
	}
	#endregion

	#region Levels
	void DrawLevelsUI()
	{		
		bool errorName = false;
		
		EditorGUILayout.LabelField("Levels & Zones Settings", EditorStyles.boldLabel);		

		GUILayout.BeginHorizontal();

		//EditorGUILayout.LabelField(new GUIContent("Zones", "Create as many zones as you want."));

		if (GUILayout.Button(new GUIContent("Add New", "Click to add")))
		{
			NJGMap.MapLevel ml2 = new NJGMap.MapLevel();
			ml2.level = Application.loadedLevelName;
			m.levels.Add(ml2);
		}

		bool toggle = GUILayout.Button(new GUIContent(!m.zonesFolded ? "Expand All" : "Collapse All", "Click to expand all"));

		if (toggle)		
			m.zonesFolded = !m.zonesFolded;

		GUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("AppToolbar");
		for (int i = 0; i < m.levels.Count; ++i)
		{
			NJGMap.MapLevel item = m.levels[i];

			GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
			//GUILayout.Space(10f);
			item.folded = EditorGUILayout.Foldout(item.folded, item.level);
			if (toggle) item.folded = m.zonesFolded;

			var upArrow = '\u25B2'.ToString();
			if (GUILayout.Button(new GUIContent(upArrow, "Click to shift up"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				if (i > 0)
				{
					NJGMap.MapLevel shiftItem = m.levels[i];
					m.levels.RemoveAt(i);
					m.levels.Insert(i - 1, shiftItem);
				}
			}

			var dnArrow = '\u25BC'.ToString();
			if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift down"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				if (i + 1 < m.mapItems.Count)
				{
					NJGMap.MapLevel shiftItem = m.levels[i];
					m.levels.RemoveAt(i);
					m.levels.Insert(i + 1, shiftItem);
				}
			}

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button(new GUIContent("+", "Click to add"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				NJGMap.MapLevel ml = new NJGMap.MapLevel();
				ml.level = Application.loadedLevelName;
				m.levels.Add(ml);
			}

			GUI.backgroundColor = Color.white;

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button(new GUIContent("-", "Click to remove"), EditorStyles.toolbarButton, GUILayout.Width(24.0f)))
			{
				item.deleteRequest = true;
			}
			GUI.backgroundColor = Color.white;

			GUILayout.EndHorizontal();

			if (item.deleteRequest)
			{
				// Show the confirmation dialog
				GUILayout.Label("Are you sure you want to delete '" + item.level + "'?");
				NGUIEditorTools.DrawSeparator();

				GUILayout.BeginHorizontal();
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Cancel")) item.deleteRequest = false;
					GUI.backgroundColor = Color.red;

					if (GUILayout.Button("Delete"))
					{
						m.levels.RemoveAt(i);
						--i;
						item.deleteRequest = false;
					}
					GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.Separator();

			}
			else if (item.folded)
			{
				GUILayout.BeginVertical();
				DrawZoneUI(item);
				GUILayout.EndVertical();
			}

			errorName = SceneIsDifferent(item.level, i);

			if (errorName)
				EditorGUILayout.HelpBox("Type names must be different", MessageType.Error);
		}
		GUILayout.EndVertical();
	}


	void DrawZoneUI(NJGMap.MapLevel l)
	{
		bool errorName = false;

		GUILayout.BeginHorizontal();

		l.level = EditorGUILayout.TextField("Level", l.level);

		if (GUILayout.Button(new GUIContent("+", "Click to add"), GUILayout.Width(24.0f)))
			l.zones.Add(new NJGMap.MapZone());

		var collapseIcon = '\u2261'.ToString();
		var expandIcon = '\u25A1'.ToString();

		bool subToggle = GUILayout.Button(new GUIContent(!l.itemsFolded ? expandIcon : collapseIcon, "Click to expand all"), GUILayout.Width(24.0f));

		GUILayout.EndHorizontal();

		if (subToggle)		
			l.itemsFolded = !l.itemsFolded;
		
		GUILayout.BeginVertical();
		for (int i = 0; i < l.zones.Count; ++i)
		{
			NJGMap.MapZone item = l.zones[i];

			GUILayout.BeginHorizontal("PreToolbar");
			GUILayout.Space(10f);
			item.folded = EditorGUILayout.Foldout(item.folded, item.type);
			if (subToggle) item.folded = l.itemsFolded;

			var upArrow = '\u25B2'.ToString();
			if (GUILayout.Button(new GUIContent(upArrow, "Click to shift up"), "PreToolbar", GUILayout.Width(24.0f)))
			{
				if (i > 0)
				{
					NJGMap.MapZone shiftItem = l.zones[i];
					l.zones.RemoveAt(i);
					l.zones.Insert(i - 1, shiftItem);
				}
			}

			var dnArrow = '\u25BC'.ToString();
			if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift down"), "PreToolbar", GUILayout.Width(24.0f)))
			{
				if (i + 1 < m.mapItems.Count)
				{
					NJGMap.MapZone shiftItem = l.zones[i];
					l.zones.RemoveAt(i);
					l.zones.Insert(i + 1, shiftItem);
				}
			}

			GUI.backgroundColor = Color.green;
			if (GUILayout.Button(new GUIContent("+", "Click to add"), "PreToolbar", GUILayout.Width(24.0f)))
				l.zones.Add(new NJGMap.MapZone());

			GUI.backgroundColor = Color.white;

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button(new GUIContent("-", "Click to remove"), "PreToolbar", GUILayout.Width(24.0f)))
			{
				item.deleteRequest = true;
			}
			GUI.backgroundColor = Color.white;
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			if (item.deleteRequest)
			{
				// Show the confirmation dialog
				GUILayout.Label("Are you sure you want to delete '" + item.type + "'?");
				NGUIEditorTools.DrawSeparator();

				GUILayout.BeginHorizontal("AppToolbar");
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Cancel")) item.deleteRequest = false;
					GUI.backgroundColor = Color.red;

					if (GUILayout.Button("Delete"))
					{
						l.zones.RemoveAt(i);
						--i;
						item.deleteRequest = false;
					}
					GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.Separator();

			}
			else if (item.folded)
			{
				GUILayout.BeginVertical();

				item.type = EditorGUILayout.TextField("Zone", item.type);
				GUI.enabled = true;
				/*if (m.atlas != null)
				{
					string spr = string.IsNullOrEmpty(item.sprite) ? mSpriteName : item.sprite;

					SpriteField("Icon Sprite", m.atlas, spr, delegate(string sp)
					{
						item.OnSelectSprite(sp);
						Save(true);
					});

					float extraSpace = 0;

					// Draw sprite preview.					
					Material mat = m.atlas.spriteMaterial;

					if (mat != null)
					{
						Texture2D tex = mat.mainTexture as Texture2D;

						if (tex != null)
						{
							UIAtlas.Sprite mSprite = m.atlas.GetSprite(spr);

							if (mSprite != null)
							{
								Rect rect = mSprite.outer;
								if (m.atlas.coordinates == UIAtlas.Coordinates.Pixels)
								{
									rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
								}

								GUILayout.Space(4f);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Space((Screen.width - 220) - m.iconSize);
									GUI.color = item.color;
									DrawSprite(tex, rect, null, false, m.iconSize);
									GUI.color = Color.white;
								}
								GUILayout.EndHorizontal();

								extraSpace = m.iconSize * (float)mSprite.outer.height / mSprite.outer.width;
							}
						}

						extraSpace = Mathf.Max(0f, extraSpace - 10f);
						GUILayout.Space(extraSpace);
					}
				}*/

				item.color = EditorGUILayout.ColorField("Color", item.color);
				item.fadeOutAfterDelay = EditorGUILayout.FloatField("Fade Out Delay", item.fadeOutAfterDelay);

				// Depth
				//NJGEditorTools.DrawDepthUI(item);

				GUI.contentColor = Color.white;
				GUI.backgroundColor = Color.white;

				GUILayout.EndVertical();
				EditorGUILayout.Separator();
			}

			errorName = NameIsDifferent(item.type, i);

			if (errorName)
				EditorGUILayout.HelpBox("Type names must be different", MessageType.Error);
		}
		GUILayout.EndVertical();		
	}

	#endregion

	#region Helper Methods

	static List<Camera> cameras = new List<Camera>();

	static void ToggleCameras(bool flag)
	{
		//find enabled cameras
		if (cameras.Count == 0)
		{
			UnityEngine.Object[] cams = FindObjectsOfType(typeof(Camera));

			Debug.Log("cams " + cams.Length);

			foreach (Camera c in cams)
				if (c.enabled) cameras.Add(c);			
		}

		Debug.Log("cameras " + cameras.Count);
		foreach (Camera c in cameras)
			c.enabled = flag;		
	}

	/// <summary>
	/// Sets the layer of the passed transform and all of its children
	/// </summary>
	
	private static void SetLayerRecursively(Transform root, int layer)
	{
		root.gameObject.layer = layer;
		foreach (Transform child in root)
			SetLayerRecursively(child, layer);
	}

	static public void Save(bool force)
	{
		if (GUI.changed || force)
		{
			m.UpdateBounds();
			EditorUtility.SetDirty(m);
		}
	}
	
	void OnSelectFont(MonoBehaviour obj)
	{
		NGUISettings.font = obj as UIFont;
		Repaint();
	}

	/// <summary>
	/// Save selected atlas.
	/// </summary>

	void OnSelectAtlas(MonoBehaviour obj)
	{
		m.atlas = obj as UIAtlas;

		// Automatically choose the first sprite
		if (string.IsNullOrEmpty(mSpriteName))
		{
			if (m.atlas != null && m.atlas.spriteList.Count > 0)
			{
				SetAtlasSprite(m.atlas.spriteList[0]);
				mSpriteName = m.defaultSprite.name;
			}
		}
	}

	/// <summary>
	/// Set the atlas sprite directly.
	/// </summary>

	protected void SetAtlasSprite(UIAtlas.Sprite sp)
	{
		if (sp != null)
		{
			m.defaultSprite = sp;
			mSpriteName = sp.name;
		}
		else
		{
			mSpriteName = (m.defaultSprite != null) ? m.defaultSprite.name : "";
			m.defaultSprite = sp;
		}
	}

	/// <summary>
	/// Check if name is different
	/// </summary>

	bool NameIsDifferent(string name, int index)
	{
		int count = 0;

		for (int i = 0, imax = m.mapItems.Count; i < imax; i++)
			if (i != index && name == m.mapItems[i].type) count++;

		return count >= 1;
	}

	bool SceneIsDifferent(string name, int index)
	{
		int count = 0;

		for (int i = 0, imax = m.levels.Count; i < imax; i++)
			if (i != index && name == m.levels[i].level) count++;

		return count >= 1;
	}

	int CalculateTextureSizeBytes(Texture tTexture)
	{

		int tWidth = tTexture.width;
		int tHeight = tTexture.height;
		if (tTexture is Texture2D)
		{
			Texture2D tTex2D = tTexture as Texture2D;
			int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
			int mipMapCount = tTex2D.mipmapCount;
			int mipLevel = 1;
			int tSize = 0;
			while (mipLevel <= mipMapCount)
			{
				tSize += tWidth * tHeight * bitsPerPixel / 8;
				tWidth = tWidth / 2;
				tHeight = tHeight / 2;
				mipLevel++;
			}
			return tSize;
		}
		return 0;
	}

	int GetBitsPerPixel(TextureFormat format)
	{
		switch (format)
		{
			case TextureFormat.Alpha8: //	 Alpha-only texture format.
				return 8;
			case TextureFormat.ARGB4444: //	 A 16 bits/pixel texture format. Texture stores color with an alpha channel.
				return 16;
			case TextureFormat.RGB24:	// A color texture format.
				return 24;
			case TextureFormat.RGBA32:	//Color with an alpha channel texture format.
				return 32;
			case TextureFormat.ARGB32:	//Color with an alpha channel texture format.
				return 32;
			case TextureFormat.RGB565:	//	 A 16 bit color texture format.
				return 16;
			case TextureFormat.DXT1:	// Compressed color texture format.
				return 4;
			case TextureFormat.DXT5:	// Compressed color with alpha channel texture format.
				return 8;
			/*
			case TextureFormat.WiiI4:	// Wii texture format.
			case TextureFormat.WiiI8:	// Wii texture format. Intensity 8 bit.
			case TextureFormat.WiiIA4:	// Wii texture format. Intensity + Alpha 8 bit (4 + 4).
			case TextureFormat.WiiIA8:	// Wii texture format. Intensity + Alpha 16 bit (8 + 8).
			case TextureFormat.WiiRGB565:	// Wii texture format. RGB 16 bit (565).
			case TextureFormat.WiiRGB5A3:	// Wii texture format. RGBA 16 bit (4443).
			case TextureFormat.WiiRGBA8:	// Wii texture format. RGBA 32 bit (8888).
			case TextureFormat.WiiCMPR:	//	 Compressed Wii texture format. 4 bits/texel, ~RGB8A1 (Outline alpha is not currently supported).
				return 0;  //Not supported yet
			*/
			case TextureFormat.PVRTC_RGB2://	 PowerVR (iOS) 2 bits/pixel compressed color texture format.
				return 2;
			case TextureFormat.PVRTC_RGBA2://	 PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
				return 2;
			case TextureFormat.PVRTC_RGB4://	 PowerVR (iOS) 4 bits/pixel compressed color texture format.
				return 4;
			case TextureFormat.PVRTC_RGBA4://	 PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
				return 4;
			case TextureFormat.ETC_RGB4://	 ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
				return 4;
			case TextureFormat.ETC2_RGBA8://	 ATC (ATITC) 8 bits/pixel compressed RGB texture format.
				return 8;
			case TextureFormat.BGRA32://	 Format returned by iPhone camera
				return 32;
			//case TextureFormat.ATF_RGB_DXT1://	 Flash-specific RGB DXT1 compressed color texture format.

			//case TextureFormat.ATF_RGBA_JPG://	 Flash-specific RGBA JPG-compressed color texture format.
			//case TextureFormat.ATF_RGB_JPG://	 Flash-specific RGB JPG-compressed color texture format.
				//return 0; //Not supported yet
		}
		return 0;
	}

	#endregion

	#region Draw Inspector Preview

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI(Rect rect, GUIStyle background)
	{
		if (m.mapTexture == null) return;

		Texture2D tex = m.mapTexture;
		if (tex == null) return;

		int size = 256;
		Material mat = AssetDatabase.LoadAssetAtPath("Assets/NinjutsuGames/NG GameMap/Materials/WorldNJGMap.mat", typeof(Material)) as Material;

		// We only want to draw into this rectangle
		if (Event.current.type == EventType.Repaint)
		{
			UnityEditor.EditorGUI.DrawPreviewTexture(new Rect(rect.x + size/3f, rect.y, size, size), tex, mat == null ? null : mat);
		}
	}

	public override bool HasPreviewGUI() { return false; }

	#endregion

	#region Draw Sprite Preview

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat) { return DrawSprite(tex, sprite, mat, true, 0); }

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat, bool addPadding)
	{
		return DrawSprite(tex, sprite, mat, addPadding, 0);
	}

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat, bool addPadding, int maxSize)
	{
		float paddingX = addPadding ? 4f / tex.width : 0f;
		float paddingY = addPadding ? 4f / tex.height : 0f;
		float ratio = (sprite.height + paddingY) / (sprite.width + paddingX);

		ratio *= (float)tex.height / tex.width;

		// Draw the checkered background
		Color c = GUI.color;
		Rect rect = GUILayoutUtility.GetRect(0f, 0f);
		rect.width = Screen.width - rect.xMin;
		rect.height = rect.width * ratio;

		rect = new Rect(250, rect.yMin - 18, maxSize, maxSize);

		GUI.color = c;

		/*if (maxSize > 0)
		{
			float dim = maxSize / Mathf.Max(rect.width, rect.height);
			rect.width *= dim;
			rect.height *= dim;
		}*/

		// We only want to draw into this rectangle
		if (Event.current.type == EventType.Repaint)
		{
			if (mat == null)
			{
				GUI.DrawTextureWithTexCoords(rect, tex, sprite);
			}
			else
			{
				// NOTE: DrawPreviewTexture doesn't seem to support BeginGroup-based clipping
				// when a custom material is specified. It seems to be a bug in Unity.
				// Passing 'null' for the material or omitting the parameter clips as expected.
				UnityEditor.EditorGUI.DrawPreviewTexture(sprite, tex, mat);
				//UnityEditor.EditorGUI.DrawPreviewTexture(drawRect, tex);
				//GUI.DrawTexture(drawRect, tex);
			}
			rect = new Rect(sprite.x + rect.x, sprite.y + rect.y, sprite.width, sprite.height);
		}
		return rect;
	}

	#endregion

	#region Sprite Field
	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void SpriteField(string fieldName, UIAtlas atlas, string spriteName,
		SpriteSelector.Callback callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(fieldName, GUILayout.Width(76f));

		if (GUILayout.Button(spriteName, "MiniPullDown", options))
		{
			SpriteSelector.Show(atlas, spriteName, callback);
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void SpriteField(string fieldName, UIAtlas atlas, string spriteName, SpriteSelector.Callback callback)
	{
		SpriteField(fieldName, null, atlas, spriteName, callback);
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void SpriteField(string fieldName, string caption, UIAtlas atlas, string spriteName, SpriteSelector.Callback callback)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(fieldName, GUILayout.Width(116f));

		if (atlas.GetSprite(spriteName) == null)
			spriteName = "";

		if (GUILayout.Button(spriteName, "MiniPullDown", GUILayout.Width(120f)))
		{
			SpriteSelector.Show(atlas, spriteName, callback);
		}

		if (!string.IsNullOrEmpty(caption))
		{
			GUILayout.Space(20f);
			GUILayout.Label(caption);
		}
		
		GUILayout.EndHorizontal();
		GUILayout.Space(-4f);
		
	}
	#endregion
}
