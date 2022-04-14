using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                //target object
    public float smoothSpeed =0.125f;       
    public Vector3 offset;                  //distance between player and camera
    public Quaternion rotationOffset;
    public int playerNumber;                // Used to identify which tank belongs to which player; This is set by this tank's manager

    private string HorizontalAxis;
    private TankMovement m_Movement;        // Reference to tank's movement script

    private void FixedUpdate() {
        if (target)
        {
            Rotate();
            Vector3 desiredPosition = target.transform.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            Quaternion desiredRotation = target.rotation * rotationOffset;
            transform.rotation = desiredRotation;
        }
        
    }

    void Rotate() {
        HorizontalAxis = "Horizontal" + playerNumber;
        offset = Quaternion.AngleAxis(Input.GetAxis(HorizontalAxis) * 3.6f, Vector3.up) * offset;
    }
}
