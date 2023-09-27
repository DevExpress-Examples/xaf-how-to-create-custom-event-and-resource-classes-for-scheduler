Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp.Xpo

Namespace CustomEventsAndResources.Module

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    Public NotInheritable Class CustomEventsAndResourcesModule
        Inherits ModuleBase

        Public Sub New()
            ' 
            ' CustomEventsAndResourcesModule
            ' 
            AdditionalExportedTypes.Add(GetType(ModelDifference))
            AdditionalExportedTypes.Add(GetType(ModelDifferenceAspect))
            AdditionalExportedTypes.Add(GetType(BaseObject))
            AdditionalExportedTypes.Add(GetType([Event]))
            AdditionalExportedTypes.Add(GetType(Resource))
            RequiredModuleTypes.Add(GetType(SystemModule.SystemModule))
            RequiredModuleTypes.Add(GetType(Security.SecurityModule))
            RequiredModuleTypes.Add(GetType(Objects.BusinessClassLibraryCustomizationModule))
            RequiredModuleTypes.Add(GetType(ConditionalAppearance.ConditionalAppearanceModule))
            RequiredModuleTypes.Add(GetType(Scheduler.SchedulerModuleBase))
            RequiredModuleTypes.Add(GetType(Validation.ValidationModule))
        End Sub

        Public Overrides Function GetModuleUpdaters(ByVal objectSpace As IObjectSpace, ByVal versionFromDB As Version) As IEnumerable(Of ModuleUpdater)
            Dim updater As ModuleUpdater = New DatabaseUpdate.Updater(objectSpace, versionFromDB)
            Return New ModuleUpdater() {updater}
        End Function

        Public Overrides Sub Setup(ByVal application As XafApplication)
            MyBase.Setup(application)
        ' Manage various aspects of the application UI and behavior at the module level.
        End Sub

        Public Overrides Sub CustomizeTypesInfo(ByVal typesInfo As ITypesInfo)
            MyBase.CustomizeTypesInfo(typesInfo)
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo)
        End Sub
    End Class
End Namespace
