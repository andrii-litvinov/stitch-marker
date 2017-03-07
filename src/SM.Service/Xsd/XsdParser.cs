namespace SM.Service.Xsd
{
    public class XsdPatternParser : IPatternParser
    {
        public Pattern Parse(byte[] content)
        {
            return new Pattern();
        }
    }

    public class Pattern
    {
    }

    public interface IPatternParser
    {
    }

}