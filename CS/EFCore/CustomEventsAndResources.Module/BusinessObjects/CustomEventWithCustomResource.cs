using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General.Compatibility;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;
using DevExpress.XtraScheduler.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Security.AccessControl;

namespace CustomEventsAndResources.Module.BusinessObjects; 
[DefaultClassOptions]
public class CustomEventWithCustomResource : BaseObject, IEvent, IXafEntityObject, IRecurrentEvent, IReminderEvent {
    #region Base Properties
    [Browsable(false)]
    public Object AppointmentId => ID;
    [Browsable(false)]
    public virtual Int32 Type { get; set; } // event type - Normal, Recurrence Pattern or Recurrence Exception
    public virtual String Subject { get; set; }
    public virtual String Description { get; set; }
    [ImmediatePostData]
    public virtual Boolean AllDay { get; set; }

    public virtual String Location { get; set; }

    public virtual Int32 Label { get; set; }

    public virtual Int32 Status { get; set; }
    public virtual Nullable<DateTime> StartOn { get; set; }
    DateTime IEvent.StartOn {
        get { return StartOn.HasValue ? StartOn.Value : DateTime.MinValue; }
        set { StartOn = value; }
    }
    public virtual Nullable<DateTime> EndOn { get; set; }
    DateTime IEvent.EndOn {
        get { return EndOn.HasValue ? EndOn.Value : DateTime.MinValue; }
        set { EndOn = value; }
    }
    #endregion
    #region Resources
    private Boolean isUpdateResourcesDelayed;
    private String resourceId;
    public override void OnSaving() {
        if ((ObjectSpace != null) && isUpdateResourcesDelayed) {
            isUpdateResourcesDelayed = false;
            UpdateResource();
        }
    }
    private void UpdateResource() {
        Resource = null;
        if (!String.IsNullOrEmpty(resourceId)) {
            XmlDocument xmlDocument = DevExpress.Utils.SafeXml.CreateDocument(resourceId);
            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes) {
                AppointmentResourceIdXmlLoader loader = new AppointmentResourceIdXmlLoader(xmlNode);
                Object keyMemberValue = loader.ObjectFromXml();
                Object obj = ObjectSpace.GetObjectByKey(typeof(CustomResource), keyMemberValue);
                if (obj != null) {
                    Resource = (CustomResource)obj;
                }
            }
        }
    }
    public void UpdateResourceIds() {
        resourceId = "<ResourceIds>\r\n";
        if (Resource != null) {
            resourceId += String.Format("<ResourceId Type=\"{0}\" Value=\"{1}\" />\r\n", Resource.Id.GetType().FullName, Resource.Id);
        }
        resourceId += "</ResourceIds>";
    }
    public virtual CustomResource Resource { get; set; }
    [NotMapped, Browsable(false)]
    public virtual String ResourceId {
        get {
            if (resourceId == null) {
                UpdateResourceIds();
            }
            return resourceId;
        }
        set {
            if (resourceId != value) {
                resourceId = value;
                if (ObjectSpace != null) {
                    UpdateResource();
                } else {
                    isUpdateResourcesDelayed = true;
                }
            }
        }
    }


    #endregion
    #region IRecurrentEvent
    [Browsable(false)]
    public virtual CustomEventWithCustomResource RecurrencePattern { get; set; }

    [Browsable(false)]
    public virtual IList<CustomEventWithCustomResource> RecurrenceEvents { get; set; } = new ObservableCollection<CustomEventWithCustomResource>();

    [StringLength(300)]
    [NonCloneable, DisplayName("Recurrence"), FieldSize(FieldSizeAttribute.Unlimited)]
    public virtual String RecurrenceInfoXml { get; set; }

    IRecurrentEvent IRecurrentEvent.RecurrencePattern {
        get { return RecurrencePattern; }
        set { RecurrencePattern = (CustomEventWithCustomResource)value; }
    }
    #endregion
    #region IReminderEvent
    private const int NoneReminder = -1;
    private IList<PostponeTime> postponeTimes;
    private string reminderInfoXml;
    private int remindInSeconds = NoneReminder;
    private DateTime? alarmTime;
    private void UpdateRemindersInfoXml(bool UpdateAlarmTime) {
        if (RemindIn.HasValue && AlarmTime.HasValue) {
            AppointmentReminderInfo apptReminder = new AppointmentReminderInfo();
            ReminderInfo reminderInfo = new ReminderInfo();
            reminderInfo.TimeBeforeStart = RemindIn.Value;
            DateTime notNullableStartOn = StartOn.HasValue ? StartOn.Value : DateTime.MinValue;
            if (UpdateAlarmTime) {
                reminderInfo.AlertTime = AlarmTime.Value;
            } else {
                reminderInfo.AlertTime = notNullableStartOn - RemindIn.Value;
            }
            apptReminder.ReminderInfos.Add(reminderInfo);
            reminderInfoXml = apptReminder.ToXml();
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
            remindInSeconds = NoneReminder;
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
    [NotMapped]
    [ModelDefault("AllowClear", "False")]
    [DataSourceProperty(nameof(PostponeTimeList))]
    [SearchMemberOptions(SearchMemberMode.Exclude)]
    [VisibleInDetailView(false), VisibleInListView(false)]
    public virtual PostponeTime ReminderTime {
        get {
            if (RemindIn.HasValue) {
                return PostponeTimeList.Where(x => (x.RemindIn != null && x.RemindIn.Value == RemindIn.Value)).FirstOrDefault();
            } else {
                return PostponeTimeList.Where(x => x.RemindIn == null).FirstOrDefault();
            }
        }
        set {
            if (value != null) {
                if (value.RemindIn.HasValue) {
                    RemindIn = value.RemindIn.Value;
                } else {
                    RemindIn = null;
                }
            }
        }
    }
    [Browsable(false), NotMapped]
    public IEnumerable<PostponeTime> PostponeTimeList {
        get {
            if (postponeTimes == null) {
                postponeTimes = CreatePostponeTimes();
            }
            return postponeTimes;
        }
    }
    [NonCloneable]
    [Browsable(false)]
    [StringLength(200)]
    public virtual string ReminderInfoXml {
        get { return reminderInfoXml; }
        set {
            if (reminderInfoXml != value) {
                reminderInfoXml = value;
                if (ObjectSpace != null) {
                    UpdateAlarmTime();
                }
            }
        }
    }
    [Browsable(false)]
    [NotMapped]
    public TimeSpan? RemindIn {
        get {
            if (remindInSeconds < 0) {
                return null;
            }
            return TimeSpan.FromSeconds(remindInSeconds);
        }
        set {
            remindInSeconds = value.HasValue ? (int)value.Value.TotalSeconds : NoneReminder;
        }
    }
    [Browsable(false)]
    public virtual int RemindInSeconds { get; set; }

    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public string NotificationMessage {
        get { return Subject; }
    }
    [Browsable(false)]
    public object UniqueId {
        get { return ID; }
    }
    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public virtual DateTime? AlarmTime {
        get { return alarmTime; }
        set {
            if (alarmTime != value) {
                alarmTime = value;
                if (ObjectSpace != null) {
                    if (value == null) {
                        remindInSeconds = NoneReminder;
                        IsPostponed = false;
                    }
                    UpdateRemindersInfoXml(true);
                }
            }
        }
    }
    [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
    public virtual bool IsPostponed { get; set; }
    public event EventHandler<CustomizeNotificationsPostponeTimeListEventArgs> CustomizeReminderTimeLookup;

    #endregion
    #region Blazor compatibility
    [NotMapped, Browsable(false)]
    public object ResourceIdBlazor
    {
        get => Resource?.Id;
        set
        {
            Resource = null;
            if (value != null)
            {
                Resource = ObjectSpace.GetObjectByKey<CustomResource>(value);
            }
        }
    }
    [NotMapped, Browsable(false)]
    public string RecurrenceInfoXmlBlazor {
        get { return RecurrenceInfoXml?.ToNewRecurrenceInfoXml(); }
        set { RecurrenceInfoXml = value?.ToOldRecurrenceInfoXml(); }
    }
    #endregion
    #region Construction
    public override void OnCreated() {
        base.OnCreated();
        var now = DateTime.Now;
        StartOn = now;
        EndOn = now.AddHours(1);
    }
    #endregion
    #region Validation
    [Browsable(false)]
    [RuleFromBoolProperty("CustomEventWithCustomResourceIntervalValid", DefaultContexts.Save, "The start date must be less than the end date", SkipNullOrEmptyValues = false, UsedProperties = "StartOn, EndOn")]
    public Boolean IsIntervalValid {
        get { return StartOn <= EndOn; }
    }
    #endregion
}
