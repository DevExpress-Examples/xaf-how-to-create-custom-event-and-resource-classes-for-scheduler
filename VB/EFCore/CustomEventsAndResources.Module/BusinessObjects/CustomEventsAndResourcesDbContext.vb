Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Design
Imports DevExpress.Persistent.BaseImpl.EF.PermissionPolicy
Imports DevExpress.Persistent.BaseImpl.EF
Imports DevExpress.ExpressApp.Design
Imports DevExpress.ExpressApp.EFCore.DesignTime

Namespace CustomEventsAndResources.Module.BusinessObjects

    ' This code allows our Model Editor to get relevant EF Core metadata at design time.
    ' For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
    Public Class CustomEventsAndResourcesContextInitializer
        Inherits DbContextTypesInfoInitializerBase

        Protected Overrides Function CreateDbContext() As DbContext
            Dim optionsBuilder = New DbContextOptionsBuilder(Of CustomEventsAndResourcesEFCoreDbContext)().UseSqlServer(";").UseChangeTrackingProxies().UseObjectSpaceLinkProxies()
            Return New CustomEventsAndResourcesEFCoreDbContext(optionsBuilder.Options)
        End Function
    End Class

    'This factory creates DbContext for design-time services. For example, it is required for database migration.
    Public Class CustomEventsAndResourcesDesignTimeDbContextFactory
        Implements IDesignTimeDbContextFactory(Of CustomEventsAndResourcesEFCoreDbContext)

        Public Function CreateDbContext(ByVal args As String()) As CustomEventsAndResourcesEFCoreDbContext Implements IDesignTimeDbContextFactory(Of CustomEventsAndResourcesEFCoreDbContext).CreateDbContext
            Throw New InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.")
        'var optionsBuilder = new DbContextOptionsBuilder<CustomEventsAndResourcesEFCoreDbContext>();
        'optionsBuilder.UseSqlServer("Integrated Security=SSPI;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=CustomEventsAndResources");
        'optionsBuilder.UseChangeTrackingProxies();
        'optionsBuilder.UseObjectSpaceLinkProxies();
        'return new CustomEventsAndResourcesEFCoreDbContext(optionsBuilder.Options);
        End Function
    End Class

    <TypesInfoInitializer(GetType(CustomEventsAndResourcesContextInitializer))>
    Public Class CustomEventsAndResourcesEFCoreDbContext
        Inherits DbContext

        Public Sub New(ByVal options As DbContextOptions(Of CustomEventsAndResourcesEFCoreDbContext))
            MyBase.New(options)
        End Sub

        'public DbSet<ModuleInfo> ModulesInfo { get; set; }
        Public Property ModelDifferences As DbSet(Of ModelDifference)

        Public Property ModelDifferenceAspects As DbSet(Of ModelDifferenceAspect)

        Public Property Roles As DbSet(Of PermissionPolicyRole)

        Public Property Users As DbSet(Of ApplicationUser)

        Public Property UserLoginInfos As DbSet(Of ApplicationUserLoginInfo)

        'public DbSet<Event> Event { get; set; }
        Public Property CustomEventWithUserResources As DbSet(Of CustomEventWithUserResources)

        Public Property CustomResources As DbSet(Of CustomResource)

        Public Property CustomEventWithCustomResource As DbSet(Of CustomEventWithCustomResource)

        Protected Overrides Sub OnModelCreating(ByVal modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)
            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues)
            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
            modelBuilder.Entity(Of ApplicationUserLoginInfo)(Sub(b) b.HasIndex(NameOf(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.LoginProviderName), NameOf(DevExpress.ExpressApp.Security.ISecurityUserLoginInfo.ProviderUserKey)).IsUnique())
            modelBuilder.Entity(Of ModelDifference)().HasMany(Function(t) t.Aspects).WithOne(Function(t) t.Owner).OnDelete(DeleteBehavior.Cascade)
        'modelBuilder.Entity<CustomEventWithUserResources>()
        '.HasMany(p => p.Resources)
        '.WithMany(r => r.Events);
        'modelBuilder.Entity<CustomEventWithUserResource>()
        '      .HasOne(p => p.Resource)
        '      .WithMany(r => r.Events);
        End Sub
    End Class
End Namespace
