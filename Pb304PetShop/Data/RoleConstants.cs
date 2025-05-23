namespace Pb304PetShop.Data
{
    public class RoleConstants
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Moderator = "Moderator";
        public const string Manager = "Manager";
        public const string SuperAdmin = "SuperAdmin";
        public static readonly List<string> AllRoles = new()
        {
            Admin,
            User,
            Moderator,
            Manager,
            SuperAdmin
        };
    }
}
