using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CertificateValidator : CertificateHandler
{
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
}
