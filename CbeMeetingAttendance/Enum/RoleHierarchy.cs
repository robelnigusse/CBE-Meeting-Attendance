namespace CbeMeetingAttendance.Enums
{
    public static class RoleHierarchy
    {
        public static readonly Dictionary<string, int> Levels =
            new()
            {
                {"SuperAdmin",100},
                {"Admin",90},
                {"VP",80},
                {"Director",70},
                {"Manager",60},
                {"Staff",10}
            };
    }
}