//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(UIWorldMap))]
public class UIWorldMapInspector : Editor 
{
	UIWorldMap m;
	Texture mMask;
	Color mColor;
	Material mMat;

	/// <summary>
	/// Draw the inspector.
	/// </summary>

	public override void OnInspectorGUI()
	{
		if (target is UIMiniMap) return;
		EditorGUIUtility.LookLikeControls(120f);
		m = target as UIWorldMap;

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
			if (m.uiTexture.material == null || mMat == null) mMat = NJGEditorTools.GetMaterial(m);
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

		mMask = (Texture2D)EditorGUILayout.ObjectField("Map Mask", m.maskTexture, typeof(Texture2D), false);
		EditorGUILayout.Separator();

		m.mapColor = EditorGUILayout.ColorField("Map Color", m.mapColor);

		EditorGUILayout.Separator();
		GUILayout.EndVertical();

		if (mMask != null)
		{
			if (m.maskTexture != mMask)
			{
				m.maskTexture = mMask;
				if (m.uiTexture != null) m.uiTexture.material.SetTexture("_Mask", m.maskTexture);
			}
		}

		if (mColor != m.mapColor)
		{
			mColor = m.mapColor;
			if (m.uiTexture != null) m.uiTexture.color = m.mapColor;
		}

		if (GUI.changed)
			EditorUtility.SetDirty(m);
	}
}
