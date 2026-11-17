using UnityEngine;

public class IzbornikKonj : MonoBehaviour
{
    public float brzinaAnimacije = 1.5f;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (anim == null) return;
        anim.SetFloat("Vert", brzinaAnimacije);
        anim.SetFloat("State", brzinaAnimacije);
    }
}