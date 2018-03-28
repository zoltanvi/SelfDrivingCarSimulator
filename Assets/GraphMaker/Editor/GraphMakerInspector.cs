using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphMaker))]
public class GraphMakerInspector : Editor
{

	GraphMaker _script;

	public override void OnInspectorGUI()
	{
		if (_script == null)
			_script = (GraphMaker)target;

		base.OnInspectorGUI();
		//DrawDefaultInspector();

		if (GUI.changed)
		{
			_script.RedrawGraph();
		}

	}

}
