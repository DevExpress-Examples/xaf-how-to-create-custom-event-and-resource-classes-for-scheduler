Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Filtering
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.SystemModule.Notifications
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.Base.General.Compatibility
Imports DevExpress.Persistent.BaseImpl.EF
Imports DevExpress.Persistent.Validation
Imports DevExpress.XtraScheduler.Xml
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports System.Xml

Namespace CustomEventsAndResources.Module.BusinessObjects

    <DefaultClassOptions>
    Public Class CustomEventWithUserResources
        Inherits BaseObject
        Implements IEvent, IXafEntityObject, IRecurrentEvent, IReminderEvent

#Region "Base Properties"
        <Browsable(False)>
        Public ReadOnly Property AppointmentId As Object Implements IEvent.AppointmentId
            Get
                Return Me.ID
            End Get
        End Property

        <Browsable(False)>
        Public Overridable Property Type As Integer Implements IEvent.Type ' event type - Normal, Recurrence Pattern or Recurrence Exception

        Public Overridable Property Subject As String Implements IEvent.Subject

        Public Overridable Property Description As String Implements IEvent.Description

        <ImmediatePostData> ' needed for immediate update of StartOn/EndOn properties 
        Public Overridable Property AllDay As Boolean Implements IEvent.AllDay

        Public Overridable Property Location As String Implements IEvent.Location

        Public Overridable Property Label As Integer Implements IEvent.Label

        Public Overridable Property Status As Integer Implements IEvent.Status

        Public Overridable Property StartOn As Date?

        Private Property StartOn As Date
            Get
                Return If(Me.StartOn.HasValue, Me.StartOn.Value, Date.MinValue)
            End Get

            Set(ByVal value As Date)
                Me.StartOn = value
            End Set
        End Property

        Public Overridable Property EndOn As Date?

        Private Property EndOn As Date
            Get
                Return If(Me.EndOn.HasValue, Me.EndOn.Value, Date.MinValue)
            End Get

            Set(ByVal value As Date)
                Me.EndOn = value
            End Set
        End Property

#End Region
#Region "Resources"
        Private isUpdateResourcesDelayed As Boolean

        Private resourceIdField As String

        Public Overrides Sub OnSaving() Implements IXafEntityObject.OnSaving
            If ObjectSpace IsNot Nothing AndAlso isUpdateResourcesDelayed Then
                isUpdateResourcesDelayed = False
                UpdateResources()
            End If
        End Sub

        Public Overrides Sub OnLoaded() Implements IXafEntityObject.OnLoaded
            Dim count As Integer = Resources.Count
        End Sub

        Private Sub UpdateResources()
            While Resources.Count > 0
                Resources.RemoveAt(Resources.Count - 1)
            End While

            If Not String.IsNullOrEmpty(resourceIdField) Then
                Dim xmlDocument As XmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceIdField)
                For Each xmlNode As XmlNode In xmlDocument.DocumentElement.ChildNodes
                    Dim loader As AppointmentResourceIdXmlLoader = New AppointmentResourceIdXmlLoader(xmlNode)
                    Dim keyMemberValue As Object = loader.ObjectFromXml()
                    Dim obj As Object = ObjectSpace.GetObjectByKey(GetType(ApplicationUser), keyMemberValue)
                    If obj IsNot Nothing Then
                        Resources.Add(CType(obj, ApplicationUser))
                    End If
                Next
            End If
        End Sub

        Public Sub UpdateResourceIds()
            resourceIdField = "<ResourceIds>" & Microsoft.VisualBasic.Constants.vbCrLf
            For Each resource As ApplicationUser In Resources
                resourceIdField += String.Format("<ResourceId Type=""{0}"" Value=""{1}"" />" & Microsoft.VisualBasic.Constants.vbCrLf, resource.ID.GetType().FullName, resource.ID)
            Next

            resourceIdField += "</ResourceIds>"
        End Sub

        Public Overridable Property Resources As IList(Of ApplicationUser) = New ObservableCollection(Of ApplicationUser)()

        <NotMapped, Browsable(False)>
        Public Overridable Property ResourceId As String Implements IEvent.ResourceId
            Get
                If Equals(resourceIdField, Nothing) Then
                    UpdateResourceIds()
                End If

                Return resourceIdField
            End Get

            Set(ByVal value As String)
                If Not Equals(resourceIdField, value) Then
                    resourceIdField = value
                    If ObjectSpace IsNot Nothing Then
                        UpdateResources()
                    Else
                        isUpdateResourcesDelayed = True
                    End If
                End If
            End Set
        End Property

