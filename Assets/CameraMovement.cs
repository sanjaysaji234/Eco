using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 5f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.back * cameraSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Moving"+transform.position.x);
            transform.position += Vector3.forward * cameraSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.right * cameraSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
        }
    }
}
