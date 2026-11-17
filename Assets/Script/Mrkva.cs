using UnityEngine;

public class Mrkva : MonoBehaviour
{
    public float brzinaRotacije = 100f;

    void Update()
    {
        transform.Rotate(0, brzinaRotacije * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<KonjKretanje>() != null)
        {
         
            if (UpraviteljIgre.instanca != null)
            {
                UpraviteljIgre.instanca.SkupiMrkvu();
            }

            Destroy(gameObject); 
        }
    }
}