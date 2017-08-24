using System.IO;
using SM.Core.Model;

namespace SM.Core
{
    public interface IPatternReader
    {
        PatternState Read(byte[] content);
    }
}
