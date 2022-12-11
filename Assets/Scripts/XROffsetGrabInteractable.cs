using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    public GameObject cube;
    private Rigidbody cubeRb;       
    private float timeLeft = 0.125f;
    private float stoppedMovingTimer = 1f;
    public Vector3 currentRotation;
    public Vector3 currentPosition;
    public Material landedMaterial;
    public Vector3 targetPosition = new Vector3(0, 0, 0);
    private bool selected = true;
    public bool landed = false;
    public bool firstContact = false;
    public bool stoppedMoving = false;


    // [][][][] = 0
    //
    //
    // []       = 1
    //
    //
    // [][][]   = 2
    //  []
    //
    // [][]     = 3
    //   [][]
    //
    // [][][]   = 4
    // []

    // In mehere separate Skripts aufteilen
    // Problem bei [][][][] Block wird nur beim  untersten sub block die rotatioj eingefroren.
    



    void Start()
    {
        cubeRb = cube.GetComponent<Rigidbody>();

        //Physics.IgnoreCollision(childCollider.GetComponent<Collider>(), GetComponent<Collider>());

        if(!attachTransform)
        {
            GameObject attachPoint = new GameObject("Offset Grab Pivot");
            attachPoint.transform.SetParent(transform, false);
            attachTransform = attachPoint.transform;
        }

    }


    void Update()
    {
        if(selected == false)
        {
            timeLeft -= Time.deltaTime;
        }
        else if(timeLeft != 0.125f)
        {
            timeLeft = 0.125f;
        }
        if(timeLeft < 0f)
        {
          StartCoroutine(LerpRotation(this.transform.rotation, 0.25f)); 
        }


        if(stoppedMoving == false && firstContact == true )
        {
            
            if(cubeRb.velocity.magnitude <  0.005f)
            {
                stoppedMovingTimer -= Time.deltaTime;
                
            }
            else
            {
                stoppedMovingTimer = 0.5f;
            }
              
        }
        if(stoppedMovingTimer < 0f && landed != true)
        {
           stoppedMoving = true; 

        }


    }

    IEnumerator LerpRotation(Quaternion initialRotation, float duration)
    {
        float time = 0;
         
        Vector3 vec = initialRotation.eulerAngles;

        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;

        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

        targetRotation.eulerAngles = new Vector3(vec.x,vec.y,vec.z);

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        cubeRb.freezeRotation = true; 

    }

    void roundPosition(Vector3 initialPosition)
    {
        Vector3 vec = initialPosition;


        vec.x = Mathf.Round(vec.x * 10.0f) * 0.1f;
        vec.z = Mathf.Round(vec.z * 10.0f) * 0.1f;


        cube.transform.position = new Vector3(vec.x, this.transform.position.y, vec.z);
    }

    IEnumerator DelayedPosition(Collision collision)
    {
        
        firstContact = true;



       
        roundPosition(cube.transform.position); 
        yield return new WaitUntil(() => stoppedMoving==true);
        Debug.Log("stoppedMoving");
        cube.GetComponent<MeshRenderer>().material = landedMaterial;
        cubeRb.isKinematic = true;
        roundPosition(cube.transform.position);  
        
        this.GetComponent<BoxCollider>().size = new Vector3(0.75f, 0.75f, 0.75f);
        landed = true;


    }



    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        selected=true;
        attachTransform.rotation = args.interactorObject.transform.rotation;
        cubeRb.freezeRotation = false;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
    
        selected=false;

    }

    void OnCollisionEnter(Collision collision)
    {
        
        

        if (collision.gameObject.tag != "Wall" && landed != true)
        {
            
            StartCoroutine(DelayedPosition(collision));

        }


    }


    private Vector3 offset(string type)
    {
        
        Vector3 offset = new Vector3(0,0,0);

        switch (type)
        {
        case "0":
            offset = new Vector3(-0.05f,0,-0.05f);
        break;
        case "1":
            offset = new Vector3(0,0,0);
        break;
        case "2":
            offset = new Vector3(0,0,0);
        break;
        case "3":
            offset = new Vector3(0,0,0);
        break;
        case "4":
            offset = new Vector3(0,0,0);
        break;

        }

        return offset;

    }
}
