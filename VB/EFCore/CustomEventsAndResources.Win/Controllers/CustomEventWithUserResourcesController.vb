Imports CustomEventsAndResources.Module.BusinessObjects
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Scheduler.Win

Namespace CustomEventsAndResources.Win.Controllers

    Public Class CustomEventWithUserResourcesController
        Inherits ObjectViewController(Of ListView, CustomEventsAndResources.Module.BusinessObjects.CustomEventWithUserResources)

        Protected Overrides Sub OnViewControlsCreated()
            MyBase.OnViewControlsCreated()
            CType(View.Editor, SchedulerListEditor).ResourcesMappings.Id = "ID"
        End Sub
    End Class
End Namespace
