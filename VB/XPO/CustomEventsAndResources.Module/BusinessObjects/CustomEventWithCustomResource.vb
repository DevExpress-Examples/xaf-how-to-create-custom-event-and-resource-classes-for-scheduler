Imports DevExpress.ExpressApp.Filtering
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.SystemModule.Notifications
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Base.General.Compatibility
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.Validation
Imports DevExpress.XtraScheduler.Xml
Imports System.ComponentModel
Imports System.Xml
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace CustomEventsAndResources.Module.BusinessObjects

    <DefaultClassOptions>
    Public Class CustomEventWithCustomResource
        Inherits BaseObject
        Implements IEvent, IRecurrentEvent, IReminderEvent

#Region "Base Properties"
        <Browsable(False)>
        Public ReadOnly Property AppointmentId As Object Implements IEvent.AppointmentId
            Get
                Return Me.Oid
            End Get
        End Property

        Public Property Subject As String Implements IEvent.Subject
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithCustomResource.Subject))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Subject), value)
            End Set
        End Property

        Public Property Description As String Implements IEvent.Description
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithCustomResource.Description))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Description), value)
            End Set
        End Property

        <Indexed>
        Public Property StartOn As DateTime
            Get
                Return GetPropertyValue(Of DateTime)(NameOf(CustomEventWithCustomResource.StartOn))
            End Get

            Set(ByVal value As DateTime)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.StartOn), value)
            End Set
        End Property

        <Indexed>
        Public Property EndOn As DateTime
            Get
                Return GetPropertyValue(Of DateTime)(NameOf(CustomEventWithCustomResource.EndOn))
            End Get

            Set(ByVal value As DateTime)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.EndOn), value)
            End Set
        End Property

        <ImmediatePostData>
        Public Property AllDay As Boolean Implements IEvent.AllDay
            Get
                Return GetPropertyValue(Of Boolean)(NameOf(CustomEventWithCustomResource.AllDay))
            End Get

            Set(ByVal value As Boolean)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.AllDay), value)
            End Set
        End Property

        Public Property Location As String Implements IEvent.Location
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithCustomResource.Location))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Location), value)
            End Set
        End Property

        Public Property Label As Integer Implements IEvent.Label
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithCustomResource.Label))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Label), value)
            End Set
        End Property

        Public Property Status As Integer Implements IEvent.Status
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithCustomResource.Status))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Status), value)
            End Set
        End Property

        <Browsable(False)>
        Public Property Type As Integer Implements IEvent.Type
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithCustomResource.Type))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Type), value)
            End Set
        End Property

#End Region
#Region "Resources"
        <Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(GetType(ObjectValidatorLargeNonDelayedMember))>
        Private resourceIds As [String]

        Private Sub UpdateResource()
            Resource = Nothing
            If Not [String].IsNullOrEmpty(resourceIds) Then
                Dim xmlDocument As XmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceIds)
                For Each xmlNode As XmlNode In xmlDocument.DocumentElement.ChildNodes
                    Dim loader As AppointmentResourceIdXmlLoader = New AppointmentResourceIdXmlLoader(xmlNode)
                    Dim keyMemberValue As [Object] = loader.ObjectFromXml()
                    Dim resource As CustomResource = Session.GetObjectByKey(Of CustomResource)(New Guid(keyMemberValue.ToString()))
                    If resource IsNot Nothing Then
                        Me.Resource = resource
                    End If
                Next
            End If
        End Sub

        Public Sub UpdateResourceIds()
            resourceIds = "<ResourceIds>" & Microsoft.VisualBasic.Constants.vbCrLf
            If Resource IsNot Nothing Then
                resourceIds += String.Format("<ResourceId Type=""{0}"" Value=""{1}"" />" & Microsoft.VisualBasic.Constants.vbCrLf, Resource.Id.GetType().FullName, Resource.Id)
            End If

            resourceIds += "</ResourceIds>"
        End Sub

        <Association("CustomEventWithCustomResource-CustomResource")>
        Public Property Resource As CustomResource
            Get
                Return GetPropertyValue(Of CustomResource)(NameOf(CustomEventWithCustomResource.Resource))
            End Get

            Set(ByVal value As CustomResource)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.Resource), value)
                UpdateResourceIds()
                OnChanged(NameOf(CustomEventWithCustomResource.ResourceId))
            End Set
        End Property

        <NonPersistent(), Browsable(False)>
        Public Property ResourceId As [String]
            Get
                If resourceIds Is Nothing Then
                    UpdateResourceIds()
                End If

                Return resourceIds
            End Get

            Set(ByVal value As [String])
                If resourceIds IsNot value Then
                    resourceIds = value
                    UpdateResource()
                End If
            End Set
        End Property

