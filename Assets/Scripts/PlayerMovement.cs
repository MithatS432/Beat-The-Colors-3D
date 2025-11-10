using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    private AudioSource audioSource;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public Camera secondaryCamera;
    public bool isUsingSecondaryCamera = false;

    [Header("UI Settings")]
    public Button pauseButton;
    public Button continueButton;
    public Button exitButton;

    [Header("Character Settings")]
    private int score = 0;
    public int waveCount = 1;
    private int health = 3;
    public int ballCount = 5;
    public string ballColor = "Red";
    public TMP_Text scoreText;
    public TMP_Text waveText;
    public TMP_Text healthText;
    public TMP_Text ballCountText;
    public TMP_Text ballColorText;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    private bool isGrounded;
    private float rotateSpeed = 100f;
    public AudioClip ballSound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        playerCamera.enabled = !isUsingSecondaryCamera;
        secondaryCamera.enabled = isUsingSecondaryCamera;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(x, 0f, 0f).normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        transform.Rotate(0f, x * rotateSpeed, 0f);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void ToggleCameras()
    {
        isUsingSecondaryCamera = !isUsingSecondaryCamera;
        playerCamera.enabled = !isUsingSecondaryCamera;
        secondaryCamera.enabled = isUsingSecondaryCamera;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
