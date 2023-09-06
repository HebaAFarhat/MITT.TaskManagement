namespace MITT.EmployeeDb.Models
{
    public partial class Manager : Employee
    {
        public Manager()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

       

        public virtual ICollection<AssignedManager> AssignedManagers { get; private set; } = new HashSet<AssignedManager>();

        public static Manager Create(string fullName, string nickName, string email, string phone) => new()
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            NickName = nickName,
            Email = email,
            Phone = phone,
            CreatedAt = DateTime.Now
        };

        public void Update(string firstName, string lastName, string email, string phone)
        {
            FullName = firstName;
            NickName = lastName;
            Phone = email;
            Email = phone;
            UpdatedAt = DateTime.Now;
        }
    }
}