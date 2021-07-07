namespace Core.Model
{
    public class CustomerContact
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Landline { get; set; }
        public string Cellphone { get; set; }
        public string Address { get; set; }
        public string Cp { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        public string FullName => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(LastName) ? $"{Name}, {LastName}" : LastName;
    }
}
