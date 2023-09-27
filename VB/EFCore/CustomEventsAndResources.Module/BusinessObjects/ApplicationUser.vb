Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Drawing
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.BaseImpl.EF.PermissionPolicy

Namespace CustomEventsAndResources.Module.BusinessObjects

    <DefaultProperty(NameOf(PermissionPolicyUser.UserName))>
    Public Class ApplicationUser
        Inherits PermissionPolicyUser
        Implements ISecurityUserWithLoginInfo, IResource

        Public Sub New()
            MyBase.New()
            Me.UserLogins = New ObservableCollection(Of ApplicationUserLoginInfo)()
        End Sub

        <Browsable(False)>
        <DC.Aggregated>
        Public Overridable Property UserLogins As IList(Of ApplicationUserLoginInfo)

        Private ReadOnly Property UserLogins As IEnumerable(Of ISecurityUserLoginInfo)
            Get
                Return Me.UserLogins.OfType(Of ISecurityUserLoginInfo)()
            End Get
        End Property

        Private Function CreateUserLoginInfo(ByVal loginProviderName As String, ByVal providerUserKey As String) As ISecurityUserLoginInfo Implements ISecurityUserWithLoginInfo.CreateUserLoginInfo
            Dim result As ApplicationUserLoginInfo = CType(Me, IObjectSpaceLink).ObjectSpace.CreateObject(Of ApplicationUserLoginInfo)()
            result.LoginProviderName = loginProviderName
            result.ProviderUserKey = providerUserKey
            result.UserProp = Me
            Return result
        End Function

#Region "Resource"
        <NotMapped>
        Public Property Color As Color
            Get
                Return Me.Color.FromArgb(ColorInt)
            End Get

            Set(ByVal value As Color)
                ColorInt = value.ToArgb()
            End Set
        End Property

        <Browsable(False)>
        Public Overridable Property ColorInt As Int32

        Public Overridable Property Events As IList(Of CustomEventWithUserResources) = New ObservableCollection(Of CustomEventWithUserResources)()

        <VisibleInListView(False), VisibleInDetailView(False), VisibleInLookupListView(False)>
        Private ReadOnly Property IResource_Id As Object Implements IResource.Id
            Get
                Return Me.ID
            End Get
        End Property

        <NotMapped>
        Public Property Caption As String Implements IResource.Caption
            Get
                Return Me.UserName
            End Get

            Set(ByVal value As String)
                Me.UserName = value
            End Set
        End Property

        <VisibleInListView(False), VisibleInDetailView(False), VisibleInLookupListView(False)>
        Public ReadOnly Property OleColor As Integer Implements IResource.OleColor
            Get
                Return ColorTranslator.ToOle(Color.FromArgb(ColorInt))
            End Get
        End Property

        Public Overrides Sub OnCreated()
            MyBase.OnCreated()
            Color = Color.White
        End Sub
#End Region
    End Class
End Namespace
