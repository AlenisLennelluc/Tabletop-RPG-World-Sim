using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private GameState gameState;
    public GameObject selectedObject;
    private GameController controller;


    private void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GameObject hitObject;
            if (CastRay(out hitObject))
            {
                controller.LeftMouseUp(hitObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GameObject hitObject;
            if (CastRay(out hitObject))
            {
                controller.RightMouseDown(hitObject);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            GameObject hitObject;
            if (CastRay(out hitObject))
            {
                controller.RightMouseUp(hitObject);
            }
        }


    }


    private bool CastRay(out GameObject hitObject)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            hitObject = hitInfo.collider.transform.gameObject;
            if (hitObject.CompareTag("Unit"))
            {
                hitObject = hitObject.transform.parent.parent.gameObject;
            }
            return true;
        }

        hitObject = null;
        return false;
    }
}
