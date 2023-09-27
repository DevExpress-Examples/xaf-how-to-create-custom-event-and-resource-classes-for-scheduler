Imports DevExpress.ExpressApp.Filtering
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.SystemModule.Notifications
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.Base.General
Imports DevExpress.Persistent.Base.General.Compatibility
Imports DevExpress.Persistent.Validation
Imports DevExpress.XtraScheduler.Xml
Imports System.ComponentModel
Imports System.Xml
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace CustomEventsAndResources.Module.BusinessObjects

    <DefaultClassOptions>
    Public Class CustomEventWithUserResources
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
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithUserResources.Subject))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Subject), value)
            End Set
        End Property

        Public Property Description As String Implements IEvent.Description
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithUserResources.Description))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Description), value)
            End Set
        End Property

        <Indexed>
        Public Property StartOn As DateTime
            Get
                Return GetPropertyValue(Of DateTime)(NameOf(CustomEventWithUserResources.StartOn))
            End Get

            Set(ByVal value As DateTime)
                SetPropertyValue(NameOf(CustomEventWithUserResources.StartOn), value)
            End Set
        End Property

        <Indexed>
        Public Property EndOn As DateTime
            Get
                Return GetPropertyValue(Of DateTime)(NameOf(CustomEventWithUserResources.EndOn))
            End Get

            Set(ByVal value As DateTime)
                SetPropertyValue(NameOf(CustomEventWithUserResources.EndOn), value)
            End Set
        End Property

        <ImmediatePostData>
        Public Property AllDay As Boolean Implements IEvent.AllDay
            Get
                Return GetPropertyValue(Of Boolean)(NameOf(CustomEventWithUserResources.AllDay))
            End Get

            Set(ByVal value As Boolean)
                SetPropertyValue(NameOf(CustomEventWithUserResources.AllDay), value)
            End Set
        End Property

        Public Property Location As String Implements IEvent.Location
            Get
                Return GetPropertyValue(Of String)(NameOf(CustomEventWithUserResources.Location))
            End Get

            Set(ByVal value As String)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Location), value)
            End Set
        End Property

        Public Property Label As Integer Implements IEvent.Label
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithUserResources.Label))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Label), value)
            End Set
        End Property

        Public Property Status As Integer Implements IEvent.Status
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithUserResources.Status))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Status), value)
            End Set
        End Property

        <Browsable(False)>
        Public Property Type As Integer Implements IEvent.Type
            Get
                Return GetPropertyValue(Of Integer)(NameOf(CustomEventWithUserResources.Type))
            End Get

            Set(ByVal value As Integer)
                SetPropertyValue(NameOf(CustomEventWithUserResources.Type), value)
            End Set
        End Property

