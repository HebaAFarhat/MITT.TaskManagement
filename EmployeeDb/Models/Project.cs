using Ardalis.GuardClauses;

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

    public static Project Create(string name, string description, ProjectType projectType, Bank bank)
    {
        name = Guard.Against.NullOrEmpty(name, nameof(name), "Name_cannot_be_null_or_empty");
        description = Guard.Against.NullOrEmpty(description, nameof(description), "Description_cannot_be_null_OR_empty");
        projectType = Guard.Against.EnumOutOfRange(projectType, nameof(projectType), "project_type_out_range");

        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            ProjectType = projectType,
            Bank = bank,
            CreatedAt = DateTime.Now
        };
    }

    public void Update(string name, string description, ProjectType projectType, Bank bank)
    {
        name = Guard.Against.NullOrEmpty(name, nameof(name), "Name_cannot_be_null_or_empty");
        description = Guard.Against.NullOrEmpty(description, nameof(description), "Description_cannot_be_null_OR_empty");
        projectType = Guard.Against.EnumOutOfRange(projectType, nameof(projectType), "project_type_out_range");

        Name = name;
        Description = description;
        ProjectType = projectType;
        UpdatedAt = DateTime.Now;
        Bank = bank;
    }
}

public enum ProjectType
{
    MB, PY, WB, OT
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