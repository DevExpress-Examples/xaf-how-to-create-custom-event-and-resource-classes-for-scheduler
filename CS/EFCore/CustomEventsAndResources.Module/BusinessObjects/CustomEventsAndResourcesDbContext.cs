using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;

namespace CustomEventsAndResources.Module.BusinessObjects;

// This code allows our Model Editor to get relevant EF Core metadata at design time.
// For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
public class CustomEventsAndResourcesContextInitializer : DbContextTypesInfoInitializerBase {
	protected override DbContext CreateDbContext() {
		var optionsBuilder = new DbContextOptionsBuilder<CustomEventsAndResourcesEFCoreDbContext>()
            .UseSqlServer(";")
            .UseChangeTrackingProxies()
            .UseObjectSpaceLinkProxies();
        return new CustomEventsAndResourcesEFCoreDbContext(optionsBuilder.Options);
	}
}
//This factory creates DbContext for design-time services. For example, it is required for database migration.
public class CustomEventsAndResourcesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomEventsAndResourcesEFCoreDbContext> {
	public CustomEventsAndResourcesEFCoreDbContext CreateDbContext(string[] args) {
		throw new InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
		//var optionsBuilder = new DbContextOptionsBuilder<CustomEventsAndResourcesEFCoreDbContext>();
		//optionsBuilder.UseSqlServer("Integrated Security=SSPI;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=CustomEventsAndResources");
        //optionsBuilder.UseChangeTrackingProxies();
        //optionsBuilder.UseObjectSpaceLinkProxies();
		//return new CustomEventsAndResourcesEFCoreDbContext(optionsBuilder.Options);
	}
}
[TypesInfoInitializer(typeof(CustomEventsAndResourcesContextInitializer))]
public class CustomEventsAndResourcesEFCoreDbContext : DbContext {
	public CustomEventsAndResourcesEFCoreDbContext(DbContextOptions<CustomEventsAndResourcesEFCoreDbContext> options) : base(options) {
	}
	//public DbSet<ModuleInfo> ModulesInfo { get; set; }
	public DbSet<ModelDifference> ModelDifferences { get; set; }
	public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
	public DbSet<PermissionPolicyRole> Roles { get; set; }
	public DbSet<CustomEventsAndResources.Module.BusinessObjects.ApplicationUser> Users { get; set; }
    public DbSet<CustomEventsAndResources.Module.BusinessObjects.ApplicationUserLoginInfo> UserLoginInfos { get; set; }
    //public DbSet<Event> Event { get; set; }
    public DbSet<CustomEventWithUserResources> CustomEventWithUserResources { get; set; }
    public DbSet<CustomResource> CustomResources { get; set; }
    public DbSet<CustomEventWithCustomResource> CustomEventWithCustomResource { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
        modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
        modelBuilder.Entity<CustomEventsAndResources.Module.BusinessObjects.ApplicationUserLoginInfo>(b => {
            b.HasIndex(nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.LoginProviderName), nameof(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.ProviderUserKey)).IsUnique();
        });
        modelBuilder.Entity<ModelDifference>()
            .HasMany(t => t.Aspects)
            .WithOne(t => t.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<CustomEventWithUserResources>()
               //.HasMany(p => p.Resources)
               //.WithMany(r => r.Events);

        //modelBuilder.Entity<CustomEventWithUserResource>()
        //      .HasOne(p => p.Resource)
        //      .WithMany(r => r.Events);
    }
}
