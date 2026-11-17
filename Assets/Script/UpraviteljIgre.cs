using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UpraviteljIgre : MonoBehaviour
{
    public static UpraviteljIgre instanca;

    public float vremenskoOgranicenje = 60f;
    public int minimalnoMrkvi = 5;

    public KonjKretanje konj;

    public TextMeshProUGUI tekstVremena;
    public TextMeshProUGUI tekstMrkvi;
    public TextMeshProUGUI tekstPoraza;

    public GameObject panelPobjeda;
    public GameObject panelPoraz;
    public GameObject panelPauza;

    public AudioSource zvucnikEfekata;
    public AudioClip zvukMrkve;
    public AudioClip zvukPobjede;
    public AudioClip zvukPoraza;

    private int skupljenoMrkvi = 0;
    private float preostaloVrijeme;
    private bool igraGotova = false;
    private bool pauzirano = false;

    void Awake()
    {
        instanca = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        preostaloVrijeme = vremenskoOgranicenje;

        panelPobjeda.SetActive(false);
        panelPoraz.SetActive(false);
        panelPauza.SetActive(false);

        AzurirajUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && igraGotova == false)
        {
            if (pauzirano == true)
            {
                Nastavi();
            }
            else
            {
                Pauziraj();
            }
        }

        if (igraGotova == true || pauzirano == true)
        {
            return;
        }

        preostaloVrijeme = preostaloVrijeme - Time.deltaTime;

        if (preostaloVrijeme <= 0)
        {
            preostaloVrijeme = 0;
            Poraz("Isteklo je vrijeme!");
        }

        AzurirajUI();
    }

    public void SkupiMrkvu()
    {
        if (igraGotova == true)
        {
            return;
        }

        skupljenoMrkvi = skupljenoMrkvi + 1;
        PustiZvuk(zvukMrkve);
        AzurirajUI();
    }

    public void StigaoDoCilja()
    {
        if (igraGotova == true)
        {
            return;
        }

        if (skupljenoMrkvi < minimalnoMrkvi)
        {
            Poraz("Premalo mrkvi! Treba barem " + minimalnoMrkvi + ".");
            return;
        }

        int sljedeciLevel = SceneManager.GetActiveScene().buildIndex + 1;
        int otkljucano = PlayerPrefs.GetInt("OtkljucanaRazina", 1);
        if (sljedeciLevel > otkljucano)
        {
            PlayerPrefs.SetInt("OtkljucanaRazina", sljedeciLevel);
        }

        igraGotova = true;
        ZaustaviIgru();
        PustiZvuk(zvukPobjede);
        panelPobjeda.SetActive(true);
    }

    public void OduzmiVrijeme(float sekunde)
    {
        if (igraGotova == true)
        {
            return;
        }

        preostaloVrijeme = preostaloVrijeme - sekunde;

        if (preostaloVrijeme < 0)
        {
            preostaloVrijeme = 0;
        }

        AzurirajUI();
    }

    public void Poraz(string razlog)
    {
        if (igraGotova == true)
        {
            return;
        }

        igraGotova = true;
        tekstPoraza.text = razlog;
        ZaustaviIgru();
        PustiZvuk(zvukPoraza);
        panelPoraz.SetActive(true);
    }

    public void Pauziraj()
    {
        pauzirano = true;
        konj.enabled = false;
        panelPauza.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Nastavi()
    {
        pauzirano = false;
        konj.enabled = true;
        panelPauza.SetActive(false);
        Time.timeScale = 1f;
    }

    void ZaustaviIgru()
    {
        konj.enabled = false;
        Time.timeScale = 0f;
    }

    void PustiZvuk(AudioClip zvuk)
    {
        if (zvucnikEfekata != null && zvuk != null)
        {
            zvucnikEfekata.PlayOneShot(zvuk);
        }
    }

    void AzurirajUI()
    {
        tekstMrkvi.text = "Mrkve: " + skupljenoMrkvi + " / " + minimalnoMrkvi;

        int minute = (int)(preostaloVrijeme / 60);
        int sekunde = (int)(preostaloVrijeme % 60);

        string tekstSekundi = sekunde.ToString();
        if (sekunde < 10)
        {
            tekstSekundi = "0" + sekunde;
        }

        tekstVremena.text = "Vrijeme: " + minute + ":" + tekstSekundi;

        if (preostaloVrijeme <= 10)
        {
            tekstVremena.color = Color.red;
        }
        else
        {
            tekstVremena.color = Color.white;
        }
    }

    public void Ponovi()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SljedecaRazina()
    {
        Time.timeScale = 1f;
        int sljedeciLevel = SceneManager.GetActiveScene().buildIndex + 1;

        if (sljedeciLevel < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sljedeciLevel);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void GlavniIzbornik()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}