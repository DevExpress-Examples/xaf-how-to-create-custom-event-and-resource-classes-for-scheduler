Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.ApplicationBuilder
Imports DevExpress.ExpressApp.Win.ApplicationBuilder
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Win
Imports DevExpress.Persistent.BaseImpl.EF.PermissionPolicy
Imports DevExpress.ExpressApp.Design

Namespace CustomEventsAndResources.Win

    Public Class ApplicationBuilder
        Implements IDesignTimeApplicationFactory

        Public Shared Function BuildApplication(ByVal connectionString As String) As WinApplication
            Dim builder = DevExpress.ExpressApp.Win.WinApplication.CreateBuilder()
            ' Register custom services for Dependency Injection. For more information, refer to the following topic: https://docs.devexpress.com/eXpressAppFramework/404430/
            ' builder.Services.AddScoped<CustomService>();
            ' Register 3rd-party IoC containers (like Autofac, Dryloc, etc.)
            ' builder.UseServiceProviderFactory(new DryIocServiceProviderFactory());
            ' builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.UseApplication(Of CustomEventsAndResourcesWindowsFormsApplication)()
            builder.Modules.AddConditionalAppearance().AddScheduler().AddValidation(Function(options)
                options.AllowValidationDetailsAccess = False
            End Function).Add(Of CustomEventsAndResources.Module.CustomEventsAndResourcesModule)().Add(Of CustomEventsAndResourcesWinModule)()
            builder.ObjectSpaceProviders.AddSecuredEFCore(Function(options) options.PreFetchReferenceProperties()).WithDbContext(Of CustomEventsAndResources.Module.BusinessObjects.CustomEventsAndResourcesEFCoreDbContext)(Function(application, options)
                ' Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
                ' Do not use this code in production environment to avoid data loss.
                ' We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
                'options.UseInMemoryDatabase("InMemory");
                options.UseSqlServer(connectionString)
                options.UseChangeTrackingProxies()
                options.UseObjectSpaceLinkProxies()
            End Function).AddNonPersistent()
            builder.Security.UseIntegratedMode(Sub(options)
                options.RoleType = GetType(PermissionPolicyRole)
                options.UserType = GetType(CustomEventsAndResources.Module.BusinessObjects.ApplicationUser)
                options.UserLoginInfoType = GetType(CustomEventsAndResources.Module.BusinessObjects.ApplicationUserLoginInfo)
                options.Events.OnSecurityStrategyCreated += Function(securityStrategy)
                    ' Use the 'PermissionsReloadMode.NoCache' option to load the most recent permissions from the database once
                    ' for every DbContext instance when secured data is accessed through this instance for the first time.
                    ' Use the 'PermissionsReloadMode.CacheOnFirstAccess' option to reduce the number of database queries.
                    ' In this case, permission requests are loaded and cached when secured data is accessed for the first time
                    ' and used until the current user logs out. 
                    ' See the following article for more details: https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Security.SecurityStrategy.PermissionsReloadMode.
                    CType(securityStrategy, SecurityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache
                End Function
            End Sub).UsePasswordAuthentication()
            builder.AddBuildStep(Function(application)
                application.ConnectionString = connectionString
#If DEBUG
                If System.Diagnostics.Debugger.IsAttached AndAlso application.CheckCompatibilityType Is CheckCompatibilityType.DatabaseSchema Then
                    application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways
                End If
#End If
            End Function)
            Dim winApplication = builder.Build()
            Return winApplication
        End Function

        Private Function Create() As XafApplication Implements IDesignTimeApplicationFactory.Create
            Return BuildApplication(XafApplication.DesignTimeConnectionString)
        End Function
    End Class
End Namespace
