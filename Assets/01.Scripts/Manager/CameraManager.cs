using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Transform firstPersonHand;
    
    public Transform FirstPersonHand => firstPersonHand; 

    private Camera cam;
    
    [SerializeField]private float sensitivity;
    private float pitch;
    private float xRotation = 0;
    
    private Vector3 currentTargetCamPos = Vector3.zero;
    private Transform currentTarget;
    private float cameraSpeed = 5f;
    
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(this);
        }
        cam = GetComponent<Camera>();
        cam.cullingMask &= ~LayerMask.GetMask("LocalBody");
    }

    private void LateUpdate()
    {
        if (currentTarget == null) return;
        
        UpdateCameraPos();
        
        LookPitch();
        
    }
    private void UpdateCameraPos()
    {
        Vector3 currentCamPos = cam.transform.position;
        cam.transform.position = Vector3.Lerp(currentCamPos, currentTargetCamPos + currentTarget.position,
            Time.deltaTime*cameraSpeed);
    }

    public void SetViewTarget(Transform target)
    {
        currentTarget = target;
    }

    private void LookPitch()
    {
        xRotation -= (pitch * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, currentTarget.eulerAngles.y, 0f);
    }

    public void AddPitch(float _pitch)
    {
        pitch = _pitch;
    }
    
    public void UpdateTargetPos(Vector3 _target)
    {
        currentTargetCamPos = _target;
    }
}
