using UnityEngine;
using UnityEngine.UI;

public class WallSensor : MonoBehaviour
{

	[SerializeField] private Transform rayOriginPoint;
	[SerializeField] private float lineDistance = 5f;
	[SerializeField] private int rayQuantity = 2;
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private Text sensorText;
	private string rawSensorText = "";
	GameObject[] rayHolders;


	// A raycastHit-ben vannak a sugarak adatai tarolva.
	private RaycastHit[] raycastHit;

	void Start()
	{
		raycastHit = new RaycastHit[rayQuantity + 1];
		rayHolders = new GameObject[rayQuantity + 1];

        InitializeLines();
    
    }

	void FixedUpdate()
	{
		CreateRays(rayQuantity);

		rawSensorText = "";
		for (int i = 0; i < raycastHit.Length; i++)
		{
			rawSensorText += (i + 1) + ". sensor: " + string.Format("{0:0.0000}", raycastHit[i].distance) + "\n";
		}
		sensorText.text = rawSensorText;
       //  Debug.Log(rayOriginPoint.parent.parent.name);
    }

    // Mindket oldalra (quantity - 1) db sugarat fog vetni, + kozepre egyet.
    void CreateRays(int quantity)
	{
      

        // Az angleBase = a sugarak kozotti szog nagysaga.
        float angleBase = 90f / quantity;
		for (int i = 1, j = (quantity - 1); i < (quantity); i++, j--)
		{
			// A szogek jobb es bal oldalra novekednek / csokkennek, angleBase-enkent.
			Quaternion rightRot = Quaternion.AngleAxis(angleBase * i, rayOriginPoint.up);
			Quaternion leftRot = Quaternion.AngleAxis(-angleBase * i, rayOriginPoint.up);

			
			//Debug.DrawRay(rayOriginPoint.position, rightRot * rayOriginPoint.forward * lineDistance, Color.red);
			//Debug.DrawRay(rayOriginPoint.position, leftRot * rayOriginPoint.forward * lineDistance, Color.red);

			// Letrehozza a bal es jobb oldali sugarakat.
			Physics.Raycast(rayOriginPoint.position, rightRot * rayOriginPoint.forward, out raycastHit[i - 1], lineDistance, LayerMask.GetMask(rayLayerName));
			Physics.Raycast(rayOriginPoint.position, leftRot * rayOriginPoint.forward, out raycastHit[j], lineDistance, LayerMask.GetMask(rayLayerName));

            // Megrajzolja a jobb oldali sugarakat.
            rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOriginPoint.position);
			rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOriginPoint.position + rightRot * rayOriginPoint.forward * lineDistance);
            // Megrajzolja a bal oldali sugarakat.
            rayHolders[j].GetComponent<LineRenderer>().SetPosition(0, rayOriginPoint.position);
			rayHolders[j].GetComponent<LineRenderer>().SetPosition(1, rayOriginPoint.position + leftRot * rayOriginPoint.forward * lineDistance);

        }

		//Debug.DrawRay(rayOriginPoint.position, rayOriginPoint.forward * lineDistance, Color.red);
        
        // Letrehozza a kozepso sugarat.
		Physics.Raycast(rayOriginPoint.position, rayOriginPoint.forward, out raycastHit[quantity], lineDistance, LayerMask.GetMask(rayLayerName));

        // Megrajzolja a kozepso sugarat.
        rayHolders[quantity].GetComponent<LineRenderer>().SetPosition(0, rayOriginPoint.position);
		rayHolders[quantity].GetComponent<LineRenderer>().SetPosition(1, rayOriginPoint.position + rayOriginPoint.forward * lineDistance);
    }

    private void InitializeLines()
    {
        Material lineMat = new Material(Shader.Find("Sprites/Default"));

        for (int i = 0; i < rayHolders.Length; i++)
        {
            rayHolders[i] = new GameObject();
            rayHolders[i].AddComponent<LineRenderer>();
            rayHolders[i].GetComponent<LineRenderer>().positionCount = 2;
            rayHolders[i].GetComponent<LineRenderer>().numCapVertices = 5;
            rayHolders[i].GetComponent<LineRenderer>().startWidth = 0.05f;
            rayHolders[i].GetComponent<LineRenderer>().endWidth = 0.05f;
            rayHolders[i].GetComponent<LineRenderer>().useWorldSpace = false;
            rayHolders[i].GetComponent<LineRenderer>().material = lineMat;
            rayHolders[i].GetComponent<LineRenderer>().startColor = Color.green;
            rayHolders[i].GetComponent<LineRenderer>().endColor = Color.green;

        }
    }


}