#End Region
#Region "IRecurrentEvent"
        <Persistent("RecurrencePattern")>
        Private recurrencePatternField As [Event]

        Private recurrenceInfoXmlField As String

        <NonCloneable>
        <DevExpress.Xpo.DisplayName("Recurrence"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(GetType(ObjectValidatorLargeNonDelayedMember))>
        Public Property RecurrenceInfoXml As String Implements IRecurrentEvent.RecurrenceInfoXml
            Get
                Return recurrenceInfoXmlField
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.RecurrenceInfoXml), recurrenceInfoXmlField, value)
            End Set
        End Property

        <PersistentAlias(NameOf(recurrencePatternField))>
        Public Property RecurrencePattern As IRecurrentEvent Implements IRecurrentEvent.RecurrencePattern
            Get
                Return recurrencePatternField
            End Get

            Set(ByVal value As IRecurrentEvent)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.RecurrencePattern), recurrencePatternField, CType(value, [Event]))
            End Set
        End Property

#End Region
#Region "IReminderEvent"
        Private reminderInfoXmlField As String

        Private remindInField As TimeSpan?

        Private alarmTimeField As Nullable(Of DateTime)

        Private postponeTimes As IList(Of PostponeTime)

        Private isPostponedField As Boolean

        Private Sub UpdateRemindersInfoXml(ByVal UpdateAlarmTime As Boolean)
            If RemindIn.HasValue AndAlso AlarmTime.HasValue Then
                Dim aptReminder As AppointmentReminderInfo = New AppointmentReminderInfo()
                Dim reminderInfo As ReminderInfo = New ReminderInfo()
                reminderInfo.TimeBeforeStart = RemindIn.Value
                If UpdateAlarmTime Then
                    reminderInfo.AlertTime = AlarmTime.Value
                Else
                    reminderInfo.AlertTime = StartOn - RemindIn.Value
                End If

                aptReminder.ReminderInfos.Add(reminderInfo)
                reminderInfoXmlField = aptReminder.ToXml()
            Else
                reminderInfoXmlField = Nothing
            End If
        End Sub

        Private Sub UpdateAlarmTime()
            If Not String.IsNullOrEmpty(reminderInfoXmlField) Then
                Dim appointmentReminderInfo As AppointmentReminderInfo = New AppointmentReminderInfo()
                Try
                    appointmentReminderInfo.FromXml(reminderInfoXmlField)
                    Me.alarmTimeField = appointmentReminderInfo.ReminderInfos(CInt(0)).AlertTime
                Catch e As XmlException
                    Tracing.Tracer.LogError(e)
                End Try
            Else
                alarmTimeField = Nothing
                remindInField = Nothing
                IsPostponed = False
            End If
        End Sub

        Private Function CreatePostponeTimes() As IList(Of PostponeTime)
            Dim result As IList(Of PostponeTime) = PostponeTime.CreateDefaultPostponeTimesList()
            result.Add(New PostponeTime("None", Nothing, "None"))
            result.Add(New PostponeTime("AtStartTime", TimeSpan.Zero, "0 minutes"))
            Dim args As CustomizeNotificationsPostponeTimeListEventArgs = New CustomizeNotificationsPostponeTimeListEventArgs(result)
            RaiseEvent CustomizeReminderTimeLookupEvent(Me, args)
            PostponeTime.SortPostponeTimesList(args.PostponeTimesList)
            Return args.PostponeTimesList
        End Function

        <ImmediatePostData>
        <NonPersistent>
        <ModelDefault("AllowClear", "False")>
        <DataSourceProperty(NameOf(CustomEventWithCustomResource.PostponeTimeList))>
        <SearchMemberOptions(SearchMemberMode.Exclude)>
        <VisibleInDetailView(False), VisibleInListView(False)>
        Public Property ReminderTime As PostponeTime
            Get
                If RemindIn.HasValue Then
                    Return PostponeTimeList.Where(Function(x) x.RemindIn IsNot Nothing AndAlso x.RemindIn.Value Is remindInField.Value).FirstOrDefault()
                Else
                    Return PostponeTimeList.Where(Function(x) x.RemindIn Is Nothing).FirstOrDefault()
                End If
            End Get

            Set(ByVal value As PostponeTime)
                If Not IsLoading AndAlso value IsNot Nothing Then
                    If value.RemindIn.HasValue Then
                        RemindIn = value.RemindIn.Value
                    Else
                        RemindIn = Nothing
                    End If
                End If
            End Set
        End Property

        <Browsable(False)>
        Public ReadOnly Property PostponeTimeList As IEnumerable(Of PostponeTime)
            Get
                If postponeTimes Is Nothing Then
                    postponeTimes = CreatePostponeTimes()
                End If

                Return postponeTimes
            End Get
        End Property

        <Browsable(False)>
        Public Property RemindIn As TimeSpan?
            Get
                Return remindInField
            End Get

            Set(ByVal value As TimeSpan?)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.RemindIn), remindInField, value)
            End Set
        End Property

        <NonCloneable>
        <Browsable(False)>
        <Size(200)>
        Public Property ReminderInfoXml As String Implements IReminderEvent.ReminderInfoXml
            Get
                Return reminderInfoXmlField
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.ReminderInfoXml), reminderInfoXmlField, value)
                If Not IsLoading Then
                    UpdateAlarmTime()
                End If
            End Set
        End Property

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public ReadOnly Property NotificationMessage As String Implements ISupportNotifications.NotificationMessage
            Get
                Return Subject
            End Get
        End Property

        <Browsable(False)>
        Public ReadOnly Property UniqueId As Object Implements ISupportNotifications.UniqueId
            Get
                Return Me.Oid
            End Get
        End Property

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public Property AlarmTime As DateTime?
            Get
                Return alarmTimeField
            End Get

            Set(ByVal value As DateTime?)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.AlarmTime), alarmTimeField, value)
                If Not IsLoading Then
                    If value Is Nothing Then
                        remindInField = Nothing
                        IsPostponed = False
                    End If

                    UpdateRemindersInfoXml(True)
                End If
            End Set
        End Property

        <VisibleInDetailView(False), VisibleInListView(False), VisibleInLookupListView(False)>
        Public Property IsPostponed As Boolean Implements ISupportNotifications.IsPostponed
            Get
                Return isPostponedField
            End Get

            Set(ByVal value As Boolean)
                SetPropertyValue(NameOf(CustomEventWithCustomResource.IsPostponed), isPostponedField, value)
            End Set
        End Property

        Public Event CustomizeReminderTimeLookup As EventHandler(Of CustomizeNotificationsPostponeTimeListEventArgs)

#End Region
#Region "Blazor compatibility"
        <NonPersistent(), Browsable(False)>
        Public Property ResourceIdBlazor As Object
            Get
                Return Resource.Id
            End Get

            Set(ByVal value As Object)
                Resource = Nothing
                If value IsNot Nothing Then
                    Resource = Session.GetObjectByKey(Of CustomResource)(value)
                End If
            End Set
        End Property

        <NonPersistent(), Browsable(False)>
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
        Public Sub New(ByVal session As Session)
            MyBase.New(session)
        End Sub

        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            StartOn = DateTime.Now
            EndOn = StartOn.AddHours(1)
        End Sub

#End Region
#Region "Validation"
        <Browsable(False)>
        <RuleFromBoolProperty("CustomEventWithCustomResourceIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues:=False, UsedProperties:="StartOn, EndOn")>
        Public ReadOnly Property IsIntervalValid As Boolean
            Get
                Return StartOn <= EndOn
            End Get
        End Property
#End Region
    End Class
End Namespace
