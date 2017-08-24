namespace SM.Service.Classes
{
    public class CreatePattern
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }

        public CreatePattern(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }
    }
}