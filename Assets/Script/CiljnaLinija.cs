using UnityEngine;

public class CiljnaLinija : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<KonjKretanje>() != null && UpraviteljIgre.instanca != null)
        {
            UpraviteljIgre.instanca.StigaoDoCilja();
        }
    }
}