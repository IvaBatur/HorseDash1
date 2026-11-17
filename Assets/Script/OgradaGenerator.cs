using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OgradaGenerator : MonoBehaviour
{
    [Header("Model ograde")]
    public GameObject segmentOgrade;
    public Material materijal;
    public Vector3 skala = Vector3.one;
    public float dodatnaRotacijaY = 90f;
    public float preklapanje = 0.05f;
    public bool dodajCollider = true;

    [Header("Put ograde (lijeva strana staze)")]
    public Vector3[] tocke =
    {
        new Vector3(-7.2f, 0.51f, -100f),
        new Vector3(-7.2f, 0.51f, 92.45f),
        new Vector3(-7.2f, 11.47f, 133.5f),
        new Vector3(-7.2f, 11.47f, 233f)
    };
    public bool iDesnaStrana = true;

    [ContextMenu("Generiraj ogradu")]
    public void Generiraj()
    {
        if (segmentOgrade == null)
        {
            Debug.LogWarning("OgradaGenerator: povuci model ograde u polje 'Segment Ograde'.");
            return;
        }

        Obrisi();

        Vector3 centar; float dnoY;
        float duljina = IzmjeriSegment(out centar, out dnoY);
        if (duljina < 0.5f)
        {
            Debug.LogWarning("OgradaGenerator: komad je prekratak (" + duljina + " m). Promijeni Dodatna Rotacija Y (90 ili 0).");
            return;
        }

        GenerirajStranu(1f, duljina, centar, dnoY);
        if (iDesnaStrana) GenerirajStranu(-1f, duljina, centar, dnoY);

        Debug.Log("OgradaGenerator: postavljeno " + transform.childCount + " objekata ograde.");
    }

    [ContextMenu("Obrisi ogradu")]
    public void Obrisi()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(transform.GetChild(i).gameObject);
#else
            DestroyImmediate(transform.GetChild(i).gameObject);
#endif
        }
    }

    float IzmjeriSegment(out Vector3 centar, out float dnoY)
    {
        GameObject temp = Instantiate(segmentOgrade, Vector3.zero, Quaternion.Euler(0f, dodatnaRotacijaY, 0f));
        temp.transform.localScale = skala;

        Renderer[] rendereri = temp.GetComponentsInChildren<Renderer>();
        if (rendereri.Length == 0) { DestroyImmediate(temp); centar = Vector3.zero; dnoY = 0f; return 0f; }

        Bounds b = rendereri[0].bounds;
        foreach (Renderer r in rendereri) b.Encapsulate(r.bounds);

        DestroyImmediate(temp);
        centar = b.center;
        dnoY = b.min.y;
        return b.size.z;
    }

    void GenerirajStranu(float strana, float duljina, Vector3 centar, float dnoY)
    {
        float korak = Mathf.Max(0.1f, duljina - preklapanje);

        for (int t = 0; t < tocke.Length - 1; t++)
        {
            Vector3 a = tocke[t];
            Vector3 b = tocke[t + 1];
            a.x *= strana;
            b.x *= strana;

            Vector3 smjer = b - a;
            float dionica = smjer.magnitude;
            if (dionica < 0.01f) continue;
            smjer /= dionica;

            Quaternion okret = Quaternion.LookRotation(smjer);
            Quaternion rotacija = okret * Quaternion.Euler(0f, dodatnaRotacijaY, 0f);
            int broj = Mathf.CeilToInt(dionica / korak);

            for (int i = 0; i < broj; i++)
            {
                Vector3 cilj = a + smjer * (i * korak + duljina * 0.5f);
                Vector3 pozicija = cilj - okret * new Vector3(centar.x, dnoY, centar.z);
                NapraviKomad(pozicija, rotacija);
            }

            if (dodajCollider) NapraviZid(a, b, okret, dionica, strana);
        }
    }

    void NapraviKomad(Vector3 pozicija, Quaternion rotacija)
    {
        GameObject komad = null;
#if UNITY_EDITOR
        komad = PrefabUtility.InstantiatePrefab(segmentOgrade, transform) as GameObject;
#endif
        if (komad == null) komad = Instantiate(segmentOgrade, transform);

        komad.transform.SetPositionAndRotation(pozicija, rotacija);
        komad.transform.localScale = skala;

        if (materijal != null)
        {
            foreach (Renderer r in komad.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = new Material[r.sharedMaterials.Length];
                for (int m = 0; m < mats.Length; m++) mats[m] = materijal;
                r.sharedMaterials = mats;
            }
        }

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(komad, "Generiraj ogradu");
#endif
    }

    void NapraviZid(Vector3 a, Vector3 b, Quaternion okret, float dionica, float strana)
    {
        GameObject zid = new GameObject("NevidljiviZid");
        zid.transform.SetParent(transform);

        Vector3 vani = Vector3.left * strana;
        zid.transform.position = (a + b) * 0.5f + vani * 0.5f + okret * Vector3.up * 5f;
        zid.transform.rotation = okret;

        BoxCollider bc = zid.AddComponent<BoxCollider>();
        bc.size = new Vector3(1f, 10f, dionica + 1f);

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(zid, "Generiraj ogradu");
#endif
    }
}