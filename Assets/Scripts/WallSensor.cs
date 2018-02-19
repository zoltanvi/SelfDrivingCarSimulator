using UnityEngine;
using UnityEngine.UI;

public class WallSensor : MonoBehaviour
{

	[SerializeField] private Transform m_RayOrigin;
	[SerializeField] private float m_lineDistance = 4f;
	[SerializeField] private int m_rayQuantity = 2;
	[SerializeField] private string m_rayLayerName = "Environment";
	[SerializeField] private Text m_sensorText;
	private string m_raycastText;

	// raycastHit-ben vannak a sugarak adatai tarolva
	private RaycastHit[] raycastHit;

	void Start()
	{
		raycastHit = new RaycastHit[m_rayQuantity + 1];
		m_raycastText = "";
	}


	// Update is called once per frame
	void FixedUpdate()
	{
		CreateRays(m_rayQuantity);

		m_raycastText = "";
		for (int i = 0; i < raycastHit.Length; i++)
		{
			m_raycastText += (i + 1) + ". sensor: " + raycastHit[i].distance.ToString() + "\n";
			//m_raycastText += raycastHit[i].distance.ToString() + "\n";
		}
		m_sensorText.text = m_raycastText;
	}

	// Mindket oldalra quantity-1 db sugarat fog vetni, + kozepre egyet.
	void CreateRays(int quantity)
	{
		// angleBase = a sugarak kozotti szog nagysaga
		float angleBase = 90f / quantity;
		for (int i = 1, j = (quantity - 1); i < (quantity); i++, j--)
		{
			// A szogek jobb es bal oldalra novekednek / csokkennek, angleBase-enkent
			Quaternion rightRot = Quaternion.AngleAxis(angleBase * i, m_RayOrigin.up);
			Quaternion leftRot = Quaternion.AngleAxis(-angleBase * i, m_RayOrigin.up);

			// Megrajzolja a bal es jobb oldali sugarakat
			Debug.DrawRay(m_RayOrigin.position, rightRot * m_RayOrigin.forward * m_lineDistance, Color.yellow);
			Debug.DrawRay(m_RayOrigin.position, leftRot * m_RayOrigin.forward * m_lineDistance, Color.yellow);

			// Letrehozza a bal es jobb oldali sugarakat
			Physics.Raycast(m_RayOrigin.position, rightRot * m_RayOrigin.forward, out raycastHit[i - 1], m_lineDistance, LayerMask.GetMask(m_rayLayerName));
			Physics.Raycast(m_RayOrigin.position, leftRot * m_RayOrigin.forward, out raycastHit[j], m_lineDistance, LayerMask.GetMask(m_rayLayerName));
			
		}

		// Megrajzolja a kozepso sugarat
		Debug.DrawRay(m_RayOrigin.position, m_RayOrigin.forward * m_lineDistance, Color.yellow);
		// Letrehozza a kozepso sugarat
		Physics.Raycast(m_RayOrigin.position, m_RayOrigin.forward, out raycastHit[quantity], m_lineDistance, LayerMask.GetMask(m_rayLayerName));
	

	}


}
