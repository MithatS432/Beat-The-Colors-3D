using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AudioSource audioSource;
    public float speed = 20f;
    public string ballColor;
    public bool isTrueColor = false;

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
            if (isTrueColor)
                AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);

            Destroy(gameObject);
        }


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