#End Region
#Region "Resources"
        <Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(GetType(ObjectValidatorLargeNonDelayedMember))>
        Private resourceIds As [String]

        Private Sub UpdateResources()
            Resources.SuspendChangedEvents()
            Try
                While Resources.Count > 0
                    Resources.Remove(Resources(0))
                End While

                If Not [String].IsNullOrEmpty(resourceIds) Then
                    Dim xmlDocument As XmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceIds)
                    For Each xmlNode As XmlNode In xmlDocument.DocumentElement.ChildNodes
                        Dim loader As AppointmentResourceIdXmlLoader = New AppointmentResourceIdXmlLoader(xmlNode)
                        Dim keyMemberValue As [Object] = loader.ObjectFromXml()
                        Dim resource As ApplicationUser = Session.GetObjectByKey(Of ApplicationUser)(New Guid(keyMemberValue.ToString()))
                        If resource IsNot Nothing Then
                            Resources.Add(resource)
                        End If
                    Next
                End If
            Finally
                Resources.ResumeChangedEvents()
            End Try
        End Sub

        Private Sub Resources_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
            If e.ListChangedType Is ListChangedType.ItemAdded OrElse e.ListChangedType Is ListChangedType.ItemDeleted Then
                UpdateResourceIds()
                OnChanged(NameOf(CustomEventWithUserResources.ResourceId))
            End If
        End Sub

        Protected Overrides Sub OnLoaded()
            MyBase.OnLoaded()
            ' B136420, Q255811
            If Resources.IsLoaded AndAlso Not Session.IsNewObject(Me) Then
                Resources.Reload()
            End If
        End Sub

        Public Sub New(ByVal session As Session)
            MyBase.New(session)
            Resources.ListChanged += New ListChangedEventHandler(AddressOf Resources_ListChanged)
        End Sub

        Public Sub UpdateResourceIds()
            resourceIds = "<ResourceIds>" & Microsoft.VisualBasic.Constants.vbCrLf
            For Each resource As ApplicationUser In Resources
                resourceIds += String.Format("<ResourceId Type=""{0}"" Value=""{1}"" />" & Microsoft.VisualBasic.Constants.vbCrLf, resource.Id.GetType().FullName, resource.Id)
            Next

            resourceIds += "</ResourceIds>"
        End Sub

        <Association("CustomEventWithUserResources-ApplicationUser")>
        Public ReadOnly Property Resources As XPCollection(Of ApplicationUser)
            Get
                Return GetCollection(Of ApplicationUser)(NameOf(CustomEventWithUserResources.Resources))
            End Get
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
                    UpdateResources()
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
                SetPropertyValue(NameOf(CustomEventWithUserResources.RecurrenceInfoXml), recurrenceInfoXmlField, value)
            End Set
        End Property

        <PersistentAlias(NameOf(recurrencePatternField))>
        Public Property RecurrencePattern As IRecurrentEvent Implements IRecurrentEvent.RecurrencePattern
            Get
                Return recurrencePatternField
            End Get

            Set(ByVal value As IRecurrentEvent)
                SetPropertyValue(NameOf(CustomEventWithUserResources.RecurrencePattern), recurrencePatternField, CType(value, [Event]))
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
        <DataSourceProperty(NameOf(CustomEventWithUserResources.PostponeTimeList))>
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
                SetPropertyValue(NameOf(CustomEventWithUserResources.RemindIn), remindInField, value)
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
                SetPropertyValue(NameOf(CustomEventWithUserResources.ReminderInfoXml), reminderInfoXmlField, value)
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
                SetPropertyValue(NameOf(CustomEventWithUserResources.AlarmTime), alarmTimeField, value)
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
                SetPropertyValue(NameOf(CustomEventWithUserResources.IsPostponed), isPostponedField, value)
            End Set
        End Property

        Public Event CustomizeReminderTimeLookup As EventHandler(Of CustomizeNotificationsPostponeTimeListEventArgs)

#End Region
#Region "Blazor compatibility"
        <NonPersistent(), Browsable(False)>
        Public Property ResourceIdBlazor As Object
            Get
                If Resources.Count = 1 Then
                    Return Resources(0).Id
                End If

                Return Nothing
            End Get

            Set(ByVal value As Object)
                Resources.SuspendChangedEvents()
                Try
                    While Resources.Count > 0
                        Resources.Remove(Resources(0))
                    End While

                    If value IsNot Nothing Then
                        Resources.Add(Session.GetObjectByKey(Of ApplicationUser)(value))
                    End If
                Finally
                    Resources.ResumeChangedEvents()
                End Try
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
        Public Overrides Sub AfterConstruction()
            MyBase.AfterConstruction()
            StartOn = DateTime.Now
            EndOn = StartOn.AddHours(1)
        End Sub

#End Region
#Region "Validation"
        <Browsable(False)>
        <RuleFromBoolProperty("CustomEventWithUserResourcesIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues:=False, UsedProperties:="StartOn, EndOn")>
        Public ReadOnly Property IsIntervalValid As Boolean
            Get
                Return StartOn <= EndOn
            End Get
        End Property
#End Region
    End Class
End Namespace
