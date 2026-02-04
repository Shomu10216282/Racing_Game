using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //ADD SOUNDS
    [SerializeField] private string thisLevel;
    //move settings
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float topSpeed = 5f;
    [SerializeField] private float boostTopSpeed = 200f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float turnSpeed = 150f;

    //boost
    [SerializeField] private float maxBoost = 100f;
    [SerializeField] private float boostDrainRate = 20f;
    [SerializeField] private Slider boostSlider;

    private float currentBoost = 100f;
    private bool isBoosting = false;

    //cam settings
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;
    [SerializeField] private float cameraHeight = 1.5f;
    [SerializeField] private float cameraCollisionOffset = 0.2f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float boostFOV = 75f;
    [SerializeField] private float fovTransitionSpeed = 5f;

    //UI
    [SerializeField] private float maxHP = 100f;
    private float HP = 100f;
    [SerializeField] private Slider HPSlider;

    [SerializeField] private TextMeshProUGUI speedUI;
    [SerializeField] private Canvas gameOverScreen;
    [SerializeField] private Canvas gameWinScreen;

    [SerializeField] private LayerMask hazardLayer;

    private CharacterController controller;
    private Camera mainCamera;
    private Transform cameraTransform;

    private Vector3 velocity;
    private float currentSpeed = 0f;

    // cam
    private float horizontalAngle = 0f;
    private float verticalAngle = 20f;

    private bool gameWon = false;

    // functions
    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        boost();
        speedUI.text = "Speed: " + currentSpeed.ToString();
        HPSlider.value = HP;
    }

    void LateUpdate()
    {
        moveCamera();
        camFOV();
    }

    void camFOV() //making camera fov change on boost
    {
        if (mainCamera == null) return;
        float targetFOV = Input.GetKey(KeyCode.LeftShift) ? boostFOV : normalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
    }

    void boost()
    {
        isBoosting = Input.GetKey(KeyCode.LeftShift) && currentSpeed > 0 && currentBoost > 0;
        if (isBoosting)
        {
            currentBoost -= boostDrainRate * Time.deltaTime;
            currentBoost = Mathf.Max(currentBoost, 0f);
        }

        if (boostSlider != null)
        {
            boostSlider.value = currentBoost;
        }
    }

    public void refillBoost(float amount)
    {
        currentBoost += amount;
        currentBoost = Mathf.Min(currentBoost, maxBoost);
    }

    void moveCamera()
    {
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        horizontalAngle += mouseX;
        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

        Vector3 targetPosition = cameraTarget != null ? cameraTarget.position : transform.position + Vector3.up * cameraHeight;

        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);

        RaycastHit hit;
        Vector3 desiredPosition = targetPosition + offset;
        Vector3 direction = desiredPosition - targetPosition;

        if (Physics.SphereCast(targetPosition, cameraCollisionOffset, direction.normalized, out hit, cameraDistance))
        {
            float distance = Mathf.Max(hit.distance - cameraCollisionOffset, 0.5f);
            cameraTransform.position = targetPosition + direction.normalized * distance;
        }
        else
        {
            cameraTransform.position = desiredPosition;
        }

        cameraTransform.LookAt(targetPosition);
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float targetTopSpeed = (isBoosting && currentBoost > 0) ? boostTopSpeed : topSpeed;

        if (vertical != 0) //I HAVE MEGA AIDS
        {
            float targetSpeed;
            if (vertical > 0)
            {
                targetSpeed = targetTopSpeed;
            }
            else
            {
                targetSpeed = -targetTopSpeed;
            }
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        if (Mathf.Abs(currentSpeed) > 0.1f && horizontal != 0)
        {
            float turn = horizontal * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, turn, 0f);
        }

        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(movement);

        velocity.y += gravity * Time.deltaTime; //gravity
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.R)) { SceneManager.LoadScene(thisLevel); Time.timeScale = 1f; }
        if (Input.GetKey(KeyCode.E) && gameWon) { SceneManager.LoadScene("MainMenu"); Time.timeScale = 1f; }//mmmm
    }

    //COLLISION

    void OnCollisionEnter(Collision collision) //fallback check
    {
        if (((1 << collision.gameObject.layer) & hazardLayer) != 0)
        {
            Pause();
            gameOverScreen.gameObject.SetActive(true);
            Debug.Log("collisionenter");
        }
    }
    void OnControllerColliderHit(ControllerColliderHit hit) //fallback check cuz i love unity <3
    {
        if (((1 << hit.gameObject.layer) & hazardLayer) != 0)
        {
            Pause();
            gameOverScreen.gameObject.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hazardLayer) != 0)
        {
            if (isBoosting == false)
            {
                HP -= 25;
                if (HP <= 0)
                {
                    Pause();
                    gameOverScreen.gameObject.SetActive(true); //convert to a loss function when u can bruh
                }
            }
            other.gameObject.SetActive(false);
        }
    }

    public void GameWon()
    {
        Pause();
        gameWinScreen.gameObject.SetActive(true);
        gameWon = true;
    }

    void Pause()
    {
        Time.timeScale = 0f;
    }

    void die()
    {

    }
}
