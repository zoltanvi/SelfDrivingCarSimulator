using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour {

     [SerializeField] Transform m_RayOrigin;
     [SerializeField] float lineDistance = 4f;


     // Update is called once per frame
     void FixedUpdate() {

          
          RaycastHit raycastHit;
          Debug.DrawRay(m_RayOrigin.position, m_RayOrigin.forward * lineDistance, Color.red);

          //Debug.Log(m_RayOrigin.forward);


          if (Physics.Raycast (m_RayOrigin.position, m_RayOrigin.forward, out raycastHit, lineDistance))
          {
               if (raycastHit.collider.tag == "Wall")
               {
                    Debug.Log(raycastHit.distance);
               }
          }



     }



}
