using Sha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security
{
    public class EncryptionService
    {

       public static string encrypt(string toEncrypt)
        {
            if (toEncrypt == null) return null;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(toEncrypt); 
            Sha256 sha = new Sha256();
            sha.AddData(data, 0, (uint)data.Length);

            ICollection<byte> hash = sha.GetHash();
            string hashPass = Sha256.ArrayToString(hash.ToList<byte>());
            return hashPass;
        }
    }
}
