//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Button Toggle World Map")]
public class UIButtonToggleMap : MonoBehaviour 
{
	void OnClick()
	{
		UIMiniMap.instance.ToggleWorldMap();
	}
}
