using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Entities;
using Workflow.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Project> Projects {get; set;}
    public DbSet<TaskItem> Tasks {get; set;}
    public DbSet<Tag> Tags {get; set;}
    public DbSet<LogEntry> LogEntries {get; set;}
    public DbSet<Notification> Notifications {get; set;}
    public DbSet<Email> Emails {get; set;}
    public DbSet<IssuedAction> IssuedActions {get; set;}
    public DbSet<UserAction> UserActions {get; set;}
    public DbSet<RequestedAction> RequestedActions {get; set;}
    public DbSet<SystemAction> SystemActions {get; set;}
    public DbSet<WorkflowRule> WorkflowRules {get; set;}
    public DbSet<TaskTagLink> TaskTagLinks {get; set;}
    public DbSet<ProjectAssignment> ProjectAssignments {get; set;}
    public DbSet<TaskAssignment> TaskAssignments {get; set;}
    public DbSet<NotificationPreference> NotificationPreferences {get; set;}
    public DbSet<SystemActionPreference> SystemActionPreferences {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.UserName)
            .HasColumnType("citext");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        
        modelBuilder.Entity<Project>()
            .Property(p => p.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Project>()
            .Property(p => p.Name)
            .HasColumnType("citext");

        modelBuilder.Entity<Project>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.OwnerID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Project>()
            .HasIndex(p => new {p.OwnerID, p.Name}).IsUnique();
        
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.Name)
            .HasColumnType("citext");
        
        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new {t.ProjectID, t.Name}).IsUnique();
        
        modelBuilder.Entity<TaskItem>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Tag>()
            .Property(t => t.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Tag>()
            .Property(t => t.Name)
            .HasColumnType("citext");

        modelBuilder.Entity<Tag>()
            .HasIndex(t => new {t.ProjectID, t.Name}).IsUnique();
        
        modelBuilder.Entity<Tag>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Email>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.RecipientID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Notification>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.RecipientID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<IssuedAction>()
            .ComplexProperty(a => a.Change, change => change.ToJson());

        modelBuilder.Entity<UserAction>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ua => ua.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAction>()
            .ComplexProperty(a => a.Change, change => change.ToJson());
        
        modelBuilder.Entity<RequestedAction>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ra => ra.UserID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SystemAction>()
            .Property(u => u.ID)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<SystemAction>()
            .HasIndex(sa => new {sa.Type, sa.Target, sa.NewState}).IsUnique();
        
        modelBuilder.Entity<WorkflowRule>()
            .Property(u => u.ID)
            .ValueGeneratedNever();

        modelBuilder.Entity<WorkflowRule>()
            .HasIndex(wr => new {wr.Trigger, wr.State}).IsUnique();
        
        modelBuilder.Entity<TaskTagLink>()
            .HasKey(ttl => new {ttl.TaskID, ttl.TagID});

        modelBuilder.Entity<TaskTagLink>()
            .HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(ttl => ttl.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskTagLink>()
            .HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ttl => ttl.TagID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NotificationPreference>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(np => np.UserID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NotificationPreference>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(np => np.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<NotificationPreference>()
            .HasKey(np => new {np.UserID, np.ProjectID});
        
        modelBuilder.Entity<ProjectAssignment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(pa => pa.UserID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ProjectAssignment>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(pa => pa.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ProjectAssignment>()
            .HasKey(pa => new {pa.UserID, pa.ProjectID});
        
        modelBuilder.Entity<TaskAssignment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(pa => pa.UserID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskAssignment>()
            .HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(pa => pa.TaskID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskAssignment>()
            .HasKey(pa => new {pa.UserID, pa.TaskID});
        
        modelBuilder.Entity<SystemActionPreference>()
            .HasOne<WorkflowRule>()
            .WithMany()
            .HasForeignKey(sap => sap.RuleID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<SystemActionPreference>()
            .HasOne<SystemAction>()
            .WithMany()
            .HasForeignKey(sap => sap.ActionID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<SystemActionPreference>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(sap => sap.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SystemActionPreference>()
            .HasKey(sap => new {sap.RuleID, sap.ActionID, sap.ProjectID});
    }
}
