//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 Ninjutsu Games LTD.
//----------------------------------------------


using UnityEngine;
using System.Collections;

[AddComponentMenu("NJG MiniMap/Interaction/Button Zoom Mini Map")]
public class UIButtonZoom : MonoBehaviour 
{
	public bool zoomIn;

	void OnClick()
	{
		if (UIMiniMap.instance)
		{
			if (zoomIn)
				UIMiniMap.instance.ZoomIn();
			else
				UIMiniMap.instance.ZoomOut();
		}
	}
}
