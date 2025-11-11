using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AudioSource audioSource;
    public float speed = 20f;
    public string ballColor;
    public bool isTrueColor = false; // Castle'a ulaştığında ses çalacak

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Castle"))
        {
            if (isTrueColor && audioSource != null)
                audioSource.Play();

            Destroy(gameObject);
        }

        // Enemy.cs

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                if (ballColor == player.currentBallColor)
                {
                    player.CollectBall(ballColor);
                }
                else
                {
                    player.TakeDamage(1);
                }

                Destroy(gameObject);
            }
        }
    }
}
