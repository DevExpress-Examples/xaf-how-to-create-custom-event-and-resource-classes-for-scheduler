Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.ApplicationBuilder
Imports DevExpress.ExpressApp.Blazor
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Security.ClientServer
Imports DevExpress.ExpressApp.SystemModule
Imports CustomEventsAndResources.Module.BusinessObjects
Imports Microsoft.EntityFrameworkCore
Imports DevExpress.ExpressApp.EFCore
Imports DevExpress.EntityFrameworkCore.Security
Imports DevExpress.ExpressApp.Utils

Namespace CustomEventsAndResources.Blazor.Server

    Public Class CustomEventsAndResourcesBlazorApplication
        Inherits BlazorApplication

        Public Sub New()
            ApplicationName = "CustomEventsAndResources"
            CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema
            DatabaseVersionMismatch += AddressOf CustomEventsAndResourcesBlazorApplication_DatabaseVersionMismatch
            Dim logonParameters As AuthenticationStandardLogonParameters = Nothing
            LastLogonParametersRead += Function(s, e)
                Dim logonParameters As AuthenticationStandardLogonParameters = Nothing
                If CSharpImpl.__Assign(logonParameters, TryCast(e.LogonObject, AuthenticationStandardLogonParameters)) IsNot Nothing AndAlso String.IsNullOrEmpty(logonParameters.UserName) Then
                    logonParameters.UserName = "Admin"
                End If
            End Function
        End Sub

        Private Class EmptySettingsStorage
            Inherits SettingsStorage

            Public Overrides Function LoadOption(ByVal optionPath As String, ByVal optionName As String) As String
                Return Nothing
            End Function

            Public Overrides Sub SaveOption(ByVal optionPath As String, ByVal optionName As String, ByVal optionValue As String)
            End Sub
        End Class

        Protected Overrides Function CreateLogonParameterStoreCore() As SettingsStorage
            Return New EmptySettingsStorage()
        End Function

        Protected Overrides Sub OnSetupStarted()
            MyBase.OnSetupStarted()
#If DEBUG
            If System.Diagnostics.Debugger.IsAttached AndAlso CheckCompatibilityType Is CheckCompatibilityType.DatabaseSchema Then
                DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways
            End If
#End If
        End Sub

        Private Sub CustomEventsAndResourcesBlazorApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
#If EASYTEST
        e.Updater.Update();
        e.Handled = true;
#Else
            If System.Diagnostics.Debugger.IsAttached Then
                e.Updater.Update()
                e.Handled = True
            Else
                Dim message As String = "The application cannot connect to the specified database, " & "because the database doesn't exist, its version is older " & "than that of the application or its schema does not match " & "the ORM data model structure. To avoid this error, use one " & "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article."
                If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
                    message += Global.Microsoft.VisualBasic.Constants.vbCrLf & Global.Microsoft.VisualBasic.Constants.vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
                End If

                Throw New InvalidOperationException(message)
            End If
#End If
        End Sub

        Private Class CSharpImpl

            <System.Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
