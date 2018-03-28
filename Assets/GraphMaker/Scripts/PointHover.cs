using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointHover : EventTrigger {

	private GraphMaker graphMaker;

	public override void OnPointerEnter(PointerEventData data)
	{
		Debug.Log("Hovering");

		if(graphMaker == null)
			graphMaker = FindObjectOfType<GraphMaker>();

		graphMaker.DisplayPointInfo(this.gameObject);
	}

	public override void OnPointerExit(PointerEventData data)
	{
		if(graphMaker == null)
			graphMaker = FindObjectOfType<GraphMaker>();

		graphMaker.TurnOffInfoDisplay();
	}
}
