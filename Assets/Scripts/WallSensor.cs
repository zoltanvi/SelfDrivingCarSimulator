using UnityEngine;
using UnityEngine.UI;

public class WallSensor : MonoBehaviour
{

	[SerializeField] private Transform rayOriginPoint;
	[SerializeField] private float lineDistance = 4f;
	[SerializeField] private int rayQuantity = 2;
	[SerializeField] private string rayLayerName = "Environment";
	[SerializeField] private Text sensorText;
	private string rawSensorText = "";

	// A raycastHit-ben vannak a sugarak adatai tarolva.
	private RaycastHit[] raycastHit;

	void Start()
	{
		raycastHit = new RaycastHit[rayQuantity + 1];
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

			// Megrajzolja a bal es jobb oldali sugarakat.
			Debug.DrawRay(rayOriginPoint.position, rightRot * rayOriginPoint.forward * lineDistance, Color.yellow);
			Debug.DrawRay(rayOriginPoint.position, leftRot * rayOriginPoint.forward * lineDistance, Color.yellow);

			// Letrehozza a bal es jobb oldali sugarakat.
			Physics.Raycast(rayOriginPoint.position, rightRot * rayOriginPoint.forward, out raycastHit[i - 1], lineDistance, LayerMask.GetMask(rayLayerName));
			Physics.Raycast(rayOriginPoint.position, leftRot * rayOriginPoint.forward, out raycastHit[j], lineDistance, LayerMask.GetMask(rayLayerName));

		}

		// Megrajzolja a kozepso sugarat.
		Debug.DrawRay(rayOriginPoint.position, rayOriginPoint.forward * lineDistance, Color.yellow);
		// Letrehozza a kozepso sugarat.
		Physics.Raycast(rayOriginPoint.position, rayOriginPoint.forward, out raycastHit[quantity], lineDistance, LayerMask.GetMask(rayLayerName));

	}


}
