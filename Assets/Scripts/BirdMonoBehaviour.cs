using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMonoBehaviour : MonoBehaviour {

    [SerializeField]
    private Transform slingLaunchObjTransform;

    private const string STATE_READY_TO_DRAG = "readyToShoot";
    private const string STATE_IS_DRAGGED = "isDragged";
    private const string STATE_IS_FLYING = "isFlying";

    private string state = STATE_READY_TO_DRAG;

    private Vector3 initPosition;
    private Quaternion initRotation;
    private Vector3 slingLaunchPoint;
    private bool isTouchedByController = false;

    private SteamVR_TrackedObject trackedObj;

    // Use this for initialization
    void Start() {
        Init();
        InitBird();
    }

    private void Init()
    { 
        initPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        initRotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        slingLaunchPoint = new Vector3(slingLaunchObjTransform.position.x, slingLaunchObjTransform.position.y, slingLaunchObjTransform.position.z);
	}

    private void InitBird()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.transform.position = new Vector3(initPosition.x, initPosition.y, initPosition.z);
        this.transform.rotation = new Quaternion(initRotation.x, initRotation.y, initRotation.z, initRotation.w);
    }
	
	// Update is called once per frame
	void Update () {
		switch(state)
        {
            case STATE_READY_TO_DRAG:
                CheckStartDrag();
                break;
            case STATE_IS_DRAGGED:
                UpdateDrag();
                break;
            case STATE_IS_FLYING:
                break;
            default:
                break;
        }
	}

    private void CheckStartDrag()
    {
        // SteamVR
        if (trackedObj != null && isTouchedByController)
        {
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                state = STATE_IS_DRAGGED;
            }
        }

        // Mouse controller
        /*
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    if (hitInfo.transform.name == "bird_lowPoly")
                    {
                        state = STATE_IS_DRAGGED;
                    }
            }
        }
        */
    }

    private void UpdateDrag()
    {
        // SteamVR controller
        this.transform.position = trackedObj.transform.position;

        if (trackedObj != null && isTouchedByController)
        {
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                Launch();
            }
        }
    

        // Mouse controller
        /*
        if (!Input.GetMouseButtonUp(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Physics.Raycast(ray.origin, ray.direction, out hitInfo);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                if (hitInfo.transform.name == "bird_lowPoly")
                {
                    this.transform.position = new Vector3(hitInfo.point.x+.5f, hitInfo.point.y - 1f);
                }            
        }
        else
        {
            Launch();
        }
        */
    }

    private void Launch()
    {
        Debug.Log("Launch");
        Rigidbody birdRigidBody = this.GetComponent<Rigidbody>();
        birdRigidBody.isKinematic = false;
        birdRigidBody.AddForceAtPosition(
            birdRigidBody.transform.right * Vector2.Distance(birdRigidBody.transform.position, slingLaunchPoint) * -300,
            slingLaunchPoint);

        state = STATE_IS_FLYING;

        StartCoroutine(WaitAndResetBird());
    }

    // Reset the bird in 5 seconds
    private IEnumerator WaitAndResetBird()
    {
        yield return new WaitForSeconds(5);
        InitBird();
        StopCoroutine(WaitAndResetBird());
    }

    private void OnTriggerEnter(Collider other)
    {
        trackedObj = other.GetComponent<SteamVR_TrackedObject>();
        if(trackedObj)
        {
            isTouchedByController = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        trackedObj = other.GetComponent<SteamVR_TrackedObject>();
        if (trackedObj)
        {
            isTouchedByController = false;
        }
    }
}
