using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.Xpo;

namespace CustomEventsAndResources.Module.BusinessObjects;

[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, IResource {
    public ApplicationUser() : base() {
        UserLogins = new ObservableCollection<ApplicationUserLoginInfo>();
    }

    [Browsable(false)]
    [DevExpress.ExpressApp.DC.Aggregated]
    public virtual IList<ApplicationUserLoginInfo> UserLogins { get; set; }

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => UserLogins.OfType<ISecurityUserLoginInfo>();

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
        ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }
    #region Resource

    [NotMapped]
    public Color Color {
        get { return Color.FromArgb(ColorInt); }
        set { ColorInt = value.ToArgb(); }
    }
    [Browsable(false)]
    public virtual Int32 ColorInt { get; protected set; }
    public virtual IList<CustomEventWithUserResources> Events { get; set; } = new ObservableCollection<CustomEventWithUserResources>();

    [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
    object IResource.Id => base.ID;
    [NotMapped]
    public string Caption { get => UserName; set => UserName = value; }
    [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
    public int OleColor => ColorTranslator.ToOle(Color.FromArgb(ColorInt));
    public override void OnCreated() {
        base.OnCreated();
        Color = Color.White;
    }
    #endregion
}
