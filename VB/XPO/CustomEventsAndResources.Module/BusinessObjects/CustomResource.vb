Imports System.ComponentModel
Imports System.Drawing
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Xpo
Imports DomainComponents.Common
Imports DevExpress.Persistent.BaseImpl

Namespace CustomEventsAndResources.Module.BusinessObjects

    '[DefaultProperty(nameof(Caption))]
    Public Class CustomResource
        Inherits BaseObject
        Implements IResource

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub

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

        Public Property Caption As String Implements IResource.Caption
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomResource.Caption))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomResource.Caption), value)
            End Set
        End Property

        <Browsable(False)>
        Public ReadOnly Property OleColor As Int32
            Get
                Return ColorTranslator.ToOle(Color)
            End Get
        End Property

        <Association("CustomEventWithCustomResource-CustomResource")>
        Public ReadOnly Property Events As XPCollection(Of CustomEventWithCustomResource)
            Get
                Return GetCollection(Of CustomEventWithCustomResource)(NameOf(CustomResource.Events))
            End Get
        End Property

        <ValueConverter(GetType(ColorToIntConverter))>
        Public Property Color As Color
            Get
                Return GetPropertyValue(Of Color)(NameOf(CustomResource.Color))
            End Get

            Set(ByVal value As Color)
                SetPropertyValue(NameOf(CustomResource.Color), value)
            End Set
        End Property
    End Class
End Namespace
