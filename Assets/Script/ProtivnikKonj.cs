using UnityEngine;

public class ProtivnikKonj : MonoBehaviour
{
    public float traka = -4f;
    public float brzina = 10.5f;
    public float skokVisina = 1.8f;
    public float skokTrajanje = 0.6f;
    public Color boja = Color.white;
    public AudioSource zvucnik;

    public Vector3[] tocke =
    {
        new Vector3(0f, 0.5f, -90f),
        new Vector3(0f, 0.5f, 92.45f),
        new Vector3(0f, 11.47f, 133.5f),
        new Vector3(0f, 11.47f, 232.5f),
        new Vector3(0f, 0.5f, 273.55f),
        new Vector3(0f, 0.5f, 305f)
    };

    private Animator anim;
    private int trenutnaTocka = 1;
    private bool krenuo = false;
    private bool gotov = false;
    private float skokTimer = 0f;
    private Vector3 pozicijaNaStazi;

    void Start()
    {
        anim = GetComponent<Animator>();

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = boja;
        }

        traka = transform.position.x;
        pozicijaNaStazi = new Vector3(traka, tocke[0].y, transform.position.z);
        transform.position = pozicijaNaStazi;
        OkreniPrema(Tocka(1));
    }

    void Update()
    {
        if (zvucnik != null)
        {
            if (Time.timeScale == 0f)
            {
                zvucnik.Pause();
            }
            else if (krenuo == true && gotov == false && zvucnik.isPlaying == false)
            {
                zvucnik.Play();
            }
        }

        if (krenuo == false)
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                krenuo = true;
            }
            else
            {
                PostaviAnimaciju(0f);
                return;
            }
        }

        if (gotov == true)
        {
            PostaviAnimaciju(0f);
            return;
        }

        Vector3 cilj = Tocka(trenutnaTocka);
        pozicijaNaStazi = Vector3.MoveTowards(pozicijaNaStazi, cilj, brzina * Time.deltaTime);
        OkreniPrema(cilj);

        if (Vector3.Distance(pozicijaNaStazi, cilj) < 0.05f)
        {
            trenutnaTocka = trenutnaTocka + 1;

            if (trenutnaTocka >= tocke.Length)
            {
                gotov = true;

                if (zvucnik != null)
                {
                    zvucnik.Stop();
                }
            }
        }

        if (skokTimer <= 0f && PreprekaIspred() == true)
        {
            skokTimer = skokTrajanje;
        }

        float visina = 0f;

        if (skokTimer > 0f)
        {
            skokTimer = skokTimer - Time.deltaTime;
            float t = 1f - Mathf.Clamp01(skokTimer / skokTrajanje);
            visina = Mathf.Sin(t * Mathf.PI) * skokVisina;
        }

        transform.position = pozicijaNaStazi + Vector3.up * visina;
        PostaviAnimaciju(1.5f);
    }

    Vector3 Tocka(int i)
    {
        Vector3 p = tocke[i];
        p.x = traka;
        return p;
    }

    void OkreniPrema(Vector3 cilj)
    {
        Vector3 smjer = cilj - pozicijaNaStazi;
        smjer.y = 0f;

        if (smjer.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(smjer);
        }
    }

    bool PreprekaIspred()
    {
        RaycastHit hit;
        Vector3 pocetak = pozicijaNaStazi + Vector3.up * 0.8f;

        if (Physics.Raycast(pocetak, transform.forward, out hit, 3f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponentInParent<Prepreka>() != null)
            {
                return true;
            }
        }

        return false;
    }

    void PostaviAnimaciju(float vrijednost)
    {
        if (anim == null)
        {
            return;
        }

        anim.SetFloat("Vert", vrijednost, 0.1f, Time.deltaTime);
        anim.SetFloat("State", vrijednost, 0.1f, Time.deltaTime);
    }
}