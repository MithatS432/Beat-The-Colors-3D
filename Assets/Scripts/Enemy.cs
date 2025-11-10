using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AudioSource audioSource;
    private float speed = 20f;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.Self);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Castle"))
        {
            Destroy(gameObject);
        }
    }
}
