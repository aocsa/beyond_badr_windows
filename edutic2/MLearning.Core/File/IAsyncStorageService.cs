using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLearning.Core.File
{
    public interface IAsyncStorageService
    {
        Task <byte[]> TryReadBinaryFile (string filename);
        Task<string> TryReadTextFile(string filename);
    }
}
