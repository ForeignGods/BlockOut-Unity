using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using System.Linq;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    public Vector3 currentRotation;
    public Vector3 currentPosition;
    public Vector3 targetPosition = new Vector3(0, 0, 0);
    public bool landed = false;
    public bool firstContact = false;
    public bool stoppedMoving = false;
    public bool shouldSpawnSoon = false;
    public bool exited = false;
    public bool canFall = false;
    public bool enteredDropzone = false;

    public Renderer wireframeRenderer;
    public Material notLandedMaterial;
    public Material almostLandedMaterial;
    public Material landedMaterial;

    public List<GameObject> goList;
    private List<Collider> rowColliders = new List<Collider>();
    private List<GameObject> collidingObjects = new List<GameObject>();
    public List<GameObject> droppedObjects = new List<GameObject>();
    private GameObject[] rays;

    private Collider spawnCollider;
    public GameObject cube;
    private Rigidbody cubeRb;



    void Start()
    {
        // Notizen
        // ----------------------
        // Auf Platform Preview von naechstem Block der erscheinen wird 3D rotierend Wireframe wow speccial effects
        // Auf PLatform Cage Save Block (links Next Block, rechts Save Block)

        // entweder ui mit armband hologram style oder auch auf platform
        // toggle gravitiy button in vr welt auf platform


        // Endless Mode, Timed Mode, Zen Mode, 
        // Haende mit Wireframe shader
        // Player Position in Tube oder oberhalb?
        
        // Rigidbody auf PArent Block um fixedjoints zu ersetzten
        // Wobbly Mode(Zero Gravity even after Dropzone, roundPos etc. after targetHit)

        // Round Tube Background, plus rising reactive walls cool efect tube and walls(wall not a good work more like )

        // Ausgangslage
        // -----------------------
        // enteredDropzone = false 
        // constraints FreezePosition = false
        // constraints FreezeRotation = false
        // hitTarget = false
        // isKinematic = false

        // When entering Dropzone
        // -----------------------
        // enteredDropzone = true
        // constraints FreezePosition = true (x,z)
        // constraints FreezeRotation = true
        // hitTarget = false
        // isKinematic = false
        
        // When hit Target 
        // -----------------------
        // enteredDropzone = true
        // constraints FreezePosition = true (x,z)
        // constraints FreezeRotation = true
        // hitTarget = true
        // isKinematic = true

        // When layer is full (collidingObjects.count == 36)
        // enteredDropzone = true 
        // constraints FreezePosition = false
        // constraints FreezeRotation = true 
        // hitTarget = false 
        // isKinematic = false

        cube.GetComponent<BoxCollider>().size = new Vector3(0.99f, 0.99f, 0.99f);
        cube.GetComponent<BoxCollider>().contactOffset=0.00001f;

        cubeRb = cube.GetComponent<Rigidbody>();
        //try only needed for debugging
        try 
        {
            spawnCollider = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Collider>();
        }
        catch
        {

        }
        rays = GameObject.FindGameObjectsWithTag("Ray");

        GameObject[] rowColliderObjs = GameObject.FindGameObjectsWithTag("RowCollider");
        foreach (GameObject obj in rowColliderObjs)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                rowColliders.Add(collider);
            }
        }


        if (!attachTransform)
        {
            GameObject attachPoint = new GameObject("Offset Grab Pivot");
            attachPoint.transform.SetParent(transform, false);
            attachTransform = attachPoint.transform;
        }

        //BlockManager.InstantiateBlock();
    }

    public bool hitTarget=false;
    public IEnumerator FallDown()
    {

        SetEnteredDropZone(true);
        RoundRotPosAll("Pos:XZ");
        SetInteractionLayer("NotGrabable");
        yield return new WaitUntil(() => hitTarget == true);
        SetIsKinematic(true);
        RoundRotPosAll("Pos:XYZ");

        for (int i = 0; i < rowColliders.Count; ++i)
        {
            Collider[] colliders = Physics.OverlapBox(rowColliders[i].bounds.center,rowColliders[i].bounds.extents);
            List<GameObject> collidingObjects = colliders
                .Select(c => c.gameObject)
                .Where(go => go != rowColliders[i].gameObject)
                .ToList();

            foreach (var obj in collidingObjects) 
            {
                Transform cubeLanded = obj.transform.Find("cube_landed");
                Transform wireframeLanded = obj.transform.Find("wireframe_landed");
                Transform wireframeNotLanded = obj.transform.Find("wireframe_not_landed");
                XROffsetGrabInteractable script = obj.GetComponent<XROffsetGrabInteractable>();

                if (cubeLanded != null && script.hitTarget) 
                {
                    Renderer renderer = cubeLanded.GetComponent<Renderer>();
                    if (!renderer.enabled) 
                    {
                        renderer.enabled = true;
                    }
                    Material newMaterial = new Material(Shader.Find("Unlit/Color"));
                    newMaterial.color = ColorManager.colors[i];
                    renderer.material = newMaterial;
                }

                if (wireframeLanded != null && !wireframeLanded.GetComponent<Renderer>().enabled) 
                {
                    wireframeLanded.GetComponent<Renderer>().enabled = true;
                }
                if (wireframeNotLanded != null && wireframeNotLanded.GetComponent<Renderer>().enabled) 
                {
                    wireframeNotLanded.GetComponent<Renderer>().enabled = false;
                }
            }

            if (collidingObjects.Count >= 36) 
            {
                BlockManager.isDeleting = false;
                CoroutineManager.Instance.StartCoroutine(BlockManager.DeleteBlocks(collidingObjects));
            }
        }

    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        //this.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        this.shouldSpawnSoon = false;
        SetShouldSpawnSoonOfBrothers(false);

        if (rays.Length >= 2)
        {
            rays[0].GetComponent<XRInteractorLineVisual>().enabled = false;
            rays[1].GetComponent<XRInteractorLineVisual>().enabled = false;
        }
        spawnCollider.enabled = false;
        attachTransform.rotation = args.interactorObject.transform.rotation;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        shouldSpawnSoon = true;
        SetShouldSpawnSoonOfBrothers(false);
        if (rays.Length >= 2)
        {
            rays[0].GetComponent<XRInteractorLineVisual>().enabled = true;
            rays[1].GetComponent<XRInteractorLineVisual>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(gameObject+" triggered "+collision.gameObject.tag);
        if (collision.gameObject.tag == "Dropzone" && enteredDropzone == false)
        {
            Debug.Log("Collision with Dropzone");
            StartCoroutine(FallDown());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject+" collided "+collision.gameObject.tag);
        if (collision.gameObject.tag=="BlockSaver")
        {

        }

        if ((collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Block") && enteredDropzone == true)
        {
            SetHitTarget(true);
            //if only needed for debugging
            if(spawnCollider != null)
            {
                spawnCollider.enabled = true;
            }
            BlockManager.SpawnBlock(gameObject);
        }
    }

    public float RoundToNearest(float value, float roundTo) 
    {
        return roundTo * Mathf.Round(value / roundTo);
    }

    public void RoundRotation() 
    {
        Vector3 eulerAngles = this.gameObject.transform.rotation.eulerAngles;
        eulerAngles.x = RoundToNearest(eulerAngles.x, 90);
        eulerAngles.y = RoundToNearest(eulerAngles.y, 90);
        eulerAngles.z = RoundToNearest(eulerAngles.z, 90);
        this.gameObject.transform.rotation = Quaternion.Euler(eulerAngles);
        cubeRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void RoundPosition(string mode) 
    {
        Vector3 position = this.gameObject.transform.position;
        position.x = Mathf.Round(position.x * 10.0f) * 0.1f;
        if(mode == "Pos:XYZ")
        {
            position.y = Mathf.Round(position.y * 10.0f) * 0.1f;
        }
        position.z = Mathf.Round(position.z * 10.0f) * 0.1f;
        this.gameObject.transform.position = position;
        if(mode == "Pos:XZ")
        {
            cubeRb.constraints = RigidbodyConstraints.FreezePositionX|RigidbodyConstraints.FreezePositionZ;
        }
    }

    public void RoundRotPosAll(string mode)
    {
        RoundRotation();
        RoundPosition(mode);
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if (cubeBrother != null)
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                offsetGrabInteractable.RoundPosition(mode);
                offsetGrabInteractable.RoundRotation();
            }
        }
    }

    public void SetShouldSpawnSoonOfBrothers(bool state)
    {
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if(cubeBrother != null )
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                if(offsetGrabInteractable.shouldSpawnSoon != state)
                {
                    offsetGrabInteractable.shouldSpawnSoon = state;
                }
            }
        }
    }

    public void SetEnteredDropZone(bool state)
    {
        enteredDropzone = state;
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if (cubeBrother != null)
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                if (offsetGrabInteractable.enteredDropzone != state)
                {
                    offsetGrabInteractable.enteredDropzone = state;
                }
            }
        }
    }

    public void SetIsKinematic(bool state)
    {
        this.cubeRb.isKinematic = state;
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if(cubeBrother != null)
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                if(offsetGrabInteractable.cubeRb.isKinematic != state) 
                {
                    offsetGrabInteractable.cubeRb.isKinematic = state;
                }
            }
        }
    }

    public void SetHitTarget(bool state)
    {

        this.hitTarget = state;
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if (cubeBrother != null)
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                if(offsetGrabInteractable.hitTarget != state) 
                {
                    offsetGrabInteractable.hitTarget = state;
                }
                
            }
        }
    }

    public void SetInteractionLayer(string layer)
    {
        this.interactionLayers = 1 << LayerMask.NameToLayer(layer);
        for (int i = 0; i < goList.Count; ++i)
        {
            GameObject cubeBrother = goList[i];
            if (cubeBrother != null)
            {
                XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                offsetGrabInteractable.interactionLayers = 1 << LayerMask.NameToLayer(layer); 
            } 
        }
    }
}
