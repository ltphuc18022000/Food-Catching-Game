using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GrabObj : MonoBehaviour, InteractionListenerInterface
{
    [Tooltip("List of the objects that may be dragged and dropped.")]
    public GameObject[] draggableObjects;

    [Tooltip("Material used to outline the currently selected object.")]
    public Material selectedObjectMaterial;

    [Tooltip("Drag speed of the selected object.")]
    public float dragSpeed = 3.0f;

    [Tooltip("Minimum Z-position of the dragged object, when moving forward and back.")]
    public float minZ = 0f;

    [Tooltip("Maximum Z-position of the dragged object, when moving forward and back.")]
    public float maxZ = 5f;

    // public options (used by the Options GUI)
    [Tooltip("Whether the objects obey gravity when released, or not. Used by the Options GUI-window.")]
    public bool loadPause = false ;
    public bool loadResume = false;
    [Tooltip("Whether the objects should be put in their original positions. Used by the Options GUI-window.")]
    public bool loadHelp = false;
    public bool loadPlay = false;
    public bool loadMenu= false;

    public bool exit = false;

    // public options (used by the Options GUI)
    [Tooltip("Whether the objects obey gravity when released, or not. Used by the Options GUI-window.")]
    public bool useGravity = false;
    public bool isKinematic = false;

    //public Engine engine;
    [SerializeField] GameObject EngineObj;
    [Tooltip("Camera used for screen ray-casting. This is usually the main camera.")]
    public Camera screenCamera;

    [Tooltip("UI-Text used to display information messages.")]
    public UnityEngine.UI.Text infoGuiText;

    [Tooltip("Interaction manager instance, used to detect hand interactions. If left empty, it will be the first interaction manager found in the scene.")]
    private InteractionManager interactionManager;

    [Tooltip("Index of the player, tracked by the respective InteractionManager. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Whether the left hand interaction is allowed by the respective InteractionManager.")]
    public bool leftHandInteraction = true;

    [Tooltip("Whether the right hand interaction is allowed by the respective InteractionManager.")]
    public bool rightHandInteraction = true;


    // hand interaction variables
    //private bool isLeftHandDrag = false;
    private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

    // currently dragged object and its parameters
    private GameObject draggedObject;
    //private float draggedObjectDepth;
    private Vector3 draggedObjectOffset;
    private Material draggedObjectMaterial;
    private float draggedNormalZ;

    // initial objects' positions and rotations (used for resetting objects)
    private Vector3[] initialObjPos;
    private Quaternion[] initialObjRot;

    // normalized and pixel position of the cursor
    private Vector3 screenNormalPos = Vector3.zero;
    private Vector3 screenPixelPos = Vector3.zero;
    private Vector3 newObjectPos = Vector3.zero;


    //// choose whether to use gravity or not
    //public void SetUsePause(bool bUsePause)
    //{
    //    this.usePause = bUsePause;
    //}
    public void SetUseGravity(bool bUseGravity)
    {
        this.useGravity = bUseGravity;
    }

    public void SetUseKinematic(bool bUseKinematic)
    {
        this.isKinematic = bUseKinematic;
    }

    // request resetting of the draggable objects
    public void howtoPlay()
    {
        loadHelp = true;
    }

    public void playGame()
    {
        loadPlay = true;
    }

    public void Menu()
    {
        loadMenu = true;
    }

    public void Pause()
    {
        loadPause = true;
    }

    public void Resume()
    {
        loadResume = true;
    }

    public void Exit()
    {
        exit = true;
    }


    void Start()
    {
        // by default set the main-camera to be screen-camera
        if (screenCamera == null)
        {
            screenCamera = Camera.main;
        }

        // save the initial positions and rotations of the objects
        initialObjPos = new Vector3[draggableObjects.Length];
        initialObjRot = new Quaternion[draggableObjects.Length];

        for (int i = 0; i < draggableObjects.Length; i++)
        {
            initialObjPos[i] = screenCamera ? screenCamera.transform.InverseTransformPoint(draggableObjects[i].transform.position) : draggableObjects[i].transform.position;
            initialObjRot[i] = screenCamera ? Quaternion.Inverse(screenCamera.transform.rotation) * draggableObjects[i].transform.rotation : draggableObjects[i].transform.rotation;
        }

        // get the interaction manager instance
        if (interactionManager == null)
        {
            //interactionManager = InteractionManager.Instance;
            interactionManager = GetInteractionManager();
        }
    }


    // tries to locate a proper interaction manager in the scene
    private InteractionManager GetInteractionManager()
    {
        // find the proper interaction manager
        MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

        foreach (MonoBehaviour monoScript in monoScripts)
        {
            if ((monoScript is InteractionManager) && monoScript.enabled)
            {
                InteractionManager manager = (InteractionManager)monoScript;

                if (manager.playerIndex == playerIndex && manager.leftHandInteraction == leftHandInteraction && manager.rightHandInteraction == rightHandInteraction)
                {
                    return manager;
                }
            }
        }

        // not found
        return null;
    }


    void Update()
    {
        if (interactionManager != null && interactionManager.IsInteractionInited())
        {
            if (loadHelp && draggedObject == null)
            {

                loadHelp = false;
                LoadHelp();
            }

            if (loadPlay && draggedObject == null)
            {

                loadPlay = false;
                LoadGamePlay();
            }

            if (loadMenu && draggedObject == null)
            {

                loadMenu = false;
                LoadMenu();
            }
            if (exit && draggedObject == null)
            {
                Application.Quit();

            }


            if (loadPause && draggedObject == null)
            {
                loadPause = false;
                EngineObj.GetComponent<Engine>().isPaused = true;
            }
            if (loadResume && draggedObject == null)
            {
                loadResume = false;
                EngineObj.GetComponent<Engine>().isPaused = false;
            }


            if (draggedObject == null)
            {
                // no object is currently selected or dragged.
                bool bHandIntAllowed = (leftHandInteraction && interactionManager.IsLeftHandPrimary()) || (rightHandInteraction && interactionManager.IsRightHandPrimary());

                // check if there is an underlying object to be selected
                if (lastHandEvent == InteractionManager.HandEventType.Grip && bHandIntAllowed)
                {
                    // convert the normalized screen pos to pixel pos
                    screenNormalPos = interactionManager.IsLeftHandPrimary() ? interactionManager.GetLeftHandScreenPos() : interactionManager.GetRightHandScreenPos();

                    screenPixelPos.x = (int)(screenNormalPos.x * (screenCamera ? screenCamera.pixelWidth : Screen.width));
                    screenPixelPos.y = (int)(screenNormalPos.y * (screenCamera ? screenCamera.pixelHeight : Screen.height));
                    Ray ray = screenCamera ? screenCamera.ScreenPointToRay(screenPixelPos) : new Ray();

                    // check if there is an underlying objects
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        foreach (GameObject obj in draggableObjects)
                        {
                            if (hit.collider.gameObject == obj)
                            {
                                // an object was hit by the ray. select it and start drgging
                                draggedObject = obj;
                                draggedObjectOffset = hit.point - draggedObject.transform.position;
                                draggedObjectOffset.z = 0; // don't change z-pos

                                draggedNormalZ = (minZ + screenNormalPos.z * (maxZ - minZ)) -
                                    draggedObject.transform.position.z; // start from the initial hand-z

                                // set selection material
                                draggedObjectMaterial = draggedObject.GetComponent<Renderer>().material;
                                draggedObject.GetComponent<Renderer>().material = selectedObjectMaterial;

                                // stop using gravity while dragging object
                                draggedObject.GetComponent<Rigidbody>().useGravity = false;
                                draggedObject.GetComponent<Rigidbody>().isKinematic = true;
                                break;
                            }
                        }
                    }
                }

            }
            else
            {
                bool bHandIntAllowed = (leftHandInteraction && interactionManager.IsLeftHandPrimary()) || (rightHandInteraction && interactionManager.IsRightHandPrimary());

                if (bHandIntAllowed)
                {
                    // continue dragging the object
                    screenNormalPos = interactionManager.IsLeftHandPrimary() ? interactionManager.GetLeftHandScreenPos() : interactionManager.GetRightHandScreenPos();

                    // convert the normalized screen pos to 3D-world pos
                    screenPixelPos.x = (int)(screenNormalPos.x * (screenCamera ? screenCamera.pixelWidth : Screen.width));
                    screenPixelPos.y = (int)(screenNormalPos.y * (screenCamera ? screenCamera.pixelHeight : Screen.height));
                    //screenPixelPos.z = screenNormalPos.z + draggedObjectDepth;
                    screenPixelPos.z = (minZ + screenNormalPos.z * (maxZ - minZ)) - draggedNormalZ -
                        (screenCamera ? screenCamera.transform.position.z : 0f);

                    newObjectPos = screenCamera.ScreenToWorldPoint(screenPixelPos) - draggedObjectOffset;
                    draggedObject.transform.position = Vector3.Lerp(draggedObject.transform.position, newObjectPos, dragSpeed * Time.deltaTime);

                    // check if the object (hand grip) was released
                    bool isReleased = lastHandEvent == InteractionManager.HandEventType.Release;

                    if (isReleased)
                    {
                        // restore the object's material and stop dragging the object
                        draggedObject.GetComponent<Renderer>().material = draggedObjectMaterial;

                        //if (usePause)
                        //{
                        //    // add gravity to the object
                        //    EngineObj.GetComponent<Engine>().ChangePauseState();
                        //}
                        if (useGravity)
                        {
                            // add gravity to the object
                            draggedObject.GetComponent<Rigidbody>().useGravity = true;
                            draggedObject.GetComponent<Rigidbody>().isKinematic = false;
                        }
                        draggedObject = null;
                    }
                }
            }

        }
    }


    void OnGUI()
    {
        if (infoGuiText != null && interactionManager != null && interactionManager.IsInteractionInited())
        {
            string sInfo = string.Empty;

            long userID = interactionManager.GetUserID();
            if (userID != 0)
            {
                if (draggedObject != null)
                    sInfo = "Dragging the " + draggedObject.name + " around.";
                else
                    sInfo = "Please grab and drag an object around.";
            }
            else
            {
                KinectManager kinectManager = KinectManager.Instance;

                if (kinectManager && kinectManager.IsInitialized())
                {
                    sInfo = "Waiting for Users...";
                }
                else
                {
                    sInfo = "Kinect is not initialized. Check the log for details.";
                }
            }

            infoGuiText.text = sInfo;
        }
    }


    // reset positions and rotations of the objects
    private void LoadHelp()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadGamePlay()
    {
        SceneManager.LoadScene(2);
    }
    private void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    //private void LoadPause()
    //{
    //    EngineObj.GetComponent<Engine>().ChangePauseState();
    //}

    public void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        if (!isHandInteracting || !interactionManager)
            return;
        if (userId != interactionManager.GetUserID())
            return;

        lastHandEvent = InteractionManager.HandEventType.Grip;
        //isLeftHandDrag = !isRightHand;
        screenNormalPos = handScreenPos;
    }

    public void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
    {
        if (!isHandInteracting || !interactionManager)
            return;
        if (userId != interactionManager.GetUserID())
            return;

        lastHandEvent = InteractionManager.HandEventType.Release;
        //isLeftHandDrag = !isRightHand;
        screenNormalPos = handScreenPos;
    }

    public bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
    {
        return true;
    }


}

