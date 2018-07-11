using SM.Service.EventReader;

namespace SM.Service.Messages
{
    public sealed partial class PatternCreated : IOwnerId
    {
        public string GetOwnerId => ownerId_;
    }

    public sealed partial class PatternDeleted : IOwnerId
    {
        public string GetOwnerId => ownerId_;
    }

    public sealed partial class PatternUploaded : IOwnerId
    {
        public string GetOwnerId => ownerId_;
    }
}
