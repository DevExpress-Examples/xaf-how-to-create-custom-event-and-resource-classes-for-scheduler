Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.ExpressApp.Model.Core
Imports DevExpress.ExpressApp.Model.DomainLogics
Imports DevExpress.ExpressApp.Model.NodeGenerators
Imports DevExpress.Persistent.BaseImpl

Namespace CustomEventsAndResources.Blazor.Server

    <ToolboxItemFilter("Xaf.Platform.Blazor")>
    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    Public NotInheritable Class CustomEventsAndResourcesBlazorModule
        Inherits ModuleBase

        'private void Application_CreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
        '    e.Store = new ModelDifferenceDbStore((XafApplication)sender, typeof(ModelDifference), true, "Blazor");
        '    e.Handled = true;
        '}
        Private Sub Application_CreateCustomUserModelDifferenceStore(ByVal sender As Object, ByVal e As CreateCustomModelDifferenceStoreEventArgs)
            e.Store = New ModelDifferenceDbStore(CType(sender, XafApplication), GetType(ModelDifference), False, "Blazor")
            e.Handled = True
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function GetModuleUpdaters(ByVal objectSpace As IObjectSpace, ByVal versionFromDB As Version) As IEnumerable(Of ModuleUpdater)
            Return ModuleUpdater.EmptyModuleUpdaters
        End Function

        Public Overrides Sub Setup(ByVal application As XafApplication)
            MyBase.Setup(application)
            ' Uncomment this code to store the shared model differences (administrator settings in Model.XAFML) in the database.
            ' For more information, refer to the following topic: https://docs.devexpress.com/eXpressAppFramework/113698/
            'application.CreateCustomModelDifferenceStore += Application_CreateCustomModelDifferenceStore;
            application.CreateCustomUserModelDifferenceStore += AddressOf Application_CreateCustomUserModelDifferenceStore
        End Sub
    End Class
End Namespace
