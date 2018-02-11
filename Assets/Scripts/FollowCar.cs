using UnityEngine;

public class FollowCar : MonoBehaviour
{

     private float smoothSpeed = 10f;
     public Transform targetCar;

     public Vector3 offset = new Vector3(0.62f, 5.83f, -7.5f);


     void FixedUpdate()
     {
          Vector3 desiredPosition = targetCar.position + (targetCar.rotation * offset);
          Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
          transform.position = smoothedPosition;

          Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetCar.rotation, Time.deltaTime * smoothSpeed);
          transform.rotation = smoothedRotation;
          transform.LookAt(targetCar);


     }

}
