using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Base.General.Compatibility;
using DevExpress.Persistent.Validation;
using DevExpress.XtraScheduler.Xml;
using System.ComponentModel;
using System.Xml;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace CustomEventsAndResources.Module.BusinessObjects;

[DefaultClassOptions]
public class CustomEventWithUserResources : BaseObject, IEvent, IRecurrentEvent, IReminderEvent
{
    #region Base Properties
    [Browsable(false)]
    public object AppointmentId {
        get => Oid;
    }
    public string Subject {
        get => GetPropertyValue<string>(nameof(Subject));
        set => SetPropertyValue(nameof(Subject), value);
    }
    public string Description {
        get => GetPropertyValue<string>(nameof(Description));
        set => SetPropertyValue(nameof(Description), value);
    }
    [Indexed]
    public DateTime StartOn {
        get => GetPropertyValue<DateTime>(nameof(StartOn));
        set => SetPropertyValue(nameof(StartOn), value);
    }
    [Indexed]
    public DateTime EndOn {
        get => GetPropertyValue<DateTime>(nameof(EndOn));
        set => SetPropertyValue(nameof(EndOn), value);
    }
    [ImmediatePostData]
    public bool AllDay {
        get => GetPropertyValue<bool>(nameof(AllDay));
        set => SetPropertyValue(nameof(AllDay), value);
    }
    public string Location {
        get => GetPropertyValue<string>(nameof(Location));
        set => SetPropertyValue(nameof(Location), value);
    }
    public int Label {
        get => GetPropertyValue<int>(nameof(Label));
        set => SetPropertyValue(nameof(Label), value);
    }
    public int Status {
        get => GetPropertyValue<int>(nameof(Status));
        set => SetPropertyValue(nameof(Status), value);
    }
    [Browsable(false)]
    public int Type {
        get => GetPropertyValue<int>(nameof(Type));
        set => SetPropertyValue(nameof(Type), value);
    }
    #endregion
    #region Resources
    [Persistent("ResourceIds"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
    private String resourceIds;
    private void UpdateResources() {
        Resources.SuspendChangedEvents();
        try {
            while (Resources.Count > 0) {
                Resources.Remove(Resources[0]);
            }
            if (!String.IsNullOrEmpty(resourceIds)) {
                XmlDocument xmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceIds);
                foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
                    AppointmentResourceIdXmlLoader loader = new AppointmentResourceIdXmlLoader(xmlNode);
                    Object keyMemberValue = loader.ObjectFromXml();
                    ApplicationUser resource = Session.GetObjectByKey<ApplicationUser>(new Guid(keyMemberValue.ToString()));
                    if (resource != null) {
                        Resources.Add(resource);
                    }
                }
            }
        } finally {
            Resources.ResumeChangedEvents();
        }
    }
    private void Resources_ListChanged(object sender, ListChangedEventArgs e) {
        if ((e.ListChangedType == ListChangedType.ItemAdded) ||
            (e.ListChangedType == ListChangedType.ItemDeleted)) {
            UpdateResourceIds();
            OnChanged(nameof(ResourceId));
        }
    }

    protected override void OnLoaded() {
        base.OnLoaded();
        // B136420, Q255811
        if (Resources.IsLoaded && !Session.IsNewObject(this)) {
            Resources.Reload();
        }
    }

    public CustomEventWithUserResources(Session session)
        : base(session) {
        Resources.ListChanged += new ListChangedEventHandler(Resources_ListChanged);
    }
    public void UpdateResourceIds() {
        resourceIds = "<ResourceIds>\r\n";
        foreach (ApplicationUser resource in Resources) {
            resourceIds += string.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", resource.Id.GetType().FullName, resource.Id);
        }
        resourceIds += "</ResourceIds>";
    }

