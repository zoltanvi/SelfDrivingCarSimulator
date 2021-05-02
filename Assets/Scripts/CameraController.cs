/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0f, 5f)] [SerializeField] private const float MovementSpeed = 3.2f;
    [Range(0f, 5f)] [SerializeField] private const float RotationSpeed = 3.8f;

    public Transform CameraTarget;
    private readonly Vector3 m_Offset = new Vector3(0.62f, 5.83f, -7.5f);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {

        // Az auto poziciojat koveti a kamera
        Vector3 desiredPosition = CameraTarget.position + (CameraTarget.rotation * m_Offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * MovementSpeed);
        transform.position = smoothedPosition;

        // Ha irányítjuk az egyik autót, akkor a kamera az autóra néz, nem fog annyira forogni
        if (Master.Instance.Manager.ManualControl)
        {
            Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, CameraTarget.rotation, Time.deltaTime * RotationSpeed);
            transform.rotation = smoothedRotation;
            transform.LookAt(CameraTarget);
        }
        else
        {
            // Lágyabb kamera forgás, ha nem mi vezetjük az autót
            Quaternion toRot = Quaternion.LookRotation(CameraTarget.position - transform.position, CameraTarget.up);
            Quaternion curRot = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * RotationSpeed);
            transform.rotation = curRot;
        }
    }
}
