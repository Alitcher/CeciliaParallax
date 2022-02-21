using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBar_Holder : MonoBehaviour 
{
    [SerializeField] private GameController m_Controller;
    [SerializeField] private SwipeParallaxController m_GameWorldParallax;
    public Animator myAnim;
    int currentState = 0;
    bool isUp;
	// Use this for initialization
	void Start () {
        myAnim = gameObject.GetComponent<Animator>();
        myAnim.SetInteger("InventoryPresses", 0);
        isUp = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isUp)
        {
            if (m_GameWorldParallax.transform.position.y < m_Controller.offsetWorldPos)
            {
                m_GameWorldParallax.transform.position = new Vector3(m_Controller.GameWorld.transform.position.x, m_Controller.GameWorld.transform.position.y + 1f, m_Controller.GameWorld.transform.position.z);
            }
            //else
            //    isUp = false;
        }
        else if(!isUp)
        {
            if (m_Controller.GameWorld.transform.position.y > m_Controller.defaultWorldPos)
            {
                m_Controller.GameWorld.transform.position = new Vector3(m_Controller.GameWorld.transform.position.x, m_Controller.GameWorld.transform.position.y - 1f, m_Controller.GameWorld.transform.position.z);
            }

        }
    }

    public void ActivateUpProduct()
    {
       // print(currentState);
        if (currentState == 1)
        {
            isUp = false;
            currentState = 2;
            myAnim.SetInteger("InventoryPresses", currentState);
            currentState = 0;

        } 
        else if (currentState == 2)
        {
            isUp = false;
            currentState = 0;
            myAnim.SetInteger("InventoryPresses", currentState);
            
        }
        else
        {

            isUp = true;
            currentState = 1;
            myAnim.SetInteger("InventoryPresses", currentState);
        }
        
    }

}
