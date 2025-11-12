using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    private Renderer rend;
    public Color damageColor = Color.red;
    private Color normalColor;


    [Header("Camera Settings")]
    public Camera playerCamera;
    public Camera secondaryCamera;
    public bool isUsingSecondaryCamera = false;

    [Header("UI Settings")]
    public Button pauseButton;
    public Button resumeButton;
    public Button exitButton;


    public TMP_Text scoreText;
    public TMP_Text waveText;
    public TMP_Text healthText;
    public TMP_Text ballCountText;
    public TMP_Text ballColorText;

    [Header("Character Settings")]
    public int score = 0;
    public int waveCount = 1;
    public int health = 3;
    public int ballCount = 5;
    public string[] ballColor = new string[] { "Red", "Blue", "Green", "Yellow", "Purple", "Orange" };
    public string currentBallColor = "Red";

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    private bool isGrounded;
    private float rotateSpeed = 100f;
    public AudioClip ballSound;
    public AudioClip wrongBallSound;
    [Header("Ball Collect Effects")]
    public GameObject correctBallEffect;
    public GameObject wrongBallEffect;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        normalColor = rend.material.color;
        rend.material = new Material(rend.material);
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ToggleCameras();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CollectBall(string collectedColor)
    {
        if (collectedColor == currentBallColor)
        {
            score += 15;

            ballCount--;
            AudioSource.PlayClipAtPoint(ballSound, transform.position);

            if (correctBallEffect != null)
            {
                GameObject effect = Instantiate(correctBallEffect, transform.position + Vector3.up, Quaternion.identity);
                Debug.Log("Correct effect spawned at: " + effect.transform.position);
                Destroy(effect, 2f);
            }
            else
            {
                Debug.LogWarning("correctBallEffect prefab'i atanmamış!");
            }

            if (ballCount <= 0)
            {
                waveCount++;
                ballCount += 2;

                string newColor;
                do
                {
                    newColor = ballColor[Random.Range(0, ballColor.Length)];
                } while (newColor == currentBallColor);

                currentBallColor = newColor;
            }
        }
        else
        {
            if (wrongBallEffect != null)
                AudioSource.PlayClipAtPoint(wrongBallSound, transform.position);

            if (wrongBallEffect != null)
            {
                GameObject wrongEffect = Instantiate(wrongBallEffect, transform.position + Vector3.up, Quaternion.identity);
                Debug.Log("Wrong effect spawned at: " + wrongEffect.transform.position);
                Destroy(wrongEffect, 2f);
            }
            else
            {
                Debug.LogWarning("wrongBallEffect prefab'i atanmamış!");
            }

            TakeDamage(1);
        }

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashDamageColor());
        UpdateUI();
    }
    private IEnumerator FlashDamageColor()
    {
        rend.material.color = damageColor;
        yield return new WaitForSeconds(0.2f);
        rend.material.color = normalColor;
    }



    public void UpdateUI()
    {
        ballCountText.text = "Balls:" + ballCount.ToString();
        waveText.text = "Wave:" + waveCount.ToString();
        scoreText.text = "Score:" + score.ToString();
        healthText.text = health.ToString();
        ballColorText.text = currentBallColor;
    }
}
