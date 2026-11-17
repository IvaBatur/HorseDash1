using UnityEngine;
using UnityEngine.SceneManagement;

public class GlavniIzbornik : MonoBehaviour
{
    public GameObject panelUpute;

    void Start()
    {
        Time.timeScale = 1f;
        if (panelUpute != null) panelUpute.SetActive(false);
    }

    public void Igraj()
    {
        SceneManager.LoadScene(1);
    }

    public void PrikaziUpute()
    {
        panelUpute.SetActive(true);
    }

    public void SakrijUpute()
    {
        panelUpute.SetActive(false);
    }

    public void Izlaz()
    {
        Debug.Log("Izlaz iz igre");
        Application.Quit();
    }
}