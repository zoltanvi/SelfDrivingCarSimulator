using UnityEngine;

public class WallSensor : MonoBehaviour {

     [SerializeField] private Transform m_RayOrigin;
     [SerializeField] private float lineDistance = 4f;
     [SerializeField] private int rayQuantity = 2;
     [SerializeField] private string sensorTag = "Wall";

     // raycastHit-ben vannak a sugarak adatai tarolva
     private RaycastHit[] raycastHit;

     void Start()
     {
          raycastHit = new RaycastHit[rayQuantity + 1];
     }


     // Update is called once per frame
     void FixedUpdate() {

          CreateRays(rayQuantity);
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
               Debug.DrawRay(m_RayOrigin.position, rightRot * m_RayOrigin.forward * lineDistance, Color.blue);
               Debug.DrawRay(m_RayOrigin.position, leftRot * m_RayOrigin.forward * lineDistance, Color.red);

               //A jobb oldali sugar
               if (Physics.Raycast(m_RayOrigin.position, rightRot * m_RayOrigin.forward, out raycastHit[i - 1], lineDistance))
               {
                    if (raycastHit[i-1].collider.tag == sensorTag)
                    {
                         Debug.Log("The " + i + ". ray hit something! Distance: " + raycastHit[i-1].distance);
                    }
               }

               // A bal oldali sugar
               if (Physics.Raycast(m_RayOrigin.position, leftRot * m_RayOrigin.forward, out raycastHit[j], lineDistance))
               {
                    if (raycastHit[j].collider.tag == sensorTag)
                    {
                         Debug.Log("The " + (j+1) + ". ray hit something! Distance: " + raycastHit[j].distance);
                    }
               }
          }

          // Megrajzolja a kozepso sugarat
          Debug.DrawRay(m_RayOrigin.position, m_RayOrigin.forward * lineDistance, Color.green);

          // A kozepso sugar 
          if (Physics.Raycast(m_RayOrigin.position, m_RayOrigin.forward, out raycastHit[quantity], lineDistance))
          {
               if (raycastHit[quantity].collider.tag == sensorTag)
               {
                    Debug.Log("The " + (quantity + 1) + ". ray hit something! Distance: " + raycastHit[quantity].distance);
               }
          }



     }


}
