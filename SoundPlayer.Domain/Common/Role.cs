namespace SoundPlayer.Domain.Common
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static IList<string> GetRoles() => new List<string> { Admin, User };
    }
}
