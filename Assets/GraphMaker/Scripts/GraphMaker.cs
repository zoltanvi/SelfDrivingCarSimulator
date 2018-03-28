using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphMaker : MonoBehaviour
{

	[Header("Graph Type")]
	public GraphType graphType = GraphType.line;

	//Parent size variables
	private RectTransform rectTransform;
	private Vector2 graphSizeDelta;
	private float width;
	private float height;

	[Header("Data Points")]
	[SerializeField]
	private GameObject pointPrefab;
	[SerializeField]
	private Color pointColor = new Color(1f, 1f, 1f, 1f);
	[Range(.0001f, 0.05f)]
	[Tooltip("Size as a percent of height")]
	[SerializeField]
	private float pointSize = 0.005f;
	private Queue<PointData> activePoints = new Queue<PointData>();
	private Queue<PointData> pointPool = new Queue<PointData>();

	[Header("Info Display")]
	[SerializeField]
	private GameObject pointInfoPrefab;
	private GameObject pointInfoInstance;
	private Text pointInfoText;
	[SerializeField]
	[Range(0, 4)]
	private int roundToDigits = 2;
	[SerializeField]
	[Range(10, 50)]
	private int infoFontSize = 30;
	[SerializeField]
	private Font infoFont;
	[SerializeField]
	private Color infoColor = new Color(1f, 1f, 1f, 1f);

	[Header("Axes")]
	[SerializeField]
	private GameObject axisPrefab;
	[Tooltip("Thickness in Pixels")]
	[Range(0.25f, 5f)]
	[SerializeField]
	private float axisThickness = 1f;
	[Range(.01f, 0.25f)]
	[Tooltip("Padding as a percent of height")]
	[SerializeField]
	private float axisVerticalPadding = 0.05f;
	[Range(.01f, 0.25f)]
	[Tooltip("Padding as a percent of width")]
	[SerializeField]
	private float axisHorizontalPadding = 0.05f;
	[Range(0f, 0.25f)]
	[Tooltip("Offset as a percent of height")]
	[SerializeField]
	private float verticalOffSet = 0f;
	[Range(0f, 0.25f)]
	[Tooltip("Offset as a percent of width")]
	[SerializeField]
	private float horizontalOffSet = 0f;
	[SerializeField]
	private Color axesColor = new Color(1f, 1f, 1f, 0.75f);
	private Image vertAxisImage;
	private Image horzAxisImage;

	[Header("Axes Labels")]
	[SerializeField]
	private GameObject axisLabelPrefab;
	private RectTransform horzLabel;
	private RectTransform vertLabel;
	[SerializeField]
	[Range(10, 150)]
	private int axesLabelFontSize = 50;
	[SerializeField]
	private Font axesLabelFont;
	[SerializeField]
	private Color axesLabelColor = new Color(1f, 1f, 1f, 1f);
	[SerializeField]
	private string horizontalLabel;
	private Text horizontalLabelText;
	[SerializeField]
	private string verticalLabel;
	private Text verticalLabelText;

	[Header("Line Graph Connectors")]
	[SerializeField]
	private GameObject connectorPrefab;
	[SerializeField]
	private bool connectPoints = true;
	[Range(1f, 10f)]
	[SerializeField]
	private float lineWidth = 2f;
	[SerializeField]
	private Color lineColor = new Color(1f, 1f, 1f, 1f);
	private Queue<RectTransform> activeConnectors = new Queue<RectTransform>();
	private Queue<RectTransform> connectorPool = new Queue<RectTransform>();
	private List<Vector3> pointLocations = new List<Vector3>();

	[Header("Scale Settings")]
	[SerializeField]
	private GameObject tickMarkPrefab;
	[SerializeField]
	private Color tickColor = new Color(1f, 1f, 1f, 1f);
	[Range(1f, 20f)]
	[Tooltip("Size in pixels")]
	[SerializeField]
	private float tickMarkSizePixels = 10f;
	[Range(1, 4)]
	[SerializeField]
	[Tooltip("Ratio of length to width")]
	private float tickMarkAspectRatio = 2f;
	private Queue<TickMarkData> activeTickMarks = new Queue<TickMarkData>();
	private Queue<TickMarkData> tickMarkPool = new Queue<TickMarkData>();
	[SerializeField]
	[Tooltip("Enables auto scaling - will ignore other values")]
	private bool autoScale = true;
	[SerializeField]
	private float verticalTickScale = 1f;
	[SerializeField]
	private bool hideHorzTickMarks = false;
	[SerializeField]
	private float horizontalTickScale = 1f;
	private float minVertTickValue = 0f;
	private float minHorzTickValue = 0f;
	private int numVertTicks = 0;
	private int numHorzTicks = 0;

	[Header("Tick Mark Text")]
	[SerializeField]
	[Range(0.01f, 1f)]
	private float fontSize = 0.5f;
	[SerializeField]
	private Font textFont;
	[SerializeField]
	private Color textColor = new Color(1f, 1f, 1f, 1f);
	[SerializeField]
	[Range(0, 40)]
	private int textOffSet = 25;

	[Header("Chart Background")]
	[SerializeField]
	private bool useBackground = true;
	[SerializeField]
	private GameObject backgroundPrefab;
	private RectTransform backgroundInstance;
	[SerializeField]
	[Range(-0.25f, 0.25f)]
	private float backgroundPadding = 0.1f;
	[SerializeField]
	private Color backgroundColor = new Color(1f, 1f, 1f, 1f);
	private Image backgroundImage;

	//Axes Size and Scale Variables
	private GameObject vertAxis;
	private RectTransform vertAxisRect;
	private float vertAxisLength;
	private float vertAxisScale;
	private GameObject horzAxis;
	private RectTransform horzAxisRect;
	private float horzAxisLength;
	private float horzAxisScale;

	//Internal Data Structures
	[SerializeField]
	[Header("Data")]
	private List<Vector2> dataPoints = new List<Vector2>();
	private float maxVertValue = -Mathf.Infinity;
	private float minVertValue = Mathf.Infinity;
	private float maxHorzValue = -Mathf.Infinity;
	private float minHorzValue = Mathf.Infinity;

	public bool InsertRandom = false;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		graphSizeDelta = rectTransform.sizeDelta;
	}

	// Use this for initialization
	void Start()
	{
		CleanUpChildren();
		ClearLists();
		RedrawGraph();
	}

	/// <summary>
	/// Call this to redraw the graph. Simple is better.
	/// </summary>
	public void RedrawGraph()
	{
		if (Application.isEditor) // cleans up children during editing
			CleanUpChildren();

		if (rectTransform == null)
			rectTransform = GetComponent<RectTransform>();

		if (InsertRandom)
		{
			Vector2 randomData = new Vector2(Random.Range(0f, 5f), Random.Range(-10f, 10f));
			AddDataPointInOrder(randomData);
			InsertRandom = false;
		}

		GetGraphSize();
		DataMinMax();
		if (autoScale)
		{
			AutoScale();
		}
		else
		{
			AutoScaleVert();
		}

		GetAxesScale();
		PlaceAxesLabels();
		CorrectNumberOfDataPoints();
		DrawAxes();
		PlaceTickMarks();
		PlaceBackground();

		if (graphType == GraphType.line)
		{
			PlacePoints();
			ToggleConnectors(connectPoints);
			if (connectPoints)
				PlaceConnectors();

		}
		else if (graphType == GraphType.bar)
		{
			PlaceBars();
			ToggleConnectors(false);
		}

		CorrectOrdering();

	}

	void ClearLists()
	{
		pointLocations.Clear();
		activePoints.Clear();
		activeTickMarks.Clear();
		activeConnectors.Clear();
	}

	void CorrectOrdering()
	{
		vertAxis.transform.SetAsLastSibling();
		horzAxis.transform.SetAsLastSibling();

		foreach (TickMarkData tick in activeTickMarks)
		{
			if (tick.rectTransform != null)
				tick.rectTransform.SetAsLastSibling();
		}

		foreach (RectTransform connector in activeConnectors)
		{
			if (connector != null)
				connector.SetAsLastSibling();
		}

		foreach (PointData point in activePoints)
		{
			if (point.rectTransform != null)
				point.rectTransform.SetAsLastSibling();
		}
	}

	void PlaceBackground()
	{
		if (backgroundPrefab == null)
		{
			Debug.LogError("No Background Prefab - Can't Place Background");
			return;
		}

		if (useBackground)
		{
			if (backgroundInstance != null)
				backgroundInstance.gameObject.SetActive(true);
			else
				backgroundInstance = Instantiate(backgroundPrefab).GetComponent<RectTransform>();

			backgroundInstance.SetParent(this.transform);
			backgroundInstance.name = "Chart Background";

			backgroundInstance.anchorMax = new Vector2(1 + horizontalOffSet - axisHorizontalPadding + backgroundPadding, 1 + verticalOffSet - axisVerticalPadding + backgroundPadding);
			backgroundInstance.anchorMin = new Vector2(horizontalOffSet + axisHorizontalPadding - backgroundPadding, verticalOffSet + axisVerticalPadding - backgroundPadding);
			backgroundInstance.pivot = new Vector2(0.5f, 0.5f);
			backgroundInstance.anchoredPosition = new Vector2(0.5f, 0f);
			backgroundInstance.sizeDelta = new Vector2(1, 1);

			if (backgroundImage == null)
				backgroundImage = backgroundInstance.GetComponent<Image>();
			backgroundImage.color = backgroundColor;


		}
		else
		{
			if (backgroundInstance != null)
				backgroundInstance.gameObject.SetActive(false);
		}
	}

	void PlaceAxesLabels()
	{
		if (axisPrefab == null)
		{
			Debug.LogError("No Axis Label Prefab - Can't Place Axis Labels");
			return;
		}

		if (horizontalLabel != "")
		{
			if (horzLabel == null)
				horzLabel = Instantiate(axisLabelPrefab).GetComponent<RectTransform>();
			if (horizontalLabelText == null)
				horizontalLabelText = horzLabel.gameObject.GetComponentInChildren<Text>();

			horzLabel.SetParent(this.transform);
			horzLabel.name = "Horizontal Axis Label";

			horzLabel.anchorMax = new Vector2(0.5f + horizontalOffSet, 0.01f);
			horzLabel.anchorMin = new Vector2(0.5f + horizontalOffSet, 0.01f);
			horzLabel.pivot = new Vector2(0.5f, 0f);
			horzLabel.anchoredPosition = new Vector2(0.5f, 0f);
			horzLabel.sizeDelta = new Vector2(1, 1);

			horizontalLabelText.text = horizontalLabel;
			horizontalLabelText.fontSize = axesLabelFontSize;
			if (axesLabelFont != null)
				horizontalLabelText.font = axesLabelFont;
			horizontalLabelText.color = axesLabelColor;
		}

		if (verticalLabel != "")
		{
			if (vertLabel == null)
				vertLabel = Instantiate(axisLabelPrefab).GetComponent<RectTransform>();
			if (verticalLabelText == null)
				verticalLabelText = vertLabel.gameObject.GetComponentInChildren<Text>();

			vertLabel.SetParent(this.transform);
			vertLabel.name = "Vertical Axes Lable";

			vertLabel.anchorMax = new Vector2(0.01f, 0.5f + verticalOffSet);
			vertLabel.anchorMin = new Vector2(0.01f, 0.5f + verticalOffSet);
			vertLabel.pivot = new Vector2(0.5f, 1f);
			vertLabel.anchoredPosition = new Vector2(0f, 0.5f);
			vertLabel.sizeDelta = new Vector2(1, 1);
			vertLabel.rotation = Quaternion.Euler(0f, 0f, 90f);

			verticalLabelText.text = verticalLabel;
			verticalLabelText.fontSize = axesLabelFontSize;
			if (axesLabelFont != null)
				verticalLabelText.font = axesLabelFont;
			verticalLabelText.color = axesLabelColor;
		}
	}

	void ToggleConnectors(bool isActive)
	{
		foreach (RectTransform connector in activeConnectors)
		{
			if (connector != null)
				connector.gameObject.SetActive(isActive);
		}
	}

	void PlaceConnectors()
	{
		if (connectorPrefab == null)
		{
			Debug.LogError("Missing Connector Prefab - Can't Connect Points");
			return;
		}
		if (dataPoints.Count >= 2)
			CorrectNumberOfConnectors(dataPoints.Count - 1); //fence post problem
		else
			CorrectNumberOfConnectors(0);

		RectTransform tempConnector = null;

		for (int i = 0; i < dataPoints.Count - 1; i++)
		{
			if (activeConnectors.Count > 0)
				tempConnector = activeConnectors.Dequeue();
			if (tempConnector == null)
				tempConnector = GetConnectorFromPool();

			if (pointLocations.Count > i + 1)
				DrawLine(pointLocations[i], pointLocations[i + 1], tempConnector);
			tempConnector.GetComponent<Image>().color = lineColor;
			activeConnectors.Enqueue(tempConnector);

		}
	}

	void PlaceTickMarks()
	{
		if (!hideHorzTickMarks)
			CorrectNumberOfTickMarks(numVertTicks + numHorzTicks); // need to calculate horizontal number too
		else
			CorrectNumberOfTickMarks(numVertTicks);

		float tickMarkWidth; //Translate inspector pixel value to screen ratio
		float tickMarkHeight; //Translate inspector pixel value to screen ratio

		//vertical axis
		for (int i = 0; i < numVertTicks; i++)
		{
			TickMarkData tickMark = activeTickMarks.Dequeue();
			if (tickMark.rectTransform == null)
				tickMark = GetTickMarkFromPool(); //occasionally tick marks are destroyed or references lost due to compiling or user/editor

			tickMarkWidth = tickMarkSizePixels * tickMarkAspectRatio / width;
			tickMarkHeight = tickMarkSizePixels / height;//

			tickMark.rectTransform.gameObject.SetActive(true);
			tickMark.position = new Vector2(axisHorizontalPadding + horizontalOffSet, (i * verticalTickScale) * vertAxisScale + verticalOffSet);
			tickMark.rectTransform.anchorMax = tickMark.position + new Vector2(tickMarkWidth, tickMarkHeight) + new Vector2(0, axisVerticalPadding);
			tickMark.rectTransform.anchorMin = tickMark.position - new Vector2(tickMarkWidth, tickMarkHeight) + new Vector2(0, axisVerticalPadding);

			tickMark.rectTransform.sizeDelta = new Vector2(1, 1);
			tickMark.rectTransform.anchoredPosition = Vector2.zero;
			tickMark.image.color = tickColor;

			tickMark.text.text = (i * verticalTickScale + minVertTickValue).ToString();
			if (textFont != null)
				tickMark.text.font = textFont;
			tickMark.text.color = textColor;
			tickMark.text.gameObject.transform.localScale = Vector3.one * fontSize;
			tickMark.textTransform.localPosition = new Vector3(-textOffSet, 0f, 0f);
			tickMark.text.alignment = TextAnchor.MiddleRight;

			activeTickMarks.Enqueue(tickMark);//return to active queue
		}

		if (hideHorzTickMarks)
			return;

		//Horizontal axis
		for (int i = 1; i < numHorzTicks + 1; i++)
		{
			TickMarkData tickMark = activeTickMarks.Dequeue();
			if (tickMark.rectTransform == null)
				tickMark = GetTickMarkFromPool(); //occasionally tick marks are destroyed or references lost due to compiling or user/editor

			tickMarkWidth = tickMarkSizePixels / width;
			tickMarkHeight = tickMarkSizePixels * tickMarkAspectRatio / height;

			tickMark.rectTransform.gameObject.SetActive(true);
			tickMark.position = new Vector2((i * horizontalTickScale) * horzAxisScale + horizontalOffSet, VerticalAxisPlacement() + verticalOffSet);
			tickMark.rectTransform.anchorMax = tickMark.position + new Vector2(tickMarkWidth, tickMarkHeight) + new Vector2(axisHorizontalPadding, 0);
			tickMark.rectTransform.anchorMin = tickMark.position - new Vector2(tickMarkWidth, tickMarkHeight) + new Vector2(axisHorizontalPadding, 0);
			tickMark.rectTransform.sizeDelta = new Vector2(1, 1);
			tickMark.rectTransform.anchoredPosition = Vector2.zero;
			tickMark.image.color = tickColor;

			tickMark.text.text = (i * horizontalTickScale + minHorzTickValue).ToString();
			if (textFont != null)
				tickMark.text.font = textFont;
			tickMark.text.color = textColor;
			//resize
			tickMark.textTransform.pivot = new Vector2(0.5f, 1f);
			tickMark.textTransform.localScale = Vector3.one * fontSize;
			tickMark.textTransform.localPosition = new Vector3(0, -textOffSet, 0f);
			tickMark.text.alignment = TextAnchor.UpperCenter;

			activeTickMarks.Enqueue(tickMark);//return to active queue
		}

	}


	void GetGraphSize()
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		width = corners[1].x - corners[2].x;
		height = corners[0].y - corners[1].y;
		width = Mathf.Abs(width);
		height = Mathf.Abs(height);

		graphSizeDelta = rectTransform.sizeDelta;
	}

	void DrawAxes()
	{
		CreateAxes();

		//draw vertical axis
		if (vertAxisRect == null)
			vertAxisRect = vertAxis.GetComponent<RectTransform>();


		vertAxisRect.anchorMax = new Vector2(horizontalOffSet, verticalOffSet) + new Vector2(axisHorizontalPadding + axisThickness / graphSizeDelta.x * 0.5f, 1 - axisVerticalPadding);
		vertAxisRect.anchorMin = new Vector2(horizontalOffSet, verticalOffSet) + new Vector2(axisHorizontalPadding - axisThickness / graphSizeDelta.x * 0.5f, axisVerticalPadding);
		vertAxisRect.sizeDelta = new Vector2(1, 1);
		vertAxisRect.anchoredPosition = new Vector2(0f, 0f);

		//draw horizontal axis
		if (horzAxisRect == null)
			horzAxisRect = horzAxis.GetComponent<RectTransform>();

		horzAxisRect.anchorMax = new Vector2(horizontalOffSet, verticalOffSet) + new Vector2(1 - axisHorizontalPadding, VerticalAxisPlacement() + axisThickness / graphSizeDelta.y * 0.5f);
		horzAxisRect.anchorMin = new Vector2(horizontalOffSet, verticalOffSet) + new Vector2(axisHorizontalPadding - axisThickness / graphSizeDelta.x * 0.5f, VerticalAxisPlacement() - axisThickness / graphSizeDelta.y * 0.5f);
		horzAxisRect.sizeDelta = new Vector2(1, 1);
		horzAxisRect.anchoredPosition = new Vector2(0, 0);

		if (vertAxisImage == null)
			vertAxisImage = vertAxisRect.GetComponent<Image>();
		if (horzAxisImage == null)
			horzAxisImage = horzAxisRect.GetComponent<Image>();

		vertAxisImage.color = axesColor;
		horzAxisImage.color = axesColor;
	}

	void PlacePoints()
	{
		pointLocations.Clear();

		for (int i = 0; i < dataPoints.Count; i++)
		{
			PointData pd = activePoints.Dequeue();

			if (pd.rectTransform == null) //occasionally points are deleted during recompiling
				pd = GetPointFromPool();

			pd.position = new Vector2(dataPoints[i].x * horzAxisScale + axisHorizontalPadding + horizontalOffSet, dataPoints[i].y * vertAxisScale + verticalOffSet);
			pd.rectTransform.anchorMax = pd.position + new Vector2(pointSize, pointSize) + new Vector2(0, VerticalAxisPlacement());
			pd.rectTransform.anchorMin = pd.position - new Vector2(pointSize, pointSize) + new Vector2(0, VerticalAxisPlacement());
			pd.rectTransform.sizeDelta = new Vector2(1, 1);
			pd.rectTransform.anchoredPosition = Vector2.zero;
			pd.image.color = pointColor;
			pd.image.preserveAspect = true;
			pd.dataValue = dataPoints[i];

			activePoints.Enqueue(pd); //return to queue

			//this is an error correction. Somehow position is not getting set at first redraw.
			if (!float.IsNaN(pd.rectTransform.position.x))
				pointLocations.Add(pd.rectTransform.position); //used later by the connectors
															   //			else
															   //			{
															   //				ClearLists();
															   //				RedrawGraph(); 
															   //			}
		}
	}

	void PlaceBars()
	{
		for (int i = 0; i < dataPoints.Count; i++)
		{
			PointData pd = activePoints.Dequeue();

			if (pd.rectTransform == null) //occasionally points are deleted during recompiling
				pd = GetPointFromPool();

			if (dataPoints[i].y >= 0)
			{
				pd.position = new Vector2(dataPoints[i].x * horzAxisScale + axisHorizontalPadding + horizontalOffSet, dataPoints[i].y * vertAxisScale / 2f + verticalOffSet);
				pd.rectTransform.anchorMax = pd.position + new Vector2(pointSize, dataPoints[i].y * vertAxisScale / 2) + new Vector2(0, VerticalAxisPlacement());
				pd.rectTransform.anchorMin = pd.position - new Vector2(pointSize, dataPoints[i].y * vertAxisScale / 2) + new Vector2(0, VerticalAxisPlacement());
			}
			else
			{
				pd.position = new Vector2(dataPoints[i].x * horzAxisScale + axisHorizontalPadding + horizontalOffSet, dataPoints[i].y * vertAxisScale / 2f + verticalOffSet);
				pd.rectTransform.anchorMax = pd.position + new Vector2(pointSize, -dataPoints[i].y * vertAxisScale / 2) + new Vector2(0, VerticalAxisPlacement());
				pd.rectTransform.anchorMin = pd.position - new Vector2(pointSize, -dataPoints[i].y * vertAxisScale / 2) + new Vector2(0, VerticalAxisPlacement());
			}

			pd.rectTransform.sizeDelta = new Vector2(1, 1);
			pd.rectTransform.anchoredPosition = Vector2.zero;
			pd.image.color = pointColor;
			pd.image.preserveAspect = false;
			pd.rectTransform.SetAsLastSibling();

			pd.pointGO = pd.rectTransform.gameObject;
			pd.dataValue = dataPoints[i];

			activePoints.Enqueue(pd); //return to queue
		}
	}

	void GetAxesScale()
	{
		vertAxisLength = 1f - 2f * axisVerticalPadding;
		horzAxisLength = 1f - 2f * axisHorizontalPadding;

		float vertAxisDist = 0f;// = Mathf.CeilToInt((maxVertValue - minVertValue) / verticalTickScale) * verticalTickScale;
		float horztAxisDist = 0f;

		//set up vert scale and tick mark values
		if (maxVertValue > 0)
			vertAxisDist += Mathf.CeilToInt(maxVertValue / verticalTickScale) * verticalTickScale;
		if (minVertValue < 0)
		{
			vertAxisDist += Mathf.CeilToInt(Mathf.Abs(minVertValue) / verticalTickScale) * verticalTickScale;
			minVertTickValue = -Mathf.CeilToInt(Mathf.Abs(minVertValue) / verticalTickScale) * verticalTickScale;
		}
		else
			minVertTickValue = 0f;

		numVertTicks = Mathf.CeilToInt(vertAxisDist / verticalTickScale) + 1;

		//setup horz scale and tick mark values
		if (maxHorzValue > 0)
			horztAxisDist += Mathf.CeilToInt(maxHorzValue / horizontalTickScale) * horizontalTickScale;
		if (minHorzValue < 0)
		{
			horztAxisDist += Mathf.CeilToInt(Mathf.Abs(minHorzValue) / horizontalTickScale) * horizontalTickScale;
			minHorzTickValue = -Mathf.CeilToInt(Mathf.Abs(minHorzValue) / horizontalTickScale) * horizontalTickScale;
		}
		else
			minHorzTickValue = 0f;

		numHorzTicks = Mathf.CeilToInt(horztAxisDist / horizontalTickScale);

		vertAxisScale = vertAxisLength / vertAxisDist;
		horzAxisScale = horzAxisLength / horztAxisDist;
	}

	//Calculate the position of the vertical axis
	float VerticalAxisPlacement()
	{
		if (minVertValue >= 0)
			return axisVerticalPadding;
		else if (maxVertValue <= 0)
			return 1 - axisVerticalPadding;
		else
		{
			float difference = (numVertTicks - 1) * verticalTickScale;
			float ratio = Mathf.Abs(minVertTickValue) / difference;
			return ratio * (1 - 2 * axisVerticalPadding) + axisVerticalPadding;
		}
	}

	//Gets min and max values from data set
	void DataMinMax()
	{
		maxVertValue = -Mathf.Infinity;
		minVertValue = Mathf.Infinity;
		maxHorzValue = -Mathf.Infinity;
		minHorzValue = Mathf.Infinity;

		foreach (Vector2 data in dataPoints)
		{
			if (data.y > maxVertValue)
				maxVertValue = data.y;
			if (data.y < minVertValue)
				minVertValue = data.y;
			if (data.x > maxHorzValue)
				maxHorzValue = data.x;
			if (data.x < minHorzValue)
				minHorzValue = data.x;
		}

		if (dataPoints.Count == 0)
		{
			maxVertValue = 1f;
			minVertValue = -1f;
			maxHorzValue = 1f;
			minHorzValue = -1f;
		}
	}

	//Instantiates the axes
	void CreateAxes()
	{
		if (axisPrefab != null)
		{
			if (vertAxis == null)
			{
				vertAxis = (GameObject)Instantiate(axisPrefab);
				vertAxis.transform.SetParent(this.transform);
				vertAxis.name = "Vertical Axis";
			}

			if (horzAxis == null)
			{
				horzAxis = (GameObject)Instantiate(axisPrefab);
				horzAxis.transform.SetParent(this.transform);
				horzAxis.name = "Horizontal Axis";
			}
		}
	}

	/// <summary>
	/// Recursive function to match number of active data points with number of data points
	/// </summary>
	void CorrectNumberOfDataPoints()
	{
		if (dataPoints.Count < activePoints.Count)
		{
			ReturnPointToPool(activePoints.Dequeue());
			CorrectNumberOfDataPoints();
		}
		else if (dataPoints.Count > activePoints.Count)
		{
			activePoints.Enqueue(GetPointFromPool());
			CorrectNumberOfDataPoints();
		}
		else
			return;
	}

	/// <summary>
	/// Recursive function to match number of active tick marks with number of tick marks
	/// </summary>
	void CorrectNumberOfTickMarks(int number)
	{
		if (number <= 0)
			return;

		if (number < activeTickMarks.Count)
		{
			if (activeTickMarks.Peek() != null && activeTickMarks.Count > 0)
				ReturnTickMarkToPool(activeTickMarks.Dequeue());
			CorrectNumberOfTickMarks(number);
		}
		else if (number > activeTickMarks.Count)
		{
			activeTickMarks.Enqueue(GetTickMarkFromPool());
			CorrectNumberOfTickMarks(number);
		}
		else
			return;
	}

	/// <summary>
	/// Recursive function to match number of active point connectors with number of tick marks
	/// </summary>
	void CorrectNumberOfConnectors(int number)
	{
		if (number <= 0)
			return;

		if (number < activeConnectors.Count)
		{
			if (activeConnectors.Peek() != null && activeConnectors.Count > 0)
				ReturnConnectorToPool(activeConnectors.Dequeue());
			else if (activeConnectors.Peek() == null)
				activeConnectors.Dequeue();
			CorrectNumberOfConnectors(number);
		}
		else if (number > activeConnectors.Count)
		{
			activeConnectors.Enqueue(GetConnectorFromPool());
			CorrectNumberOfConnectors(number);
		}
		else
			return;
	}

	/// <summary>
	/// Over writes data set with new Vector2 List allows renaming of axes labels.
	/// </summary>
	/// <param name="dataSet"></param>
	public void NewDataSet(List<Vector2> dataSet, string vertLabel = "", string horzLabel = "")
	{
		horizontalLabel = horzLabel;
		verticalLabel = vertLabel;
		dataPoints = dataSet;
		RedrawGraph();
	}

	/// <summary>
	/// Over writes data set with new Vector2 List
	/// </summary>
	/// <param name="dataSet"></param>
	public void NewDataSet(List<Vector2> dataSet)
	{
		dataPoints = dataSet;
		RedrawGraph();
	}

	/// <summary>
	/// Add a data point to the current graph
	/// </summary>
	/// <param name="data"></param>
	public void AddDataPoint(Vector2 data)
	{
		dataPoints.Add(data);
		RedrawGraph();
	}

	/// <summary>
	/// Adds the data point in order - ensures data is graphed in order
	/// </summary>
	/// <param name="data">Data.</param>
	public void AddDataPointInOrder(Vector2 data)
	{
		int index = dataPoints.Count;
		for (int i = 0; i < dataPoints.Count; i++)
		{
			if (dataPoints[0].x > data.x)
			{
				index = 0;
				break;
			}
			else if (i + 1 < dataPoints.Count)
			{
				if (dataPoints[i].x <= data.x && dataPoints[i + 1].x >= data.x)
				{
					index = i + 1;
					break;
				}
			}
		}

		dataPoints.Insert(index, data);
	}

	/// <summary>
	/// Gets graph point from pool or creates new if none exist
	/// </summary>
	/// <returns></returns>
	RectTransform GetConnectorFromPool()
	{
		if (connectorPool.Count > 0)
		{
			//recurisve loop to clean out queue if points get deleted
			if (connectorPool.Peek() == null)
			{
				connectorPool.Dequeue();
				return GetConnectorFromPool();
			}
			return connectorPool.Dequeue();
		}

		else
		{
			GameObject tempConnector = (GameObject)Instantiate(connectorPrefab);
			tempConnector.transform.SetParent(this.transform);
			return tempConnector.GetComponent<RectTransform>();
		}
	}

	/// <summary>
	/// Returns tick mark to pool
	/// </summary>
	/// <param name="point"></param>
	void ReturnConnectorToPool(RectTransform connector)
	{
		if (connector != null)
		{
			if (connector != null)
			{
				connectorPool.Enqueue(connector);
				connector.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Returns point to pool
	/// </summary>
	/// <param name="point"></param>
	void ReturnPointToPool(PointData point)
	{
		if (point.rectTransform == null)
			return;
		pointPool.Enqueue(point);
		point.pointGO.SetActive(false);
	}

	/// <summary>
	/// Gets graph point from pool or creates new if none exist
	/// </summary>
	/// <returns></returns>
	PointData GetPointFromPool()
	{
		if (pointPool.Count > 0)
		{
			//recurisve loop to clean out queue if points get deleted
			if (pointPool.Peek() == null)
			{
				pointPool.Dequeue();
				return GetPointFromPool();
			}
			return pointPool.Dequeue();
		}

		else
		{
			GameObject tempPoint = (GameObject)Instantiate(pointPrefab);
			tempPoint.transform.SetParent(this.transform);
			PointData tempData = new PointData(tempPoint, tempPoint.GetComponent<RectTransform>(), Vector2.zero, tempPoint.GetComponent<Image>());
			return tempData;
		}
	}

	/// <summary>
	/// Returns tick mark to pool
	/// </summary>
	/// <param name="point"></param>
	void ReturnTickMarkToPool(TickMarkData tickMark)
	{
		if (tickMark != null)
		{
			if (tickMark.rectTransform != null)
			{
				tickMarkPool.Enqueue(tickMark);
				tickMark.rectTransform.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// Gets tick mark from pool or creates new if none exist
	/// </summary>
	/// <returns></returns>
	TickMarkData GetTickMarkFromPool()
	{
		if (tickMarkPool.Count > 0)
		{
			//recurisve loop to clean out queue if points get deleted
			if (tickMarkPool.Peek() == null)
			{
				tickMarkPool.Dequeue();
				return GetTickMarkFromPool();
			}
			else if (tickMarkPool.Peek().rectTransform == null)
			{
				tickMarkPool.Dequeue();
				return GetTickMarkFromPool();
			}
			return tickMarkPool.Dequeue();
		}

		else
		{
			GameObject tempMark = (GameObject)Instantiate(tickMarkPrefab);
			tempMark.transform.SetParent(this.transform);
			TickMarkData tempData = new TickMarkData(tempMark.GetComponent<RectTransform>(), Vector2.zero, tempMark.GetComponentInChildren<Text>(), tempMark.GetComponent<Image>());
			return tempData;
		}
	}

	void CleanUpChildren() //when recompiling children get lost
	{
		foreach (Transform child in this.transform)
		{
			if (child != this.transform)
				DestroyImmediate(child.gameObject);
		}
	}

	void DrawLine(Vector3 pt1, Vector3 pt2, RectTransform connector)
	{
		if (connector == null)
			return;

		Vector3 differenceVector = pt1 - pt2;
		connector.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
		connector.pivot = new Vector2(0, 0.5f);
		connector.position = pt2;//previousNode.transform.position;
		float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
		connector.rotation = Quaternion.Euler(0, 0, angle);
	}

	public void DisplayPointInfo(GameObject point)
	{
		PointData tempData = null;
		foreach (PointData _point in activePoints)
		{
			if (_point.pointGO == point)
				tempData = _point;
		}

		if (tempData != null)
			DisplayPointInfo(tempData.dataValue);
	}

	void DisplayPointInfo(Vector2 data)
	{
		if (pointInfoPrefab == null)
		{
			Debug.LogError("Not Data Info Prefab");
			return;
		}
		else if (pointInfoInstance == null)
		{
			pointInfoInstance = (GameObject)Instantiate(pointInfoPrefab);
			pointInfoInstance.transform.SetParent(this.transform.parent);
		}

		pointInfoInstance.SetActive(true);//ensure GO is on

		if (pointInfoText == null)
			pointInfoText = pointInfoInstance.GetComponentInChildren<Text>();

		pointInfoText.fontSize = infoFontSize;
		if (infoFont != null)
			pointInfoText.font = infoFont;
		pointInfoText.resizeTextForBestFit = false;
		pointInfoText.color = infoColor;

		//round data point;
		float x;
		x = Mathf.RoundToInt(data.x * Mathf.Pow(10, roundToDigits)) / Mathf.Pow(10, roundToDigits);
		float y;
		y = Mathf.RoundToInt(data.y * Mathf.Pow(10, roundToDigits)) / Mathf.Pow(10, roundToDigits);
		pointInfoText.text = "( " + x + " , " + y + " )";

		//Move to mouse cursor
		pointInfoInstance.transform.position = Input.mousePosition + new Vector3(40f, 40f, 0f);
	}

	public void TurnOffInfoDisplay()
	{
		if (pointInfoInstance != null)
			pointInfoInstance.SetActive(false);
	}

	void AutoScale()
	{
		DataMinMax(); //make sure max and min are current
		int maxNumVertTicks = 10;
		//int maxNumHorzTicks = 10;

		float vertRange = 0f;
		if (maxVertValue > 0f)
			vertRange += maxVertValue;
		if (minVertValue < 0f)
			vertRange += Mathf.Abs(minVertValue);

		float horzRange = 0f;
		if (maxHorzValue > 0)
			horzRange += maxHorzValue;
		if (minHorzValue < 0)
			horzRange += Mathf.Abs(minHorzValue);

		if (verticalTickScale >= 1)
			verticalTickScale = Mathf.CeilToInt(vertRange / maxNumVertTicks);
		else
			verticalTickScale = RoundToTwoDigits(verticalTickScale);

		//if (horizontalTickScale >= 1)
		//	horizontalTickScale = Mathf.CeilToInt(vertRange / maxNumHorzTicks);
		//else
		//	horizontalTickScale = RoundToTwoDigits(horizontalTickScale);

	}

	void AutoScaleVert()
	{
		DataMinMax(); //make sure max and min are current
		int maxNumVertTicks = 10;
		//int maxNumHorzTicks = 10;

		float vertRange = 0f;
		if (maxVertValue > 0f)
			vertRange += maxVertValue;
		if (minVertValue < 0f)
			vertRange += Mathf.Abs(minVertValue);

		if (verticalTickScale >= 1)
			verticalTickScale = Mathf.CeilToInt(vertRange / maxNumVertTicks);
		else
			verticalTickScale = RoundToTwoDigits(verticalTickScale);

	}

	float RoundToTwoDigits(float input)
	{
		return Mathf.CeilToInt(input / 100) * 100;
	}

	public void ReorderData()
	{

	}


}

[System.Serializable]
public class PointData
{
	public GameObject pointGO;
	public RectTransform rectTransform;
	public Vector2 position;
	public Image image;
	public Vector2 dataValue;

	public PointData(GameObject pt, RectTransform rectTrans, Vector2 value, Image _image)
	{
		pointGO = pt;
		rectTransform = rectTrans;
		position = value;
		image = _image;
	}
}

[System.Serializable]
public class TickMarkData
{
	public RectTransform rectTransform;
	public Vector2 position;
	public UnityEngine.UI.Text text;
	public RectTransform textTransform;
	public UnityEngine.UI.Image image;

	public TickMarkData(RectTransform rectTrans, Vector2 location, Text _text, Image _image)
	{
		rectTransform = rectTrans;
		position = location;
		text = _text;
		textTransform = _text.gameObject.GetComponent<RectTransform>();
		image = _image;
	}
}

public enum GraphType
{
	line,
	bar
}

