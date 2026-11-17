using UnityEngine;

public class TrcanjeZvuk : MonoBehaviour
{
    public AudioSource zvucnik;
    public KonjKretanje konj;
    public float glasnoca = 0.6f;
    public float brzinaPrijelaza = 1.5f;

    void Start()
    {
        zvucnik.volume = 0f;
    }

    void Update()
    {
        bool trci = false;

        if (konj.enabled == true && Input.GetAxis("Vertical") > 0.1f)
        {
            trci = true;
        }

        float ciljanaGlasnoca = 0f;

        if (trci == true)
        {
            ciljanaGlasnoca = glasnoca;

            if (zvucnik.isPlaying == false)
            {
                zvucnik.Play();
            }
        }

        zvucnik.volume = Mathf.MoveTowards(zvucnik.volume, ciljanaGlasnoca, brzinaPrijelaza * Time.unscaledDeltaTime);

        if (trci == false && zvucnik.volume <= 0.01f && zvucnik.isPlaying == true)
        {
            zvucnik.Pause();
        }
    }
}