// ======================================================================================
// MIT License:
//	Copyright (C) 2013 Eu-Ming Lee

//	Permission is hereby granted, free of charge, to any person obtaining a copy of this 
//	software and associated documentation files (the "Software"), to deal in the Software 
//	without restriction, including without limitation the rights to use, copy, modify, 
//	merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
//	permit persons to whom the Software is furnished to do so, subject to the following 
//	conditions:

//	The above copyright notice and this permission notice shall be included in all copies 
//	or substantial portions of the Software.

//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
//	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ======================================================================================
// File         : TouchListener.cs
// Author       : Eu-Ming Lee 
// Changelist   :
//	7/18/2012 - First creation
// Description  : 
//	Expected to be attached to a Camera to receive Input commands from Mobile devices
//	in order to simulate Mouse controls.
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using CustomExtensions;

///////////////////////////////////////////////////////////////////////////////
///
/// TouchListener
/// 
///////////////////////////////////////////////////////////////////////////////
[System.Serializable] // Required so it shows up in the inspector 
[LitJson.ExportType(LitJson.ExportType.NoExport)]	//	use this to prevent specific fields from being exported by LitJson library
[AddComponentMenu ("GUI/TouchListener (Camera)")]
public class TouchListener : MonoBehaviour
{
	private Camera			m_camera;
	private GameObject[]	m_currentlyTouchedGO = {null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null};	//	16 slots. 7 wasn't enough!
	//private	Publisher		m_myPublisher;
	
	void Awake()
	{
		m_camera = GetComponent<Camera>();
		if (m_camera == null) {
			Rlplog.Error("TouchListener.Awake", "No Camera found!");
		}
		//m_myPublisher = GetComponent<Publisher>();
	}
	
	/*
	 * 	tries to simulate a mouse press. Should handle the case where you press on the button, but then move your finger off the button.
	 * 	Like the mouse controls, it activates when you release, not when you press.
	 */
	void TouchTapSelect()
	{
		Camera cam = m_camera;
		GameObject hitGO = null;
		
		int TouchNum = 0;
		
		foreach (Touch touch in Input.touches) {
			bool		bForgetButton = false;
			string		buttonEnterExitMsg;
			string		buttonMsg;
			int			fingerID = touch.fingerId;
			
			TouchNum++;
			
			if (fingerID >= m_currentlyTouchedGO.Length) {	//	check bounds. ignore input beyond 7, send out an error message and continue as normal.
				Rlplog.Error("TouchListener.TouchTapSelect", "Does not support device with > " + m_currentlyTouchedGO.Length + " finger touches.");
				Rlplog.Error("TouchListener.TouchTapSelect", "Total Touches detected: " + Input.touches.Length);
				Rlplog.Error("TouchListener.TouchTapSelect", "Current fingerID: " + fingerID);
				continue;
			}
			
			Ray ray = cam.ScreenPointToRay(touch.position);
			RaycastHit hit;
			hitGO = null;
			
			if (Physics.Raycast(ray, out hit)) {
				hitGO = hit.transform.gameObject;	//	if we touched something
			}
			buttonMsg = null;
			buttonEnterExitMsg = null;
			switch (touch.phase)
			{
				default:
					break;
				case TouchPhase.Began:
					buttonMsg = "OnMouseDown";
					if (hitGO != null) {
						buttonEnterExitMsg = "OnMouseEnter";
						m_currentlyTouchedGO[fingerID] = hitGO;
					}
					break;
				case TouchPhase.Canceled:
				case TouchPhase.Ended:
					buttonMsg = "OnMouseUp";
					if (m_currentlyTouchedGO[fingerID] != hitGO) {
						buttonEnterExitMsg = "OnMouseExit";
					}
					bForgetButton = true;
					break;
			}
			
			if (m_currentlyTouchedGO[fingerID] != null) {
				if (buttonEnterExitMsg != null) {
					m_currentlyTouchedGO[fingerID].SendMessage(buttonEnterExitMsg);
				}
				if (buttonMsg != null) {
					m_currentlyTouchedGO[fingerID].SendMessage(buttonMsg);
				}
				if (bForgetButton) {
					m_currentlyTouchedGO[fingerID] = null;
				}
			}
		}
	}
	
	void Update()
	{
		TouchTapSelect();
	}
}