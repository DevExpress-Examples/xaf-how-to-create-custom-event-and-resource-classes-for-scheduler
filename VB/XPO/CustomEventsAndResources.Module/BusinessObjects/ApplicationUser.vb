Imports System.ComponentModel
Imports System.Drawing
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.BaseImpl.PermissionPolicy
Imports DevExpress.Xpo
Imports DomainComponents.Common

Namespace CustomEventsAndResources.Module.BusinessObjects

    <MapInheritance(MapInheritanceType.ParentTable)>
    <DefaultProperty(NameOf(PermissionPolicyUser.UserName))>
    Public Class ApplicationUser
        Inherits PermissionPolicyUser
        Implements ISecurityUserWithLoginInfo, IResource

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub

        <Browsable(False)>
        <Aggregated, Association("User-LoginInfo")>
        Public ReadOnly Property LoginInfo As XPCollection(Of ApplicationUserLoginInfo)
            Get
                Return GetCollection(Of ApplicationUserLoginInfo)(NameOf(ApplicationUser.LoginInfo))
            End Get
        End Property

        Private ReadOnly Property UserLogins As IEnumerable(Of ISecurityUserLoginInfo)
            Get
                Return LoginInfo.OfType(Of ISecurityUserLoginInfo)()
            End Get
        End Property

        Private Function CreateUserLoginInfo(ByVal loginProviderName As String, ByVal providerUserKey As String) As ISecurityUserLoginInfo Implements ISecurityUserWithLoginInfo.CreateUserLoginInfo
            Dim result As ApplicationUserLoginInfo = New ApplicationUserLoginInfo(Session)
            result.LoginProviderName = loginProviderName
            result.ProviderUserKey = providerUserKey
            result.UserProp = Me
            Return result
        End Function

#Region "Resource"
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            Color = Color.White
        End Sub

        <Browsable(False)>
        Public ReadOnly Property Id As Object Implements IResource.Id
            Get
                Return Me.Oid
            End Get
        End Property

        <NonPersistent>
        Public Property Caption As String Implements IResource.Caption
            Get
                Return UserName
            End Get

            Set(ByVal value As String)
                UserName = value
            End Set
        End Property

        <Browsable(False)>
        Public ReadOnly Property OleColor As Int32
            Get
                Return ColorTranslator.ToOle(Color)
            End Get
        End Property

        <Association("CustomEventWithUserResources-ApplicationUser")>
        Public ReadOnly Property Events As XPCollection(Of CustomEventWithUserResources)
            Get
                Return GetCollection(Of CustomEventWithUserResources)(NameOf(ApplicationUser.Events))
            End Get
        End Property

        <ValueConverter(GetType(ColorToIntConverter))>
        Public Property Color As Color
            Get
                Return GetPropertyValue(Of Color)(NameOf(ApplicationUser.Color))
            End Get

            Set(ByVal value As Color)
                SetPropertyValue(NameOf(ApplicationUser.Color), value)
            End Set
        End Property
#End Region
    End Class
End Namespace
