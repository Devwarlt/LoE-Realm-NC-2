#region

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Text;

#endregion

namespace LoESoft.GameServer
{
    public class RSA
    {
        public static readonly RSA Instance = new RSA(@"
-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQDfhb/mtO6xf4SieiNtBhGwPUgVtvNDQ6j50eX69U2z5fF0iHOR
zVGL4Fr6b4hWcN7l0wf4eGxWHAaJlmungHgwmOoEoT1EftmWgEJxkBIt6+Cpflyt
XrvC7E6LhfY+2i/omJlFh2iPv0ncsCkGvJ64o33lmBqqju8Rb3YrPC154wIDAQAB
AoGAOVUrNdfu6aRVtv8xGwPVgakWVkuI9hXiv8FxBf/poF04o7VkP1+0rPYtH9N/
2vw0kCqi/r6UuzmRC7WDg4FWUlX56v2OqK9LlMSAamVt5yiCTq7KsdCjyBVl+f/Y
SwYcOGgmi/RkEMIVY8hJ+C+6GGnzRl1IIOsWlqXLnrKFUsECQQD4QFs82b7AzwK9
C8oq3DaFbi/v4jB23m8x14nijt0O3NmugPDlLpmGkU6pn+RN3bDpsw8qHOuhiudy
p8ofgYOTAkEA5n/MLujlBA9zFG1Xx8S+rlYZqGhi9zDsZYwATQUz0650fIXbLuGl
eZgmQoLp8AOEmmUSi56PsuOLLGCpcp/CcQJAU9DuvRHLbK+3/fnwDR6CrfQw7S4S
LOAW7N4X6M1RZ4Y7XMaeDtL39M40n+KjI4MZCx5wnUhcahNK55QtEwwYTwJBAJCI
3Hkh0tF7+pZ5hgyfQ05AcTBX3I7SX7nBU0L4myoGf8bBjNJV7hRUItGcE6NMIW9L
J5jjIYp0AoYeSsK0iRECQQCD4bGDZF23Xp1iM/HfT/T+OhJS8vsIcHPyWth6tY0b
/mk8lJ560cUWA7rN4R4c1mcTQMjsFqqx8WTFmRR54MOm
-----END RSA PRIVATE KEY-----");

        private readonly RsaEngine engine;
        private readonly AsymmetricKeyParameter key;

        private RSA(string privPem)
        {
            key = (new PemReader(new StringReader(privPem.Trim())).ReadObject() as AsymmetricCipherKeyPair).Private;
            engine = new RsaEngine();
            engine.Init(true, key);
        }

        public string Decrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            byte[] dat = Convert.FromBase64String(str);
            Pkcs1Encoding encoding = new Pkcs1Encoding(engine);
            encoding.Init(false, key);
            return Encoding.UTF8.GetString(encoding.ProcessBlock(dat, 0, dat.Length));
        }

        public string Encrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            byte[] dat = Encoding.UTF8.GetBytes(str);
            Pkcs1Encoding encoding = new Pkcs1Encoding(engine);
            encoding.Init(true, key);
            return Convert.ToBase64String(encoding.ProcessBlock(dat, 0, dat.Length));
        }
    }
}