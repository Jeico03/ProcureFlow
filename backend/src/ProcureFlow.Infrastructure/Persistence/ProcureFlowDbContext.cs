using Microsoft.EntityFrameworkCore;
using ProcureFlow.Domain.Entities;

namespace ProcureFlow.Infrastructure.Persistence;

public class ProcureFlowDbContext : DbContext
{
    public ProcureFlowDbContext(DbContextOptions<ProcureFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    public DbSet<Approval> Approvals => Set<Approval>();
    public DbSet<Request> Requests => Set<Request>();

    public DbSet<ReceivingIssue> ReceivingIssues => Set<ReceivingIssue>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ProcureFlowDbContext).Assembly);
    }
}
