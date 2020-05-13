using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;

namespace SkillzSDK.Editor
{
    /// <summary>
    /// Exposes commands in a "Certificate" menu item in the Unity editor.
    /// </summary>
    public static class Certificate
    {
        [MenuItem("Certificate/Get Public Key String")]
        public static void GetPublicKeyString()
        {
            var selectedFile = EditorUtility.OpenFilePanel(
                "Pick a certificate file", 
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
                "crt"
            );

            if (!File.Exists(selectedFile))
            {
                return;
            }

            try
            {
                var certificate = X509Certificate.CreateFromCertFile(selectedFile);
                EditorUtility.DisplayDialog(
                    "Public Key Retrieved",
                    string.Format("Here's the certificate's public key string as hex:\n{0}", certificate.GetPublicKeyString()),
                    "OK"
                );
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog(
                    "Error", 
                    string.Format("There was an error opening the selected file: {0}", 
                    e.Message), 
                    "OK"
                );
            }
        }
    }
}
