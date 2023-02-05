using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    public List<GameObject> goList;
    Material[] newMaterials = new Material[2];


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
}
