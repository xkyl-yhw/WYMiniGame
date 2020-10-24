//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UIMiniMap))]
public class UIMiniMapInspector : Editor {

	UIMiniMap m;
	Texture mMask;
	Color mColor;
	Material mMat;

	/// <summary>
	/// Draw the inspector.
	/// </summary>

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls(120f);
		m = target as UIMiniMap;

		NJGEditorTools.DrawEditMap();

		if (m.uiTexture == null)
		{
			m.uiTexture = m.GetComponentInChildren<UITexture>();

			if (m.uiTexture == null)
			{
				GUI.backgroundColor = Color.red;
				EditorGUILayout.HelpBox("No UITexture found.\nCreate a new one.", MessageType.Error);
				GUI.backgroundColor = Color.white;

				if (GUILayout.Button("Create UITexture"))
				{
					NJGEditorTools.CreateUIMapTexture(m);
				}
				EditorGUILayout.Separator();
				return;
			}
		}
		else
		{
			if(m.uiTexture.material == null || mMat == null) mMat = NJGEditorTools.GetMaterial(m);
		}

		if (m.uiTexture.material != mMat)
		{
			m.uiTexture.material = mMat;
			EditorUtility.SetDirty(m);
		}

		GUILayout.BeginVertical();
		EditorGUILayout.Separator();

		GUILayout.BeginHorizontal();
		m.usePanelForIcons = EditorGUILayout.Toggle("Icons Panel", m.usePanelForIcons, GUILayout.Width(140f));
		GUI.contentColor = m.usePanelForIcons ? Color.cyan : Color.gray;
		EditorGUILayout.LabelField("+1 Draw Call but fix any z-depth issue for icons.");
		GUI.contentColor = Color.white;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m.limitBounds = EditorGUILayout.Toggle("Limit Map Bounds", m.limitBounds, GUILayout.Width(140f));
		GUI.contentColor = m.limitBounds ? Color.cyan : Color.gray;
		EditorGUILayout.LabelField("Prevent map to display beyond borders.");
		GUI.contentColor = Color.white;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		m.rotateWithPlayer = EditorGUILayout.Toggle("Lock", m.rotateWithPlayer, GUILayout.Width(140f));
		GUI.contentColor = m.rotateWithPlayer ? Color.cyan : Color.gray;
		EditorGUILayout.LabelField("Makes the map follow target rotation.");
		GUI.contentColor = Color.white;
		GUILayout.EndHorizontal();

		mMask = (Texture2D)EditorGUILayout.ObjectField("Map Mask", m.maskTexture, typeof(Texture2D), false);
		mColor = EditorGUILayout.ColorField("Map Color", m.mapColor);
		
		m.target = (Transform)EditorGUILayout.ObjectField(new GUIContent("Map Target", "The object that this map is going to follow"), m.target, typeof(Transform), true);
		m.targetTag = EditorGUILayout.TagField(new GUIContent("Target Tag", "Assign a tag to auto search for the Map Target"), m.targetTag);

		GUILayout.BeginHorizontal();
		m.zoom = EditorGUILayout.IntField(new GUIContent("Zoom", "Current zoom level"), m.zoom);
		m.zoom = Mathf.Clamp(m.zoom, (int)m.minZoom, (int)m.maxZoom);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Zoom Range", "Min and Max level of zoom"), GUILayout.Width(116.0f));

		m.minZoom = EditorGUILayout.FloatField(m.minZoom, GUILayout.Width(25.0f));
		EditorGUILayout.MinMaxSlider(ref m.minZoom, ref m.maxZoom, 1, 20);		
		m.maxZoom = EditorGUILayout.FloatField(m.maxZoom, GUILayout.Width(25.0f));
		m.minZoom = Mathf.Round(m.minZoom);
		m.maxZoom = Mathf.Round(m.maxZoom);
		GUILayout.EndHorizontal();

		m.mapBorderRadius = EditorGUILayout.FloatField(new GUIContent("Border Radius Limit", "If icons get farther this radius they will dissapear"), m.mapBorderRadius);

		m.northIcon = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Map Target", "The object that this map is going to follow"), m.northIcon, typeof(GameObject), true);
		if(m.northIcon != null)
			m.northIconOffset = EditorGUILayout.IntField(new GUIContent("North Icon Offset", "Adjust the north icon distance from map border"), m.northIconOffset);

		m.mapKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Map Key", "The key to toggle the world map"), m.mapKey);

		EditorGUILayout.Separator();
		GUILayout.EndVertical();		

		if (GUI.changed)
			EditorUtility.SetDirty(m);

		if (mMask != null)
		{
			if (m.maskTexture != mMask)
			{
				m.maskTexture = mMask;
				if (m.uiTexture != null) m.uiTexture.material.SetTexture("_Mask", m.maskTexture);
			}
		}

		if (m.mapColor != mColor)
		{
			m.mapColor = mColor;
			if (m.uiTexture != null) m.uiTexture.color = m.mapColor;
		}
	}
}
