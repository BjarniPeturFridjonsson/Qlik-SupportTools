namespace SenseApiLibrary
{
    public class SenseApiUser
    {
        public static readonly SenseApiUser RepositoryServiceAccount = new SenseApiUser("INTERNAL", "sa_repository", "Repository Service Account");

        private readonly string _displayName;

        private SenseApiUser(string userDirectory, string userId, string displayName)
        {
            _displayName = displayName;
            UserDirectory = userDirectory;
            UserId = userId;
        }

        public string UserDirectory { get; }

        public string UserId { get; }

        public override string ToString() => _displayName;
    }
}