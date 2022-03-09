using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public Transform MainCam;
    public float camSpeed;
    public Transform GameWorld;
    public float defaultWorldPos;
    public float offsetWorldPos;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void ScrollCam()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            MainCam.position -= transform.right * camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MainCam.position += transform.right * camSpeed * Time.deltaTime;
        }
    }
    }
