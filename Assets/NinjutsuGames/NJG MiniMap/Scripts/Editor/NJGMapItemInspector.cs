//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit NJGMapItems.
/// </summary>

[CustomEditor(typeof(NJGMapItem))]
public class NJGMapItemInspector : Editor 
{
	UISprite mSprite;
	NJGMapItem m;
	float extraSpace = 0;

	/// <summary>
	/// Draw the Map Marker inspector.
	/// </summary>

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls(80f);
		m = target as NJGMapItem;		

		NJGEditorTools.DrawEditMap();

		string tooltip = "You can use to display name + anything else you want.\nFor example: Ore [FF0000]+100 Mineral[-]";

		EditorGUILayout.LabelField(new GUIContent("Tooltip Content", tooltip));
		m.content = EditorGUILayout.TextArea(m.content);

		GUI.backgroundColor = Color.gray;
		EditorGUILayout.HelpBox(tooltip, MessageType.Info);
		GUI.backgroundColor = Color.white;

		m.type = NGUIEditorTools.DrawList("Marker Type", NJGMap.mapItemTypes, m.type);

		if (NJGMap.instance != null)
		{
			if (NJGMap.instance.atlas != null)
			{
				GUILayout.BeginHorizontal();
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
				GUILayout.EndHorizontal();

				extraSpace = Mathf.Max(0f, extraSpace - 30f);
				//GUILayout.Space(extraSpace);
			}
			EditorGUILayout.Separator();			
		}

		if (UIMiniMap.instance != null)
		{
			string info = "Minimap position: " + UIMiniMap.instance.WorldToMap(m.cachedTransform.position).ToString();

			GUI.backgroundColor = Color.gray;
			EditorGUILayout.HelpBox(info, MessageType.Info);
			GUI.backgroundColor = Color.white;
		}

		EditorGUILayout.Separator();

		if (GUI.changed)
			EditorUtility.SetDirty(m);
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
