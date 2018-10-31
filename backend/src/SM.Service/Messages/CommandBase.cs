namespace SM.Service.Command
{
    public partial class MarkStitches : ICommand
    {
    }

    public partial class UnmarkStitches : ICommand
    {
    }

    public partial class MarkBackstitches : ICommand
    {
    }

    public partial class UnmarkBackstitches : ICommand
    {
    }


    public interface ICommand
    {
        string PatternId { get; }
    }
}
