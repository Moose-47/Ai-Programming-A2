using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    private bool triggered = false;
    public ParticleSystem ps;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            if (ps != null)
            {
                ps.Play();
            }
            Debug.Log("Checkpoint reached, saving game...");

            GameManager.Instance.SaveGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = false;
            Debug.Log("Player exited checkpoint trigger.");
        }
    }
}
