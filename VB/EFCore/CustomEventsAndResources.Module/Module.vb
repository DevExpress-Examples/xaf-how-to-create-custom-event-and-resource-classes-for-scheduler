Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Updating

Namespace CustomEventsAndResources.Module

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    Public NotInheritable Class CustomEventsAndResourcesModule
        Inherits ModuleBase

        Public Sub New()
            ' 
            ' CustomEventsAndResourcesModule
            ' 
            AdditionalExportedTypes.Add(GetType(BusinessObjects.ApplicationUser))
            AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.EF.PermissionPolicy.PermissionPolicyRole))
            AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.EF.ModelDifference))
            AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.EF.ModelDifferenceAspect))
            RequiredModuleTypes.Add(GetType(SystemModule.SystemModule))
            RequiredModuleTypes.Add(GetType(Security.SecurityModule))
            RequiredModuleTypes.Add(GetType(Objects.BusinessClassLibraryCustomizationModule))
            RequiredModuleTypes.Add(GetType(ConditionalAppearance.ConditionalAppearanceModule))
            RequiredModuleTypes.Add(GetType(Scheduler.SchedulerModuleBase))
            RequiredModuleTypes.Add(GetType(Validation.ValidationModule))
            Security.SecurityModule.UsedExportedTypes = UsedExportedTypes.Custom
            AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.EF.Event))
            AdditionalExportedTypes.Add(GetType(DevExpress.Persistent.BaseImpl.EF.Resource))
        End Sub

        Public Overrides Function GetModuleUpdaters(ByVal objectSpace As IObjectSpace, ByVal versionFromDB As Version) As IEnumerable(Of ModuleUpdater)
            Dim updater As ModuleUpdater = New DatabaseUpdate.Updater(objectSpace, versionFromDB)
            Return New ModuleUpdater() {updater}
        End Function

        Public Overrides Sub Setup(ByVal application As XafApplication)
            MyBase.Setup(application)
        ' Manage various aspects of the application UI and behavior at the module level.
        End Sub
    End Class
End Namespace
