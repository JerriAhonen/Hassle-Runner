using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Transform lookAt; //The player to follow
    public Vector3 offset = new Vector3(0, 5.0f, -10.0f);

    private void Start()
    {
        transform.position = lookAt.position + new Vector3(0, 0, -2.0f);
    }

    private void LateUpdate()   //Late update so that the player moves before the camera.
    {
        Vector3 targetPosition = lookAt.position + offset;
        targetPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
    }
}
