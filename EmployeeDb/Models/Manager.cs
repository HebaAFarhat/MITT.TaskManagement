namespace MITT.EmployeeDb.Models
{
    public partial class Manager : Identity
    {
        public Manager(string fullName,string nickName,string email ,string phone ) : base(fullName,nickName,email,phone)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        public string Image { get; private set; }
        public ActiveState ActiveState { get; set; } = ActiveState.Active;

        public virtual ICollection<AssignedManager> AssignedManagers { get; private set; } = new HashSet<AssignedManager>();

        public static Manager Create(string fullName, string nickName, string email, string phone) => new( fullName, nickName, email , phone ) 
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