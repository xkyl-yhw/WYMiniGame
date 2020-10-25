//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Label World Name")]
[RequireComponent(typeof(UILabel))]
public class UILabelWorldName : MonoBehaviour 
{
	UILabel label;

	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	void Start() 
	{ 		
		NJGMap.instance.onWorldNameChanged += OnNameChanged;
	}

	void OnNameChanged(string worldName)
	{
		label.text = worldName;
	}
}
