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

    public virtual ICollection<AssignedManager> AssignedManagers { get; set; }

    public static Project Create(string name, string description, ProjectType projectType) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        ProjectType = projectType,
        CreatedAt = DateTime.Now
    };

    public void Update(string name, string description, ProjectType projectType)
    {
        Name = name;
        Description = description;
        ProjectType = projectType;
        UpdatedAt = DateTime.Now;
    }
}

public enum ProjectType
{
    MB, PY, WB, OT
}