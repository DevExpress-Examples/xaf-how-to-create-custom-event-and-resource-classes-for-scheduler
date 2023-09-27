Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports System
Imports System.Collections.ObjectModel
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Drawing
Imports DevExpress.Persistent.Base.General
Imports System.ComponentModel.DataAnnotations

Namespace CustomEventsAndResources.Module.BusinessObjects

    '[DefaultProperty(nameof(Caption))]
    Public Class CustomResource
        Implements IResource, IXafEntityObject

        <Key, VisibleInListView(False), VisibleInDetailView(False), VisibleInLookupListView(False)>
        Public Overridable Property Key As Guid

        Public Overridable Property Caption As String Implements IResource.Caption

        <Browsable(False)>
        Public Overridable Property Color_Int As Integer

        Public Overridable Property Events As IList(Of CustomEventWithCustomResource) = New ObservableCollection(Of CustomEventWithCustomResource)()

        <VisibleInListView(False), VisibleInDetailView(False), VisibleInLookupListView(False)>
        Public ReadOnly Property Id As Object Implements IResource.Id
            Get
                Return Key
            End Get
        End Property

        <VisibleInListView(False), VisibleInDetailView(False), VisibleInLookupListView(False)>
        Public ReadOnly Property OleColor As Integer Implements IResource.OleColor
            Get
                Return ColorTranslator.ToOle(Color.FromArgb(Color_Int))
            End Get
        End Property

        <NotMapped>
        Public Property Color As Color
            Get
                Return Me.Color.FromArgb(Color_Int)
            End Get

            Set(ByVal value As Color)
                Color_Int = value.ToArgb()
            End Set
        End Property

        ' IXafEntityObject
        Public Overridable Sub OnCreated() Implements IXafEntityObject.OnCreated
            Color = Color.White
        End Sub

        Public Overridable Sub OnSaving() Implements IXafEntityObject.OnSaving
        End Sub

        <SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals")>
        Public Overridable Sub OnLoaded() Implements IXafEntityObject.OnLoaded
            Dim count As Integer = Events.Count
        End Sub
    End Class
End Namespace
