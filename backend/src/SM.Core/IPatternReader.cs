using System.IO;
using SM.Core.Model;

namespace SM.Core
{
    public interface IPatternReader
    {
        Pattern Read(byte[] content);
    }
}
