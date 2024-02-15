using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Telemetry_Util : MonoBehaviour
{

    string UUID;
    System.DateTime dt = System.DateTime.Now;

    //Singleton 
    private static Telemetry_Util instance;
    // Read-only public access
    public static Telemetry_Util Instance => instance;

    void Awake()
    {

        // Does another instance already exist?
        if (instance && instance != this)
        {
            // Destroy myself
            Destroy(gameObject);
            return;
        }

        // Otherwise store my reference and make me DontDestroyOnLoad
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator SaveAID(string aid)
    {
        Debug.Log("starting save aid post request");
        var cert = new CertificateValidator();

        WWWForm form = new WWWForm();
        form.AddField("data", aid);
        UnityWebRequest www = UnityWebRequest.Post("https://inc0293516.cs.ualberta.ca/save_aid.php", form);
        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("AID Saved as " + aid);
        }
    }

    public IEnumerator PostData(string msg)
    {
        var cert = new CertificateValidator();

        WWWForm form = new WWWForm();
        form.AddField("data", msg + "; " + dt.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://inc0293516.cs.ualberta.ca/save_unity_data.php", form);
        //NEEDED TO AVOID CERTIFICATE VALIDATION ERROR
        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
