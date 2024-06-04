using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVScript
{
    public interface ISecurity
    {
        public byte[] Encrypt(string plain);
        public string Decrypt(byte[] cipher);
    }
}
