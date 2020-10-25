//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Label Coords")]
[RequireComponent(typeof(UILabel))]
public class UILabelCoords : MonoBehaviour
{
	UILabel label;

	void Awake() { label = GetComponent<UILabel>(); }

	void Update()
	{
		if (UIMiniMap.instance == null)
			return;

		if (UIMiniMap.instance.target)
		{
			label.text = "X:" + Mathf.RoundToInt(UIMiniMap.instance.target.position.x) + " Y:" + Mathf.RoundToInt(UIMiniMap.instance.target.position.z);
		}
	}
}