#End Region
#Region "IRecurrentEvent"
        <Browsable(False)>
        Public Overridable Property RecurrencePatternProp As CustomEventWithUserResources

        <Browsable(False)>
        Public Overridable Property RecurrenceEvents As IList(Of CustomEventWithUserResources) = New ObservableCollection(Of CustomEventWithUserResources)()

        <StringLength(300)>
        <NonCloneable, DisplayName("Recurrence"), FieldSize(FieldSizeAttribute.Unlimited)>
        Public Overridable Property RecurrenceInfoXml As String Implements IRecurrentEvent.RecurrenceInfoXml

        Private Property RecurrencePattern As IRecurrentEvent Implements IRecurrentEvent.RecurrencePattern
            Get
                Return RecurrencePatternProp
            End Get

            Set(ByVal value As IRecurrentEvent)
                RecurrencePatternProp = CType(value, CustomEventWithUserResources)
            End Set
        End Property

#End Region
#Region "IReminderEvent"
        Private Const NoneReminder As Integer = -1

        Private postponeTimes As IList(Of PostponeTime)

        Private reminderInfoXmlField As String

        Private remindInSecondsField As Integer = NoneReminder

        Private alarmTimeField As Date?

        Private Sub UpdateRemindersInfoXml(ByVal UpdateAlarmTime As Boolean)
            If RemindIn.HasValue AndAlso AlarmTime.HasValue Then
                Dim apptReminder As AppointmentReminderInfo = New AppointmentReminderInfo()
                Dim reminderInfo As ReminderInfo = New ReminderInfo()
                reminderInfo.TimeBeforeStart = RemindIn.Value
                Dim notNullableStartOn As Date = If(Me.StartOn.HasValue, Me.StartOn.Value, Date.MinValue)
                If UpdateAlarmTime Then
                    reminderInfo.AlertTime = AlarmTime.Value
                Else
                    reminderInfo.AlertTime = notNullableStartOn - RemindIn.Value
                End If

                apptReminder.ReminderInfos.Add(reminderInfo)
                reminderInfoXmlField = apptReminder.ToXml()
            Else
                reminderInfoXmlField = Nothing
            End If
        End Sub

        Private Sub UpdateAlarmTime()
            If Not String.IsNullOrEmpty(reminderInfoXmlField) Then
                Dim appointmentReminderInfo As AppointmentReminderInfo = New AppointmentReminderInfo()
                Try
                    appointmentReminderInfo.FromXml(reminderInfoXmlField)
                    alarmTimeField = appointmentReminderInfo.ReminderInfos(CInt(0)).AlertTime
                Catch e As XmlException
                    Tracing.Tracer.LogError(e)
                End Try
            Else
                alarmTimeField = Nothing
                remindInSecondsField = NoneReminder
                IsPostponed = False
            End If
        End Sub

        Private Function CreatePostponeTimes() As IList(Of PostponeTime)
            Dim result As IList(Of PostponeTime) = PostponeTime.CreateDefaultPostponeTimesList()
            result.Add(New PostponeTime("None", Nothing, "None"))
            result.Add(New PostponeTime("AtStartTime", TimeSpan.Zero, "0 minutes"))
            Dim args As CustomizeNotificationsPostponeTimeListEventArgs = New CustomizeNotificationsPostponeTimeListEventArgs(result)
            RaiseEvent CustomizeReminderTimeLookup(Me, args)
            PostponeTime.SortPostponeTimesList(args.PostponeTimesList)
            Return args.PostponeTimesList
        End Function

        <ImmediatePostData>
        <NotMapped>
        <ModelDefault("AllowClear", "False")>
        <DataSourceProperty(NameOf(CustomEventWithUserResources.PostponeTimeList))>
        <SearchMemberOptions(SearchMemberMode.Exclude)>
        <VisibleInDetailView(False), VisibleInListView(False)>
        Public Overridable Property ReminderTime As PostponeTime
            Get
                If RemindIn.HasValue Then
                    Return PostponeTimeList.Where(Function(x) x.RemindIn IsNot Nothing AndAlso x.RemindIn.Value = RemindIn.Value).FirstOrDefault()
                Else
                    Return PostponeTimeList.Where(Function(x) x.RemindIn Is Nothing).FirstOrDefault()
                End If
            End Get

            Set(ByVal value As PostponeTime)
                If value IsNot Nothing Then
                    If value.RemindIn.HasValue Then
                        RemindIn = value.RemindIn.Value
                    Else
                        RemindIn = Nothing
                    End If
                End If
            End Set
        End Property

        <Browsable(False), NotMapped>
        Public ReadOnly Property PostponeTimeList As IEnumerable(Of PostponeTime)
            Get
                If postponeTimes Is Nothing Then
                    postponeTimes = CreatePostponeTimes()
                End If

                Return postponeTimes
            End Get
        End Property

        <NonCloneable>
        <Browsable(False)>
        <StringLength(200)>
        Public Overridable Property ReminderInfoXml As String Implements IReminderEvent.ReminderInfoXml
            Get
                Return reminderInfoXmlField
            End Get

            Set(ByVal value As String)
                If Not Equals(reminderInfoXmlField, value) Then
                    reminderInfoXmlField = value
                    If ObjectSpace IsNot Nothing Then
                        UpdateAlarmTime()
                    End If
                End If
            End Set
        End Property

        <Browsable(False)>
        <NotMapped>
        Public Property RemindIn As TimeSpan?
            Get
                If remindInSecondsField < 0 Then
                    Return Nothing
                End If

                Return TimeSpan.FromSeconds(remindInSecondsField)
            End Get

            Set(ByVal value As TimeSpan?)
                remindInSecondsField = If(value.HasValue, CInt(value.Value.TotalSeconds), NoneReminder)
            End Set
        End Property

        <Browsable(False)>
        Public Overridable Property RemindInSeconds As Integer

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public ReadOnly Property NotificationMessage As String Implements ISupportNotifications.NotificationMessage
            Get
                Return Subject
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property UniqueId As Object Implements ISupportNotifications.UniqueId
            Get
                Return Me.ID
            End Get
        End Property

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public Overridable Property AlarmTime As Date?
            Get
                Return alarmTimeField
            End Get

            Set(ByVal value As Date?)
                If alarmTimeField <> value Then
                    alarmTimeField = value
                    If ObjectSpace IsNot Nothing Then
                        If value Is Nothing Then
                            remindInSecondsField = NoneReminder
                            IsPostponed = False
                        End If

                        UpdateRemindersInfoXml(True)
                    End If
                End If
            End Set
        End Property

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public Overridable Property IsPostponed As Boolean Implements ISupportNotifications.IsPostponed

        Public Event CustomizeReminderTimeLookup As EventHandler(Of CustomizeNotificationsPostponeTimeListEventArgs)

