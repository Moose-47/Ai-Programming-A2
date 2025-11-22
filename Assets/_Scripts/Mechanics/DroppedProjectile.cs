using UnityEngine;

public class DroppedProjectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerHealth -= 1;
            Debug.Log(GameManager.Instance.PlayerHealth);
            Destroy(gameObject);
        }
    }
}
