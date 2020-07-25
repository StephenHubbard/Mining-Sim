using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPlayerArrow : MonoBehaviour
{

    PlayerMovement player;
    public GameObject extensionSprite;

    public Camera mainCam;

    private void Awake()
    {
        mainCam = FindObjectOfType<Camera>();
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        Ray cameraRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Vector3 pointToLook = cameraRay.GetPoint(9f);
        Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

        transform.LookAt(new Vector3(transform.position.x, pointToLook.y, pointToLook.z));
    }
}
