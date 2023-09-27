using System.ComponentModel;
using System.Drawing;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using DomainComponents.Common;

namespace CustomEventsAndResources.Module.BusinessObjects;

[MapInheritance(MapInheritanceType.ParentTable)]
[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, IResource {
    public ApplicationUser(Session session) : base(session) { }

    [Browsable(false)]
    [Aggregated, Association("User-LoginInfo")]
    public XPCollection<ApplicationUserLoginInfo> LoginInfo {
        get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
    }

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey) {
        ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session);
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }
    #region Resource
    public override void AfterConstruction() {
        base.AfterConstruction();
        Color = Color.White;
    }
    [Browsable(false)]
    public object Id {
        get => Oid;
    }
    [NonPersistent]
    public string Caption { get => UserName; set => UserName = value; }
    [Browsable(false)]
    public Int32 OleColor {
        get { return ColorTranslator.ToOle(Color); }
    }
    [Association("CustomEventWithUserResources-ApplicationUser")]
    public XPCollection<CustomEventWithUserResources> Events {
        get { return GetCollection<CustomEventWithUserResources>(nameof(Events)); }
    }
    [ValueConverter(typeof(ColorToIntConverter))]
    public Color Color {
        get => GetPropertyValue<Color>(nameof(Color));
        set => SetPropertyValue(nameof(Color), value);
    }
    #endregion
}
