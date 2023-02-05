using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private bool meshRendererEnabled = false;
    // Update is called once per frame
    void Update()
    {
    
        if(gameObject.GetComponent<XROffsetGrabInteractable>().stoppedMoving == true && meshRendererEnabled==false)
        {
            // Access the first child GameObject
            GameObject firstChild = gameObject.transform.GetChild(0).gameObject;
            // Enable the MeshRenderer component of the first child
            firstChild.GetComponent<MeshRenderer>().enabled = true;
            
            // Access the second child GameObject
            GameObject secondChild = gameObject.transform.GetChild(1).gameObject;
            // Enable the MeshRenderer component of the second child
            secondChild.GetComponent<MeshRenderer>().enabled = true;
            
            if(string.Compare(gameObject.name, "1", true) == 0)
            {

                GameObject thirdChild = gameObject.transform.GetChild(2).gameObject;
                // Enable the MeshRenderer component of the second child
                thirdChild.GetComponent<MeshRenderer>().enabled = false;

            }

            meshRendererEnabled= true;

        }

    }
}
