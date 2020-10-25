//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit NJGMapZone.
/// </summary>

[CustomEditor(typeof(NJGMapZone))]
public class NJGMapZoneInspector : Editor
{
	UISprite mSprite;
	NJGMapZone m;
	float extraSpace = 0;
	string mName;

	/// <summary>
	/// Draw the inspector.
	/// </summary>

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls(130f);
		m = target as NJGMapZone;

		NJGEditorTools.DrawEditMap();

		GUILayout.BeginHorizontal("AppToolbar");
		EditorGUILayout.LabelField(new GUIContent("Zone Name Preview", ""), GUILayout.Width(130f));
		GUI.contentColor = m.color;
		EditorGUILayout.LabelField(new GUIContent(m.zone, ""), EditorStyles.boldLabel);
		GUI.contentColor = Color.white;
		GUILayout.EndHorizontal();		

		m.level = NGUIEditorTools.DrawList("Level", NJGMap.GetLevels(), m.level);
		m.zone = NGUIEditorTools.DrawList("Zone", NJGMap.GetZones(m.level), m.zone);
		m.triggerTag = EditorGUILayout.TagField("Trigger Tag", m.triggerTag);
		m.colliderRadius = (int)EditorGUILayout.Slider("Collider Radius", m.colliderRadius, 1, 1000);

		mName = "Zone - [" + m.mId + "] " + m.zone;

		if (m.name != mName)
			m.name = mName;

		if (NJGMap.instance != null)
		{
			if (NJGMap.instance.atlas != null)
			{
				/*GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Icon Sprite", GUILayout.Width(100.0f));

				// Draw sprite preview.		
				Material mat = NJGMap.instance.atlas.spriteMaterial;

				if (mat != null)
				{
					Texture2D tex = mat.mainTexture as Texture2D;

					if (tex != null)
					{
						UIAtlas.Sprite sprite = m.sprite;
						Rect rect = sprite.outer;
						if (NJGMap.instance.atlas.coordinates == UIAtlas.Coordinates.Pixels)
						{
							rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
						}

						GUILayout.Space(4f);
						GUILayout.Label("", GUILayout.Height(NJGMap.instance.iconSize));
						GUI.color = m.color;
						DrawSprite(tex, rect, null, false, NJGMap.instance.iconSize);
						GUI.color = Color.white;

						extraSpace = NJGMap.instance.iconSize * (float)sprite.outer.height / sprite.outer.width;
					}

				}
				GUILayout.EndHorizontal();*/

				extraSpace = Mathf.Max(0f, extraSpace - 30f);
				//GUILayout.Space(extraSpace);
			}
			//EditorGUILayout.Separator();
		}

		NGUIEditorTools.DrawSeparator();
		
		GUILayout.BeginHorizontal();

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Add New Zone"))
		{
			NJGMenu.AddMapZone();
		}
		GUI.backgroundColor = Color.white;

		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("Delete Zone"))
		{
			Delete();
		}
		GUI.backgroundColor = Color.white;

		GUILayout.EndHorizontal();
		
		
		EditorGUILayout.Separator();

		if (GUI.changed && m != null)
			EditorUtility.SetDirty(m);
	}

	void Delete()
	{		
		NGUITools.DestroyImmediate(m.gameObject);
		
		if (NJGMapZone.list.size > 0)
			Selection.activeGameObject = NJGMapZone.list[NJGMapZone.list.size - 1].gameObject;
		else
			Selection.activeGameObject = NJGMap.instance.gameObject;
	}

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

		rect = new Rect(85, rect.yMin + 0, NJGMap.instance.iconSize, NJGMap.instance.iconSize);

		GUI.color = c;

		if (maxSize > 0)
		{
			float dim = maxSize / Mathf.Max(rect.width, rect.height);
			rect.width *= dim;
			rect.height *= dim;
		}

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
}
