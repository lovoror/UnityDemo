using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Camera mCamera;
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition -= transform.right;
        }
        if (Input.GetMouseButton(0))
        {
            float x = Input.GetAxisRaw("Mouse Y");
            float y = Input.GetAxisRaw("Mouse X");
            transform.transform.localEulerAngles += new Vector3(-x, y, 0);
        }
        if (mCamera == null)
        {
            mCamera = Camera.main;
        }
        else
        {
            mCamera.transform.localPosition = transform.localPosition;
            mCamera.transform.localEulerAngles = transform.localEulerAngles;
        }
    }
}
