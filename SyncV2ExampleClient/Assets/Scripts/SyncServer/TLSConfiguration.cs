using UnityEngine;

namespace Servers
{
    [CreateAssetMenu(fileName = "TLSConfig", menuName = "TLS Configuration")]
    public sealed class TLSConfiguration : ScriptableObject
    {
#pragma warning disable IDE0044 // Add readonly modifier
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        private string developerDescription = string.Empty;
#endif

        [Tooltip("The public key of the TLS Certificate as a hex string.")]
        [SerializeField]
        private string publicKey = string.Empty;

        [Tooltip("The hostname of the server specified in the TLS Certificate. This is usually from the Common Name.")]
        [SerializeField]
        private string targetHost = string.Empty;
#pragma warning restore IDE0044 // Add readonly modifier

        public string PublicKey
        {
            get
            {
                return publicKey;
            }
        }

        // For a correctly formed TLS certificate, it will contain SAN extensions
        // with a DNS hostname specified. That will be the target host. However,
        // if it is missing, the target host becomes the CN property of the
        // Subject Name.
        // See: https://stackoverflow.com/questions/14023438/where-can-i-find-the-targethost-of-my-x509-certificate
        public string TargetHost
        {
            get
            {
                return targetHost;
            }
        }
    }
}
