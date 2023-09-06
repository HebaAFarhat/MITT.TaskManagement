namespace MITT.EmployeeDb.Models;

public partial class Project : BaseEntity
{
    private Project()
    {
        AssignedManagers = new HashSet<AssignedManager>();
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProjectType ProjectType { get; private set; }
    public Bank Bank { get; private set; }

    public virtual ICollection<AssignedManager> AssignedManagers { get; set; }

    public static Project Create(string name, string description, ProjectType projectType, Bank bank) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        ProjectType = projectType,
        CreatedAt = DateTime.Now,
        Bank = bank
    };

    public void Update(string name, string description, ProjectType projectType, Bank bank)
    {
        Name = name;
        Description = description;
        ProjectType = projectType;
        UpdatedAt = DateTime.Now;
        Bank = bank;
    }
}

public enum ProjectType
{
    MB , PY, WB, OT
}

public enum Bank
{
    NorthAfrica = 10, 
    Wahda = 20, 
    Tejari = 30, 
    Jumhoria = 40, 
    Ismali = 50,
    Sahara = 60, 
}