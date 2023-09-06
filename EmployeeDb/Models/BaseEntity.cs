using Ardalis.GuardClauses;

namespace MITT.EmployeeDb.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}

public abstract class Identity : BaseEntity
{
    public Identity(string fullName,string nickName,string email ,string phone)
    {
        FullName = Guard.Against.NullOrEmpty(fullName,nameof(fullName),"FullName_cannot_be_null");
        NickName = Guard.Against.NullOrEmpty(nickName,nameof(nickName),"Nickname_cannot_be_null");
        Email = Guard.Against.NullOrEmpty(email,nameof(email),"Email_cannot_be_null");
        Phone = Guard.Against.NullOrEmpty(phone ,nameof(phone) ,"phone_cannot_be_null");
    }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Pin { get; set; }
    public bool IsSigned { get; set; } = false;
}

public enum ActiveState
{
    Inactive, Active
}