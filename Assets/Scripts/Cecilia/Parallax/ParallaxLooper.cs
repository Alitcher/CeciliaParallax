using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cecilia.Parallax 
{
    public class ParallaxLooper : MonoBehaviour
    {
        //Attach this script to the parent and assign children to the list
        [SerializeField] private float[] boundary;
        [SerializeField] private List<Transform> childrenObjects;
        // Update is called once per frame
        void Update()
        {
            WarpLocalChild();
        }

        private void WarpLocalChild() 
        {
            if (this.gameObject.transform.position.x < boundary[0])
            {
                this.transform.position = new Vector3(boundary[1], this.transform.position.y, this.transform.position.z);
            }
            else if (this.gameObject.transform.position.x > boundary[1])
            {
                this.transform.position = new Vector3(boundary[0], this.transform.position.y, this.transform.position.z);
            }
        }
    }
}


