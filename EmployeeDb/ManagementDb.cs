using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb.Models;
using Newtonsoft.Json;

namespace MITT.EmployeeDb
{
    public partial class ManagementDb : DbContext
    {
        public ManagementDb()
        {
        }

        public ManagementDb(DbContextOptions<ManagementDb> options) : base(options)
        {
        }

        public virtual DbSet<AssignedBeTask> AssignedBetasks { get; set; }
        public virtual DbSet<AssignedManager> AssignedManagers { get; set; }
        public virtual DbSet<AssignedQaTask> AssignedQatasks { get; set; }
        public virtual DbSet<Developer> Developers { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<DevTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder.UseSqlServer("Data Source =.; Initial Catalog = ManagementDb; Trusted_Connection = true;TrustServerCertificate=True;");
        }

        public async Task<string> GenerateSequence(ProjectType prefix = ProjectType.MB, bool withPrefix = true)
        {
            var result = new SqlParameter("@result", System.Data.SqlDbType.BigInt)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            await Database.ExecuteSqlInterpolatedAsync($"SELECT {result} = (NEXT VALUE FOR SequenceGenerator.[Sequence-Generator])");
            return withPrefix ? MapSeqNumber($"{prefix}{(long)result.Value}") : result.ToString()!;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignedBeTask>()
                .Property(x => x.BeReviews)
                .HasConversion(m => JsonConvert.SerializeObject(m), vm => JsonConvert.DeserializeObject<List<BeReview>>(vm));

            modelBuilder.Entity<AssignedQaTask>()
                .Property(x => x.QaReviews)
                .HasConversion(m => JsonConvert.SerializeObject(m), vm => JsonConvert.DeserializeObject<List<QaReview>>(vm));

            modelBuilder.Entity<DevTask>()
                .Property(x => x.Requirements)
                .HasConversion(m => JsonConvert.SerializeObject(m), vm => JsonConvert.DeserializeObject<List<string>>(vm));

            modelBuilder.HasSequence<long>("Sequence-Generator", schema: "SequenceGenerator")
                   .StartsAt(1)
                   .IncrementsBy(1);

            modelBuilder.Entity<AssignedBeTask>(entity =>
            {
                entity.ToTable("AssignedBETasks");

                entity.HasIndex(e => e.DevTaskId, "IX_AssignedBETasks_DevTaskId");

                entity.HasIndex(e => e.DeveloperId, "IX_AssignedBETasks_DeveloperId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.DevTask)
                    .WithMany(p => p.AssignedBetasks)
                    .HasForeignKey(d => d.DevTaskId);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.AssignedBetasks)
                    .HasForeignKey(d => d.DeveloperId);
            });
            modelBuilder.Entity<AssignedQaTask>(entity =>
            {
                entity.ToTable("AssignedQATasks");

                entity.HasIndex(e => e.DevTaskId, "IX_AssignedQATasks_DevTaskId");

                entity.HasIndex(e => e.DeveloperId, "IX_AssignedQATasks_DeveloperId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.DevTask)
                    .WithMany(p => p.AssignedQatasks)
                    .HasForeignKey(d => d.DevTaskId);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.AssignedQatasks)
                    .HasForeignKey(d => d.DeveloperId);
            });

            modelBuilder.Entity<AssignedManager>(entity =>
            {
                entity.HasIndex(e => e.ProjectId, "IX_AssignedManagers_ProjectId");

                entity.HasIndex(e => e.ProjectManagerId, "IX_AssignedManagers_ProjectManagerId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.AssignedManagers)
                    .HasForeignKey(d => d.ProjectId);

                entity.HasOne(d => d.ProjectManager)
                    .WithMany(p => p.AssignedManagers)
                    .HasForeignKey(d => d.ProjectManagerId);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();
                entity.HasIndex(x => new { x.Bank, x.ProjectType }).IsUnique();
            });

            modelBuilder.Entity<DevTask>(entity =>
            {
                entity.HasIndex(e => e.AssignedManagerId, "IX_Tasks_AssignedManagerId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.AssignedManager)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.AssignedManagerId);
            });
            modelBuilder.HasSequence("Sequence-Generator", "SequenceGenerator");
        }

        private static string MapSeqNumber(string seqNumber, int padding = 5, char paddingValue = '0')
            => new string(seqNumber.Take(2).ToArray()) + seqNumber[2..].PadLeft(padding, paddingValue);
    }
}