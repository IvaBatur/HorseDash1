using UnityEngine;

public class KonjKretanje : MonoBehaviour
{
    public float brzina = 8f;
    public float maksimalnaBrzina = 25f;
    public float ubrzanje = 12f;
    public float brzinaOkretanja = 60f;
    public float silaSkoka = 7f;

    [Header("Provjera tla")]
    public float visinaProvjere = 0.5f;
    public float duljinaProvjere = 1.0f;

    [Header("Zvuk")]
    public AudioSource zvucnikEfekata;
    public AudioClip zvukSkoka;

    [Header("Postavke Realisticnosti Glave")]
    public Transform kostGlave;
    public float jacinaNjihanjaGlave = 2.5f;
    public float brzinaNjihanja = 3f;

    private Rigidbody rb;
    private Animator anim;
    private bool naZemlji = true;
    private float tajmerGlave = 0f;
    private float trenutnaBrzina;
    private float vrijemeSkoka = -10f;
    private float zadnjiDodirTla = -10f;
    private float zadnjaKazna = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        trenutnaBrzina = brzina;

        if (kostGlave == null && anim != null)
        {
            Transform[] sveKosti = GetComponentsInChildren<Transform>();
            foreach (Transform t in sveKosti)
            {
                if (t.name.ToLower().Contains("head") || t.name.ToLower().Contains("neck"))
                {
                    kostGlave = t;
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && naZemlji == true)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, silaSkoka, rb.linearVelocity.z);
            naZemlji = false;
            vrijemeSkoka = Time.time;

            if (zvucnikEfekata != null && zvukSkoka != null)
            {
                zvucnikEfekata.PlayOneShot(zvukSkoka);
            }
        }
    }

    void FixedUpdate()
    {
        ProvjeriTlo();

        float naprijed = Input.GetAxis("Vertical");
        float okret = Input.GetAxis("Horizontal");

        if (naprijed > 0 && naZemlji == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.8f, transform.forward, out hit, 1.2f,
                                ~0, QueryTriggerInteraction.Ignore))
            {
                if (JeLiPodloga(hit.collider) == false)
                {
                    naprijed = 0f;
                    rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

                    Prepreka p = hit.collider.GetComponentInParent<Prepreka>();
                    if (p != null && Time.time - zadnjaKazna > 2f && UpraviteljIgre.instanca != null)
                    {
                        zadnjaKazna = Time.time;
                        UpraviteljIgre.instanca.OduzmiVrijeme(p.kaznaSekundi);
                    }
                }
            }
        }

        if (naprijed > 0.1f)
        {
            trenutnaBrzina = trenutnaBrzina + ubrzanje * Time.fixedDeltaTime;
            trenutnaBrzina = Mathf.Min(trenutnaBrzina, maksimalnaBrzina);
        }
        else
        {
            trenutnaBrzina = brzina;
        }

        transform.Rotate(0, okret * brzinaOkretanja * Time.fixedDeltaTime, 0);

        Vector3 kretanje = transform.forward * naprijed * trenutnaBrzina * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + kretanje);

        if (anim != null)
        {
            float unos = Mathf.Abs(naprijed);
            float ciljanaVrijednost = 0f;
            if (unos > 0.1f)
            {
                ciljanaVrijednost = unos * 1.5f;
            }

            anim.SetFloat("Vert", ciljanaVrijednost, 0.1f, Time.fixedDeltaTime);
            anim.SetFloat("State", ciljanaVrijednost, 0.1f, Time.fixedDeltaTime);
        }
    }

    void ProvjeriTlo()
    {
        if (Time.time - vrijemeSkoka < 0.25f)
        {
            naZemlji = false;
            return;
        }

        Vector3 pocetak = transform.position + Vector3.up * visinaProvjere;
        bool zrakaPogodila = Physics.Raycast(pocetak, Vector3.down, duljinaProvjere, ~0, QueryTriggerInteraction.Ignore);
        naZemlji = zrakaPogodila || Time.time - zadnjiDodirTla < 0.15f;
    }

    bool JeLiPodloga(Collider c)
    {
        if (c is TerrainCollider) return true;
        if (c.CompareTag("Staza")) return true;
        return c.gameObject.name.ToLower().Contains("staza");
    }

    void LateUpdate()
    {
        if (kostGlave != null)
        {
            float naprijed = Input.GetAxis("Vertical");
            float okret = Input.GetAxis("Horizontal");

            float klimanjeX = 0f;
            float rotacijaY = 0f;

            if (Mathf.Abs(naprijed) > 0.1f)
            {
                tajmerGlave = tajmerGlave + Time.deltaTime * brzinaNjihanja;
                klimanjeX = Mathf.Sin(tajmerGlave) * jacinaNjihanjaGlave;
            }

            if (Mathf.Abs(okret) > 0.1f)
            {
                rotacijaY = okret * 12f;
            }

            Quaternion ciljanaRotacija = Quaternion.Euler(klimanjeX, rotacijaY, 0);
            kostGlave.localRotation = Quaternion.Slerp(kostGlave.localRotation, kostGlave.localRotation * ciljanaRotacija, Time.deltaTime * 10f);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                zadnjiDodirTla = Time.time;
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (naZemlji == true)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Vector3 pocetak = transform.position + Vector3.up * visinaProvjere;
        Gizmos.DrawLine(pocetak, pocetak + Vector3.down * duljinaProvjere);
    }
}