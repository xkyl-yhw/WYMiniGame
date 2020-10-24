//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game map can have icons on it -- this class takes care of animating them when needed.
/// </summary>

public class UIMapArrow : MonoBehaviour
{
	[SerializeField]
	public NJGMapItem item;
	[HideInInspector] public UISprite sprite;

	/// <summary>
	/// Cache transform for speed.
	/// </summary>
	
	private Transform mTrans;
	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	float rotationOffset = 0.0f;

	/// <summary>
	/// Cache icon root transform for speed.
	/// </summary>
	
	Transform mRoot;
	Transform mIconRoot { get { if (mRoot == null) mRoot = UIMiniMap.instance.iconRoot.transform; return mRoot; } }

	/// <summary>
	/// Cache camera transform for speed.
	/// </summary>

	Transform mCam;
	Transform mCamera { get { if (mCam == null) mCam = Camera.main.transform; return mCam; } }

	Transform mTarget;

	/// <summary>
	/// Triggered when the icon is visible on the map.
	/// </summary>

	public void UpdatePosition()
	{
		// If there is no target defined lets use the mainCamera
		if (UIMiniMap.instance.target == null && Camera.main != null)
		{
			if (mTarget != Camera.main.transform) mTarget = Camera.main.transform;
		}
		else if (UIMiniMap.instance.target != null && mTarget != UIMiniMap.instance.target) mTarget = UIMiniMap.instance.target;

		if (mTarget == null) return;

		Vector3 from = item.cachedTransform.position - UIMiniMap.instance.target.position;
		from.y = 0;
		from.Normalize();

		Vector3 cameraForward = UIMiniMap.instance.rotateWithPlayer ? mCamera.forward : mIconRoot.forward;
		float direction = Vector3.Dot(cameraForward, from);

		if (Vector3.Cross(cameraForward, from).y > 0)
			rotationOffset = (1.0f - direction) * -90;
		else
			rotationOffset = (1.0f - direction) * 90;

		cachedTransform.localRotation = Quaternion.Euler(0f, 0f, (rotationOffset));
	}
}