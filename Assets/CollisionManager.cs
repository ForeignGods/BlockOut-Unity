using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    public List<GameObject> goList;
    public Material landedMaterial;

    void Start()
    {
        foreach(GameObject goA in goList)
        {
            

            foreach(GameObject goB in goList)
            {
                
                Physics.IgnoreCollision(goA.GetComponent<Collider>(), goB.GetComponent<Collider>());

            }
        }
    }

    // Performance problem
    void FixedUpdate()
    {
        foreach(GameObject goA in goList)
        {
            
            if(goA.GetComponent<XROffsetGrabInteractable>().stoppedMoving==true)
            {
                        this.GetComponent<MeshRenderer>().material = landedMaterial;
            }

        }  
    }
}
