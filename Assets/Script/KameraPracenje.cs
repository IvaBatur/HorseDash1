using UnityEngine;

public class KameraPracenje : MonoBehaviour
{
    public Transform meta; 
    public Vector3 odmak = new Vector3(0f, 3f, -6f); 
    public float brzinaPracenja = 5f;
    public float brzinaRotacije = 5f;

    void LateUpdate()
    {
        if (meta == null) return;

       
        Vector3 ciljanaPozicija = meta.TransformPoint(odmak);
        transform.position = Vector3.Lerp(transform.position, ciljanaPozicija, Time.deltaTime * brzinaPracenja);

        Quaternion ciljanaRotacija = Quaternion.LookRotation(meta.position + Vector3.up * 1.5f - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, ciljanaRotacija, Time.deltaTime * brzinaRotacije);
    }
}