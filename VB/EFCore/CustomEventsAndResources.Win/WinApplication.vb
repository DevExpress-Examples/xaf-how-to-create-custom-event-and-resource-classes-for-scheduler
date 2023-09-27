Imports System.ComponentModel
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Win
Imports DevExpress.ExpressApp.Win.Utils
Imports DevExpress.ExpressApp.Security
Imports CustomEventsAndResources.Module
Imports CustomEventsAndResources.Module.BusinessObjects

Namespace CustomEventsAndResources.Win

    ' For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
    Public Class CustomEventsAndResourcesWindowsFormsApplication
        Inherits WinApplication

        Public Sub New()
            SplashScreen = New DXSplashScreen(GetType(XafSplashScreen), New DefaultOverlayFormOptions())
            ApplicationName = "CustomEventsAndResources"
            Me.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema
            Me.UseOldTemplates = False
            DatabaseVersionMismatch += AddressOf CustomEventsAndResourcesWindowsFormsApplication_DatabaseVersionMismatch
            CustomizeLanguagesList += AddressOf CustomEventsAndResourcesWindowsFormsApplication_CustomizeLanguagesList
            Dim logonParameters As AuthenticationStandardLogonParameters = Nothing
            LastLogonParametersRead += Function(s, e)
                Dim logonParameters As AuthenticationStandardLogonParameters = Nothing
                If CSharpImpl.__Assign(logonParameters, TryCast(e.LogonObject, AuthenticationStandardLogonParameters)) IsNot Nothing AndAlso String.IsNullOrEmpty(logonParameters.UserName) Then
                    logonParameters.UserName = "Admin"
                End If
            End Function
        End Sub

        Private Sub CustomEventsAndResourcesWindowsFormsApplication_CustomizeLanguagesList(ByVal sender As Object, ByVal e As CustomizeLanguagesListEventArgs)
            Dim userLanguageName As String = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
            If Not Equals(userLanguageName, "en-US") AndAlso e.Languages.IndexOf(userLanguageName) = -1 Then
                e.Languages.Add(userLanguageName)
            End If
        End Sub

        Private Sub CustomEventsAndResourcesWindowsFormsApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
#If EASYTEST
        e.Updater.Update();
        e.Handled = true;
#Else
            If System.Diagnostics.Debugger.IsAttached Then
                e.Updater.Update()
                e.Handled = True
            Else
                Dim message As String = "The application cannot connect to the specified database, " & "because the database doesn't exist, its version is older " & "than that of the application or its schema does not match " & "the ORM data model structure. To avoid this error, use one " & "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article."
                If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
                    message += Microsoft.VisualBasic.Constants.vbCrLf & Microsoft.VisualBasic.Constants.vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
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
