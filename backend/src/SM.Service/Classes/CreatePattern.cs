namespace SM.Service.Classes
{
    public class CreatePattern
    {
        public CreatePattern(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }

        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}
