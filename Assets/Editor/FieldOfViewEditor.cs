﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor (typeof (GuardFieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI(){
		GuardFieldOfView fov = (GuardFieldOfView)target;
		Handles.color = Color.white;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
		Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
		Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

		Handles.color = Color.red;
		foreach(Transform visibleTarget in fov.visibleTargets){
			Handles.DrawLine(fov.transform.position, visibleTarget.position);
		}
	}

}
