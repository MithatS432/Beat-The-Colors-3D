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

    public Button powerActive;
    public Button powerDeactive;


    [Header("Character Settings")]
    public int score = 0;
    public int waveCount = 1;
    public int health = 3;
    public int ballCount = 5;
    public string[] ballColor = new string[] { "Red", "Blue", "Green", "Yellow", "Purple", "Orange" };
    public string currentBallColor = "Red";

    public int startingBallCount = 5;
    public int ballsPerWaveIncrease = 2;

    public bool isPowerUpActive = false;
    public float powerUpDuration = 5f;
    private bool isColorChanging = false;



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
        if (isPowerUpActive)
        {
            score += 15;
            ballCount--;
            AudioSource.PlayClipAtPoint(ballSound, transform.position);

            if (correctBallEffect != null)
            {
                GameObject effect = Instantiate(correctBallEffect, Camera.main.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (ballCount <= 0)
            {
                waveCount++;
                ballCount = startingBallCount + (waveCount - 1) * ballsPerWaveIncrease;

                string newColor;
                do
                {
                    newColor = ballColor[Random.Range(0, ballColor.Length)];
                } while (newColor == currentBallColor);

                currentBallColor = newColor;

                if (waveCount % 5 == 0)
                {
                    StartCoroutine(PowerUpRoutine());
                }
            }

            UpdateUI();
            return;
        }

        if (collectedColor == currentBallColor)
        {
            score += 15;
            ballCount--;
            AudioSource.PlayClipAtPoint(ballSound, transform.position);

            if (correctBallEffect != null)
            {
                GameObject effect = Instantiate(correctBallEffect, Camera.main.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (ballCount <= 0)
            {
                waveCount++;
                ballCount = startingBallCount + (waveCount - 1) * ballsPerWaveIncrease;

                string newColor;
                do
                {
                    newColor = ballColor[Random.Range(0, ballColor.Length)];
                } while (newColor == currentBallColor);

                currentBallColor = newColor;

                if (waveCount % 5 == 0)
                {
                    StartCoroutine(PowerUpRoutine());
                }
            }
            if (waveCount >= 7 && !isColorChanging)
            {
                StartCoroutine(ChangeColorPeriodically());
            }
        }
        else
        {
            TakeDamage(1);

            if (wrongBallEffect != null)
            {
                GameObject wrongEffect = Instantiate(wrongBallEffect, Camera.main.transform.position, Quaternion.identity);
                Destroy(wrongEffect, 2f);
            }
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
        ballCountText.text = ballCount.ToString();
        waveText.text = "Wave:" + waveCount.ToString();
        scoreText.text = "Score:" + score.ToString();
        healthText.text = health.ToString();
        ballColorText.text = currentBallColor;
        switch (currentBallColor)
        {
            case "Red":
                ballColorText.color = Color.red;
                break;
            case "Blue":
                ballColorText.color = Color.blue;
                break;
            case "Green":
                ballColorText.color = Color.green;
                break;
            case "Yellow":
                ballColorText.color = Color.yellow;
                break;
            case "Purple":
                ballColorText.color = new Color(0.5f, 0f, 0.5f);
                break;
            case "Orange":
                ballColorText.color = new Color(1f, 0.65f, 0f);
                break;
            default:
                ballColorText.color = Color.white;
                break;
        }
        if (waveCount >= 3)
        {
            switch (currentBallColor)
            {
                case "Red":
                    ballColorText.color = Color.blue;
                    break;
                case "Blue":
                    ballColorText.color = new Color(1f, 0.65f, 0f);
                    break;
                case "Green":
                    ballColorText.color = Color.yellow;
                    break;
                case "Yellow":
                    ballColorText.color = new Color(0.5f, 0f, 0.5f);
                    break;
                case "Purple":
                    ballColorText.color = Color.red;
                    break;
                case "Orange":
                    ballColorText.color = Color.green;
                    break;
                default:
                    ballColorText.color = Color.white;
                    break;
            }
        }
    }
    private IEnumerator PowerUpRoutine()
    {
        isPowerUpActive = true;
        powerActive.gameObject.SetActive(true);
        Invoke("CheckPowerText", 0.5f);
        yield return new WaitForSeconds(powerUpDuration);
        isPowerUpActive = false;
        powerDeactive.gameObject.SetActive(true);
        Invoke("CheckDepowerText", 0.5f);

    }
    void CheckPowerText()
    {
        powerActive.gameObject.SetActive(false);
    }
    void CheckDepowerText()
    {
        powerDeactive.gameObject.SetActive(false);
    }
    private IEnumerator ChangeColorPeriodically()
    {
        isColorChanging = true;
        while (true)
        {
            string newColor;
            do
            {
                newColor = ballColor[Random.Range(0, ballColor.Length)];
            } while (newColor == currentBallColor);

            currentBallColor = newColor;
            UpdateUI();

            yield return new WaitForSeconds(5f);
        }
    }
}
