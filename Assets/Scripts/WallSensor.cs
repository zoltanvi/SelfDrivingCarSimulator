using UnityEngine;
using UnityEngine.UI;

public class WallSensor : MonoBehaviour
{

    [SerializeField] private Transform rayOriginPoint;
    [SerializeField] private float lineDistance = 5f;
    [SerializeField] private int rayQuantity = 3;
    [SerializeField] private string rayLayerName = "Environment";
    [SerializeField] private Text sensorText;
    private string rawSensorText = "";
    GameObject[] rayHolders;


    // A raycastHit-ben vannak a sugarak adatai tarolva.
    private RaycastHit[] raycastHit;

    void Start()
    {
        raycastHit = new RaycastHit[rayQuantity];
        rayHolders = new GameObject[rayQuantity];

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
    }

    // Letrehozza az erzekelo sugarakat.
    void CreateRays(int quantity)
    {
        // Az angleBase = a sugarak kozotti szog nagysaga.
        float angleBase = 180f / (quantity + 1);
        for (int i = 1; i < (quantity + 1); i++)
        {
            // A jelenlegi sugar szoge balrol jobbra szamitva.
            Quaternion lineRotation = Quaternion.AngleAxis((angleBase * i), (rayOriginPoint.up));

            // Letrehozza a sugarat.
            Physics.Raycast(rayOriginPoint.position, lineRotation * (-rayOriginPoint.right), out raycastHit[i - 1], lineDistance, LayerMask.GetMask(rayLayerName));

            // Megrajzolja a sugarat.
            rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOriginPoint.position);
            rayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOriginPoint.position + lineRotation * (-rayOriginPoint.right) * lineDistance);
        }
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
            rayHolders[i].GetComponent<LineRenderer>().startWidth = 0.08f;
            rayHolders[i].GetComponent<LineRenderer>().endWidth = 0.08f;
            rayHolders[i].GetComponent<LineRenderer>().useWorldSpace = false;
            rayHolders[i].GetComponent<LineRenderer>().material = lineMat;
            rayHolders[i].GetComponent<LineRenderer>().startColor = Color.red;
            rayHolders[i].GetComponent<LineRenderer>().endColor = Color.white;

        }
    }

}
