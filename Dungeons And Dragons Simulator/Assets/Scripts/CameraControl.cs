using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	private float upDownAccel;
	private float leftRightAccel;
	private float rotateAccel;
	private float cameraAngle;

	// Use this for initialization
	void Start () 
	{
		upDownAccel = 0;
		leftRightAccel = 0;
		rotateAccel = 0;
		cameraAngle = 75;
	}
	
	// Update is called once per frame
	void Update () 
	{
        {
            /*if (Input.GetKey (KeyCode.W)) 
		{
			upDownAccel += 10;
		}
		else if (Input.GetKey (KeyCode.S)) 
		{
			upDownAccel -= 10;
		}
		else if (upDownAccel > 0) 
		{
			if (upDownAccel > 20) 
			{
				upDownAccel -= 20;
			} 
			else 
			{
				upDownAccel = 0;
			}
		} 
		else if (upDownAccel < 0) 
		{
			if (upDownAccel < -20) 
			{
				upDownAccel += 20;
			} 
			else 
			{
				upDownAccel = 0;
			}
		}

		if (Input.GetKey (KeyCode.A)) 
		{
			leftRightAccel -= 10;
		}
		else if (Input.GetKey (KeyCode.D))
		{
			leftRightAccel += 10;
		}
		else if (leftRightAccel > 0) 
		{
			if (leftRightAccel > 20) 
			{
				leftRightAccel -= 20;
			} 
			else 
			{
				leftRightAccel = 0;
			}
		} 
		else if (leftRightAccel < 0) 
		{
			if (leftRightAccel < -20) 
			{
				leftRightAccel += 20;
			} 
			else 
			{
				leftRightAccel = 0;
			}
		}*/
        }

        upDownAccel = Input.GetAxis("Vertical");
        leftRightAccel = Input.GetAxis("Horizontal");

        float z = upDownAccel * 6 * Time.deltaTime;
        float x = leftRightAccel * 6 * Time.deltaTime;

		gameObject.transform.Translate(x, 0, z);

		if (Input.GetKey(KeyCode.Q))
		{
			rotateAccel -= 5;
		}
		if (Input.GetKey(KeyCode.E))
		{
			rotateAccel += 5;
		}

		rotateAccel = rotateAccel * 4 * Time.deltaTime;

		gameObject.transform.Rotate(0, rotateAccel, 0);

		rotateAccel = 0;

        //float scroll = Input.mouseScrollDelta.y * -1;
        float scroll = -12 * Input.GetAxis("Mouse ScrollWheel");

		if ((scroll > 0 && cameraAngle < 75) || (scroll < 0 && cameraAngle > 0))
		{
			Camera.main.transform.Rotate(Vector3.right * scroll);
			Camera.main.transform.Translate(new Vector3(0, scroll / 2, -1 * scroll));
			cameraAngle += scroll;
		}
	}

    public void UpdatePos(float x, float z)
    {
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
