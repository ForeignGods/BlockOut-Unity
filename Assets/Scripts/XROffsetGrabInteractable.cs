using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    public GameObject cube;
    private Rigidbody cubeRb;       
    private float timeLeft = 0.125f;
    private float stoppedMovingTimer = 0.5f;
    public Vector3 currentRotation;
    public Vector3 currentPosition;
    public Material landedMaterial;
    public Vector3 targetPosition = new Vector3(0, 0, 0);
    private bool selected = true;
    public bool landed = false;
    public bool firstContact = false;
    public bool stoppedMoving = false;
    public List<GameObject> goList;
    private GameObject[] rays;

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
    
    // Rigidbody 
    // Mass = 10 
    // Drag = 2.5 
    // Angular Drag = 250

    // XR Grab Interactable

    // Movement type = Velocity Tracking 

    // Smooth Position Amount = 10
    // Tighten Position = 1 
    // Velocity Damping = 1
    // Velocity Scale = 1 

    // Smooth Position Amount = 1
    // Tighten Position = 1 
    // Angular Velocity Damping = 1
    // Angular Velocity Scale = 1 






    void Start()
    {
        cubeRb = cube.GetComponent<Rigidbody>();
        rays = GameObject.FindGameObjectsWithTag("Ray");

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
            
            if(cubeRb.velocity.magnitude <  0.5f)
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

        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cube = goList[i];
            if(cube.GetComponent<Rigidbody>().freezeRotation == false)
            {
                
                Vector3 vecCube = cube.transform.rotation.eulerAngles;

                vecCube.x = Mathf.Round(vec.x / 90) * 90;
                vecCube.y = Mathf.Round(vec.y / 90) * 90;
                vecCube.z = Mathf.Round(vec.z / 90) * 90;

                Quaternion targetRotationCube = Quaternion.Euler(0, 0, 0);

                targetRotationCube.eulerAngles = new Vector3(vecCube.x,vecCube.y,vecCube.z);

                cube.transform.rotation = targetRotationCube;
                cube.GetComponent<Rigidbody>().freezeRotation = true; 
            }

        }

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

        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cube = goList[i];
            Vector3 vec = cube.transform.position;


            vec.x = Mathf.Round(vec.x * 10.0f) * 0.1f;
            vec.z = Mathf.Round(vec.z * 10.0f) * 0.1f;


            cube.transform.position = new Vector3(vec.x, cube.transform.position.y, vec.z);

        } 

        yield return new WaitUntil(() => stoppedMoving==true);
        Debug.Log("stoppedMoving");
        cubeRb.isKinematic = true;
        roundPosition(cube.transform.position);  
        
        this.GetComponent<BoxCollider>().size = new Vector3(0.75f, 0.75f, 0.75f);
        landed = true;
        roundPosition(cube.transform.position); 
        // roundPosition for every other cube in same block
        // set material = landed, set all bools and change size of boxCollider
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cube = goList[i];
            if(cube.GetComponent<XROffsetGrabInteractable>().landed == false)
            {
                
                cube.GetComponent<Rigidbody>().isKinematic = true;
                //roundPosition(cube.transform.position); 
                cube.GetComponent<BoxCollider>().size = new Vector3(0.75f, 0.75f, 0.75f);

                cube.GetComponent<XROffsetGrabInteractable>().stoppedMoving = true;
                cube.GetComponent<XROffsetGrabInteractable>().firstContact = true;
                cube.GetComponent<XROffsetGrabInteractable>().landed = true;
            }

        }

    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        

        if (rays.Length >= 2)
        {
            rays[0].GetComponent<XRInteractorLineVisual>().enabled=false;
            rays[1].GetComponent<XRInteractorLineVisual>().enabled=false;
        }
        else
        {
            Debug.LogError("There are not enough GameObjects with the Ray tag to disable the line visuals.");
        }


        selected=true;

        attachTransform.rotation = args.interactorObject.transform.rotation;
        cubeRb.freezeRotation = false;
        base.OnSelectEntered(args);
    }

    public GameObject[] blocks;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (rays.Length >= 2)
        {
            rays[0].GetComponent<XRInteractorLineVisual>().enabled=true;
            rays[1].GetComponent<XRInteractorLineVisual>().enabled=true;
        }
        else
        {
            Debug.LogError("There are not enough GameObjects with the Ray tag to disable the line visuals.");
        }
        //rightRay.enabled=true;
        Invoke("spawnBlock", 1f);
        


        selected=false;

    }

    public void spawnBlock(){

        Vector3 spawnPos = new Vector3(-0.6f,-0.4f,0.35f);

        Vector3 spawnRot = new Vector3(0,0,0);
        Quaternion spawnQuaternion = Quaternion.Euler(spawnRot);
        int minValue = 0;
        int maxValue = 2;
        System.Random random = new System.Random();
        //Random random = new Random();
        int randomInt = random.Next(minValue, maxValue + 1);
        
        Debug.Log(spawnPos);

        GameObject prefabInstance = Instantiate(blocks[randomInt],spawnPos,spawnQuaternion);
        //PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

    }

    void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag != "Wall" && landed != true)
        {
            
            StartCoroutine(DelayedPosition(collision));

        }


    }
 
}
