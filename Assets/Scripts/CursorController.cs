using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private Transform cursorTransform;


    private PlayerController playerController;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        mainCamera = cameraObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cursorTransform.position = Camera.main.ScreenToWorldPoint(playerController.mouseAction.ReadValue<Vector2>());
    }
}