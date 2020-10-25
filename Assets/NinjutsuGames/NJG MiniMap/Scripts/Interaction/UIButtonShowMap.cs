//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Button Show World Map")]
public class UIButtonShowMap : MonoBehaviour 
{
	void OnClick()
	{
		if (UIMiniMap.instance)		
			UIMiniMap.instance.ShowWorldMap();		
	}
}