#End Region
#Region "Blazor compatibility"
        <NotMapped, Browsable(False)>
        Public Property ResourceIdBlazor As Object
            Get
                If Resources.Count = 1 Then
                    Return Resources(CInt(0)).ID
                End If

                Return Nothing
            End Get

            Set(ByVal value As Object)
                While Resources.Count > 0
                    Resources.RemoveAt(Resources.Count - 1)
                End While

                If value IsNot Nothing Then
                    Resources.Add(ObjectSpace.GetObjectByKey(Of ApplicationUser)(value))
                End If
            End Set
        End Property

        <NotMapped, Browsable(False)>
        Public Property RecurrenceInfoXmlBlazor As String
            Get
                Return RecurrenceInfoXml?.ToNewRecurrenceInfoXml()
            End Get

            Set(ByVal value As String)
                RecurrenceInfoXml = value?.ToOldRecurrenceInfoXml()
            End Set
        End Property

#End Region
#Region "Construction"
        Public Overrides Sub OnCreated() Implements IXafEntityObject.OnCreated
            MyBase.OnCreated()
            Dim now = Date.Now
            Me.StartOn = now
            Me.EndOn = now.AddHours(1)
        End Sub

#End Region
#Region "Validation"
        <Browsable(False)>
        <RuleFromBoolProperty("CustomEventWithUserResourcesIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues:=False, UsedProperties:="StartOn, EndOn")>
        Public ReadOnly Property IsIntervalValid As Boolean
            Get
                Return Me.StartOn <= Me.EndOn
            End Get
        End Property
#End Region
    End Class
End Namespace
