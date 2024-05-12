using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    private List<Collider> rowColliders = new List<Collider>();
    private List<GameObject> collidingObjects = new List<GameObject>();
    public static bool isDeleting = false;

    public static List<GameObject> blockPrefabs;
    public static List<GameObject> previewBlockPrefabs;

    public static Vector3 activeSpawnPos = new Vector3(-0.535f, -0.285f, 0.955f);
    public static GameObject activeBlock = null;
    public static Vector3 previewSpawnPos = new Vector3(-0.9f, -0.35f, 0.785f);
    public static GameObject upcomingBlock = null;

    public static int upcomingBlockType = 2;

    private static BlockManager instance;

    public static BlockManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("BlockManager").AddComponent<BlockManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("RowCollider");
        foreach (GameObject gameObject in objectsWithTag)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                rowColliders.Add(collider);
            }
        }

        blockPrefabs = new List<GameObject>();
        for (int i = 1; i < 5; i++)
        {
            string prefabName = "Prefabs/Blocks/Block" + i;
            GameObject blockPrefab = Resources.Load<GameObject>(prefabName);
            blockPrefabs.Add(blockPrefab);
        }

        previewBlockPrefabs = new List<GameObject>();
        for (int i = 1; i < 5; i++)
        {
            string prefabName = "Prefabs/PreviewBlocks/PreviewBlock" + i;
            GameObject blockPrefab = Resources.Load<GameObject>(prefabName);
            previewBlockPrefabs.Add(blockPrefab);
        }

    }

    // public static void InstantiateBlock()
    // {
    //     Destroy(upcomingBlock);
    //     activeBlock = Instantiate(blockPrefabs[upcomingBlockType],activeSpawnPos,Quaternion.Euler(new Vector3(0, 0, 0)));
    //     int minValue = 0;
    //     int maxValue = 3;
    //     System.Random random = new System.Random();
    //     upcomingBlockType = random.Next(minValue, maxValue + 1);
    //     upcomingBlock = Instantiate(previewBlockPrefabs[upcomingBlockType],previewSpawnPos,Quaternion.Euler(new Vector3(0, 0, 0)));

    // }
    public static void InstantiateBlock()
    {
        Destroy(upcomingBlock);
        activeBlock = Instantiate(blockPrefabs[upcomingBlockType], activeSpawnPos, Quaternion.Euler(new Vector3(0, 0, 0)));
        int minValue = 0;
        int maxValue = 3;
        System.Random random = new System.Random();
        upcomingBlockType = random.Next(minValue, maxValue + 1);
        upcomingBlock = Instantiate(previewBlockPrefabs[upcomingBlockType], previewSpawnPos, Quaternion.Euler(new Vector3(0, 0, 0)));
    }


    public static void SpawnBlock(GameObject obj)
    {
        XROffsetGrabInteractable objScript = obj.GetComponent<XROffsetGrabInteractable>();
        if (objScript.shouldSpawnSoon == true)
        {
            
            BlockManager.InstantiateBlock();
            objScript.shouldSpawnSoon = false;
        }
        else
        {
            for (int i = 0; i < objScript.goList.Count; ++i)
            {
                GameObject cubeBrother = objScript.goList[i];
                if(cubeBrother != null)
                {
                    XROffsetGrabInteractable offsetGrabInteractable = cubeBrother.GetComponent<XROffsetGrabInteractable>();
                    if (offsetGrabInteractable.shouldSpawnSoon == true)
                    {
                        BlockManager.InstantiateBlock();                        
                        offsetGrabInteractable.shouldSpawnSoon = false;
                    }
                }
            }
        }
    }

    public static IEnumerator DeleteBlocks(List<GameObject> collidingObjects)
    {
        if(!isDeleting) //before isDeleting == false
        { 
            isDeleting=true;

            //Debug.Log("Only Once?");
            foreach (GameObject obj in collidingObjects)
            {
                Transform parentTransform = obj.transform.parent;

                // Check if the parent object should be destroyed
                bool destroyParent = true;
                foreach (Transform childTransform in parentTransform)
                {
                    if (collidingObjects.Contains(childTransform.gameObject))
                    {
                        // Destroy the child object if it's in the collidingObjects list
                        Destroy(childTransform.gameObject);
                    }
                    else
                    {
                        // If there is at least one child object not in the collidingObjects list, don't destroy the parent
                        destroyParent = false;
                    }
                }

                // Destroy the parent object if all child objects were in the collidingObjects list
                if (destroyParent)
                {
                    Destroy(parentTransform.gameObject);
                }

                // Destroy the current object
                Destroy(obj);
            }
            //wait one frame to wait for complete deletion of obj
            yield return new WaitForEndOfFrame();         
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject obj in blocks)
            {
                if(obj != null)
                {
                    obj.GetComponent<XROffsetGrabInteractable>().SetEnteredDropZone(false);
                    obj.GetComponent<XROffsetGrabInteractable>().SetHitTarget(false);
                }
            }

            //Debug.Log("Block Length"+blocks.Length);
            foreach (GameObject obj in blocks)
            {
                if(obj != null)
                {
                    //dont remove all constraints only y pos
                    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    obj.GetComponent<Rigidbody>().isKinematic = false;
                    if(obj.GetComponent<XROffsetGrabInteractable>().enteredDropzone == false)
                    {
                        CoroutineManager.Instance.StartCoroutine(obj.GetComponent<XROffsetGrabInteractable>().FallDown());
                        //Debug.Log("inner Falldown");
                    }
                }
            }
        }
    }
}
