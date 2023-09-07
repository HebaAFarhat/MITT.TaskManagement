using Ardalis.GuardClauses;

namespace MITT.EmployeeDb.Models
{
    public partial class Developer : Employee
    {
        public Developer(string fullName,string nickName,string email ,string phone ) : base(fullName,nickName,email,phone)
        {
            AssignedBetasks = new HashSet<AssignedBeTask>();
            AssignedQatasks = new HashSet<AssignedQaTask>();
        }

        public DeveloperType Type { get; set; }
        
        public ActiveState ActiveState { get; set; } = ActiveState.Active;
        public virtual ICollection<AssignedBeTask> AssignedBetasks { get; set; }
        public virtual ICollection<AssignedQaTask> AssignedQatasks { get; set; }

        public static Developer Create(string fullName, string nickName, string email, string phone, string pin, DeveloperType developerType)
        {
            pin = Guard.Against.NullOrEmpty(pin,pin,"pin_most_not_be_null");
            developerType = Guard.Against.EnumOutOfRange(developerType, nameof(developerType), "out of range developerType");
            
            return new(fullName, nickName,email,phone)
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                NickName = nickName,
                Email = email,
                Phone = phone,
                Pin = pin,
                Type = developerType,
                CreatedAt = DateTime.Now
            };
        }

        public void Update(string fullName, string nickName, string email, string phone, DeveloperType developerType)
        {
            developerType = Guard.Against.EnumOutOfRange(developerType, nameof(developerType), "out of range developerType");

            FullName = fullName;
            NickName = nickName;
            Phone = email;
            Email = email;
            Type = developerType;
            UpdatedAt = DateTime.Now;
        }
    }

    public enum DeveloperType
    {
        Be = 1, Qa, Pm, Rv
    }
}