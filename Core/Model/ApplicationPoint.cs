namespace Core.Model
{
    public class ApplicationPoint
    {
        public int Id { get; set; }
        public string StoragePlace { get; set; }
        public string PointOfApplication { get; set; }
        public string Location { get; set; }
        public float Pressure { get; set; }
        public float Variation { get; set; }
        public float? Waterflow { get; set; }
        public float? WaterflowVariation { get; set; }

        public string City => !string.IsNullOrWhiteSpace(Location) ? Location.Split(',')[4].Trim() : "";
    }
}
