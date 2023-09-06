namespace MITT.EmployeeDb.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}

public abstract class Employee : BaseEntity
{
    public string FullName { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Pin { get; set; }
    public string Image { get; set; }
    public bool IsSigned { get; set; } = false;
    public ActiveState ActiveState { get; set; } = ActiveState.Active;

}

public enum ActiveState
{
    Inactive, Active
}