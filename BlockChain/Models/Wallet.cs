using BlockChain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain.Models
{
    public class Wallet
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

        public Wallet(CryptoService cryptoService)
        {
            var KeyPair = cryptoService.GenerateKeyPair();
            PublicKey = KeyPair.publicKey;
            PrivateKey = KeyPair.privateKey;
        }
    }
}
