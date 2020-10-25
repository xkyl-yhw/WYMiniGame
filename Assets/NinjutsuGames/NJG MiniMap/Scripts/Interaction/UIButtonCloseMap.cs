//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Button Close Map")]
public class UIButtonCloseMap : MonoBehaviour 
{
	void OnClick()
	{
		if (UIMiniMap.instance)		
			UIMiniMap.instance.HideWorldMap();		
	}
}
