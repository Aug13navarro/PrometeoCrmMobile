namespace Core.Model
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Consecutive { get; set; }
        public int LoadTn { get; set; }

        public Device Device { get; set; }
        public ApplicationPoint[] PointApplications { get; set; }
    }
}