    [Association("CustomEventWithUserResources-ApplicationUser")]
    public XPCollection<ApplicationUser> Resources {
        get { return GetCollection<ApplicationUser>(nameof(Resources)); }
    }
    [NonPersistent(), Browsable(false)]
    public String ResourceId {
        get {
            if (resourceIds == null) {
                UpdateResourceIds();
            }
            return resourceIds;
        }
        set {
            if (resourceIds != value) {
                resourceIds = value;
                UpdateResources();
            }
        }
    }
    #endregion
    #region IRecurrentEvent
    [Persistent("RecurrencePattern")]
    private Event recurrencePattern;
    private string recurrenceInfoXml;
    [NonCloneable]
    [DevExpress.Xpo.DisplayName("Recurrence"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
    public string RecurrenceInfoXml {
        get { return recurrenceInfoXml; }
        set { SetPropertyValue(nameof(RecurrenceInfoXml), ref recurrenceInfoXml, value); }
    }
    [PersistentAlias(nameof(recurrencePattern))]
    public IRecurrentEvent RecurrencePattern {
        get { return recurrencePattern; }
        set { SetPropertyValue(nameof(RecurrencePattern), ref recurrencePattern, (Event)value); }
    }
    #endregion
    #region IReminderEvent

    private string reminderInfoXml;
    private TimeSpan? remindIn;
    private Nullable<DateTime> alarmTime;
    private IList<PostponeTime> postponeTimes;
    private bool isPostponed;

    private void UpdateRemindersInfoXml(bool UpdateAlarmTime) {
        if (RemindIn.HasValue && AlarmTime.HasValue) {
            AppointmentReminderInfo aptReminder = new AppointmentReminderInfo();
            ReminderInfo reminderInfo = new ReminderInfo();
            reminderInfo.TimeBeforeStart = RemindIn.Value;
            if (UpdateAlarmTime)
                reminderInfo.AlertTime = AlarmTime.Value;
            else
                reminderInfo.AlertTime = StartOn - RemindIn.Value;
            aptReminder.ReminderInfos.Add(reminderInfo);
            reminderInfoXml = aptReminder.ToXml();
        } else {
            reminderInfoXml = null;
        }
    }
    private void UpdateAlarmTime() {
        if (!string.IsNullOrEmpty(reminderInfoXml)) {
            AppointmentReminderInfo appointmentReminderInfo = new AppointmentReminderInfo();
            try {
                appointmentReminderInfo.FromXml(reminderInfoXml);
                alarmTime = appointmentReminderInfo.ReminderInfos[0].AlertTime;
            } catch (XmlException e) {
                Tracing.Tracer.LogError(e);
            }
        } else {
            alarmTime = null;
            remindIn = null;
            IsPostponed = false;
        }
    }
    private IList<PostponeTime> CreatePostponeTimes() {
        IList<PostponeTime> result = PostponeTime.CreateDefaultPostponeTimesList();
        result.Add(new PostponeTime("None", null, "None"));
        result.Add(new PostponeTime("AtStartTime", TimeSpan.Zero, "0 minutes"));
        CustomizeNotificationsPostponeTimeListEventArgs args = new CustomizeNotificationsPostponeTimeListEventArgs(result);
        if (CustomizeReminderTimeLookup != null) {
            CustomizeReminderTimeLookup(this, args);
        }
        PostponeTime.SortPostponeTimesList(args.PostponeTimesList);
        return args.PostponeTimesList;
    }
    [ImmediatePostData]
    [NonPersistent]
    [ModelDefault("AllowClear", "False")]
    [DataSourceProperty(nameof(PostponeTimeList))]
    [SearchMemberOptions(SearchMemberMode.Exclude)]
    [VisibleInDetailView(false), VisibleInListView(false)]
    public PostponeTime ReminderTime {
        get {
            if (RemindIn.HasValue) {
                return PostponeTimeList.Where(x => (x.RemindIn != null && x.RemindIn.Value == remindIn.Value)).FirstOrDefault();
            } else {
                return PostponeTimeList.Where(x => x.RemindIn == null).FirstOrDefault();
            }
        }
        set {
            if (!IsLoading && (value != null)) {
                if (value.RemindIn.HasValue) {
                    RemindIn = value.RemindIn.Value;
                } else {
                    RemindIn = null;
                }
            }
        }
    }
    [Browsable(false)]
    public IEnumerable<PostponeTime> PostponeTimeList {
        get {
            if (postponeTimes == null) {
                postponeTimes = CreatePostponeTimes();
            }
            return postponeTimes;
        }
    }
    [Browsable(false)]
    public TimeSpan? RemindIn {
        get { return remindIn; }
        set {
            SetPropertyValue(nameof(RemindIn), ref remindIn, value);
        }
    }
    [NonCloneable]
    [Browsable(false)]
    [Size(200)]
    public string ReminderInfoXml {
        get { return reminderInfoXml; }
        set {
            SetPropertyValue(nameof(ReminderInfoXml), ref reminderInfoXml, value);
            if (!IsLoading) {
                UpdateAlarmTime();
            }
        }
    }
    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public string NotificationMessage {
        get { return Subject; }
    }
    [Browsable(false)]
    public object UniqueId {
        get { return Oid; }
    }
    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public DateTime? AlarmTime {
        get { return alarmTime; }
        set {
            SetPropertyValue(nameof(AlarmTime), ref alarmTime, value);
            if (!IsLoading) {
                if (value == null) {
                    remindIn = null;
                    IsPostponed = false;
                }
                UpdateRemindersInfoXml(true);
            }
        }
    }
    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public bool IsPostponed {
        get { return isPostponed; }
        set { SetPropertyValue(nameof(IsPostponed), ref isPostponed, value); }
    }
    public event EventHandler<CustomizeNotificationsPostponeTimeListEventArgs> CustomizeReminderTimeLookup;
    #endregion
    #region Blazor compatibility
    [NonPersistent(), Browsable(false)]
    public string RecurrenceInfoXmlBlazor {
        get { return RecurrenceInfoXml?.ToNewRecurrenceInfoXml(); }
        set { RecurrenceInfoXml = value?.ToOldRecurrenceInfoXml(); }
    }
    #endregion
    #region Construction
    public override void AfterConstruction() {
        base.AfterConstruction();
        StartOn = DateTime.Now;
        EndOn = StartOn.AddHours(1);
    }
    #endregion
    #region Validation
    [Browsable(false)]
    [RuleFromBoolProperty("CustomEventWithUserResourcesIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues = false, UsedProperties = "StartOn, EndOn")]
    public bool IsIntervalValid { get { return StartOn <= EndOn; } }
    #endregion
}
