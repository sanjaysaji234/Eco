using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 5f;
    UiInteractions uiInteractions;
    Camera cam;
    [SerializeField] private Slider cameraSpanSlider;
    [SerializeField]private float scrollSensitivity=2f;
    private void Start()
    {
        cam= GetComponent<Camera>();
        uiInteractions = FindAnyObjectByType<UiInteractions>();
        cam.fieldOfView = uiInteractions.cameraSpan * 10;
    }
    private void Update()
    {

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cameraSpanSlider.value -= scroll * scrollSensitivity;
        }
        // 1. Get input from standard Unity axes (returns -1, 0, or 1)
        // A/Left Arrow = -1, D/Right Arrow = 1
        float moveX = Input.GetAxisRaw("Horizontal");

        // S/Down Arrow = -1, W/Up Arrow = 1
        float moveZ = Input.GetAxisRaw("Vertical");

        // 2. Combine inputs into a single direction vector
        // Note: We use .normalized so diagonal movement isn't faster than moving straight
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // 3. Apply the movement to the transform
        transform.position += moveDirection * cameraSpeed * Time.unscaledDeltaTime;

        cam.fieldOfView = uiInteractions.cameraSpan*10;
    }
}