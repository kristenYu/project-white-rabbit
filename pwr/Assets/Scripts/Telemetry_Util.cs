using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Telemetry_Util : MonoBehaviour
{

    string UUID;
    System.DateTime dt = System.DateTime.Now;
    
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
        Debug.Log("Starting Post Request");
        var cert = new CertificateValidator();

        WWWForm form = new WWWForm();
        form.AddField("data", msg + "; " + dt.ToString() + "\n");

        UnityWebRequest www = UnityWebRequest.Post("https://inc0293516.cs.ualberta.ca/save_unity_data.php", form);
        //NEEDED TO AVOID CERTIFICATE VALIDATION ERROR
        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        Debug.Log("Post Request Recieved");

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
