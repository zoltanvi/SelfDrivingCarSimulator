using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateFrame : MonoBehaviour
{


	public Font Font;



	private GameObject[] rayHolders;
	private GameObject[] textHolders;
	private RectTransform rect;
	private float height;
	private float width;
	private float vertTickD;
	private float horTickD;


	// Use this for initialization
	void Start()
	{
		rayHolders = new GameObject[GraphCreator.TickCount * 2];
		textHolders = new GameObject[GraphCreator.TickCount * 2];
		rect = gameObject.GetComponent<RectTransform>();
		height = rect.rect.height;
		width = rect.rect.width;

		vertTickD = height / GraphCreator.TickCount;
		horTickD = width / GraphCreator.TickCount;


		Material lineMat = new Material(Shader.Find("Sprites/Default"));

		// Creates vertical ticks
		for (int i = 0; i < GraphCreator.TickCount; i++)
		{
			rayHolders[i] = new GameObject();
			textHolders[i] = new GameObject();
			rayHolders[i].transform.SetParent(this.transform);
			textHolders[i].transform.SetParent(this.transform);

			rayHolders[i].AddComponent<LineRenderer>();

			Text text = textHolders[i].AddComponent<Text>();
			text.text = i.ToString();
			text.color = new Color(1f, 1f, 1f, 1f);
			text.font = Font;
			text.alignment = TextAnchor.MiddleRight;
			text.fontSize = 20;

			LineRenderer tmp = rayHolders[i].GetComponent<LineRenderer>();

			tmp.positionCount = 2;
			tmp.startWidth = 2.0f;
			tmp.endWidth = 2.0f;
			tmp.useWorldSpace = false;
			tmp.material = lineMat;
			tmp.startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			tmp.endColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			Vector3 lineBegin = new Vector3(-10, (i * vertTickD), 0) + this.transform.position - new Vector3(0, height - vertTickD);
			Vector3 lineEnd = new Vector3(2, (i * vertTickD), 0) + this.transform.position - new Vector3(0, height - vertTickD);

			text.rectTransform.position = lineBegin - new Vector3(-50, 0);

			tmp.SetPosition(0, lineBegin);
			tmp.SetPosition(1, lineEnd);
		}

		// Creates horizontal ticks
		for (int i = GraphCreator.TickCount; i < GraphCreator.TickCount * 2; i++)
		{
			rayHolders[i] = new GameObject();
			rayHolders[i].transform.SetParent(this.transform);

			rayHolders[i].AddComponent<LineRenderer>();
			LineRenderer tmp = rayHolders[i].GetComponent<LineRenderer>();

			tmp.positionCount = 2;
			tmp.startWidth = 2.0f;
			tmp.endWidth = 2.0f;
			tmp.useWorldSpace = false;
			tmp.material = lineMat;
			tmp.startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			tmp.endColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			Vector3 lineBegin = new Vector3((i - GraphCreator.TickCount) * horTickD, 0) + this.transform.position - new Vector3(0, height);
			Vector3 lineEnd = new Vector3((i - GraphCreator.TickCount) * horTickD, -10) + this.transform.position - new Vector3(0, height);

			tmp.SetPosition(0, lineBegin);
			tmp.SetPosition(1, lineEnd);
		}


	}

	// Update is called once per frame
	void Update()
	{
		//for (int i = 0; i < GraphCreator.TickCount; i++)
		//{
		//	textHolders[i].GetComponent<Text>().rectTransform.position = rayHolders[i].GetComponent<LineRenderer>().GetPosition(0);

		//}
	}
}
