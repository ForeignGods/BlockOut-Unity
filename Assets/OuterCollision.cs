using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterCollision : MonoBehaviour
{
    
    public GameObject parentCube;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(parentCube.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.tag == "Outer" )
        {
            
            Debug.Log(collision.gameObject.name);
            //StartCoroutine(LerpPosition(this.transform.position, 0.25f)); 
            Vector3 initialPosition = parentCube.transform.position;
            Vector3 vec = initialPosition;

            vec.x = Mathf.Round(vec.x * 10.0f) * 0.1f;
            vec.z = Mathf.Round(vec.z * 10.0f) * 0.1f;

            



            parentCube.transform.position = new Vector3(vec.x, parentCube.transform.position.y , vec.z);



            //Debug.Log(collision.gameObject.tag);

            //Debug.Log(offset(collision.gameObject.tag));



        }
    }
}
