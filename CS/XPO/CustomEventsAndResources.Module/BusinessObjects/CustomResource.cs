using System.ComponentModel;
using System.Drawing;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DomainComponents.Common;
using DevExpress.Persistent.BaseImpl;

namespace CustomEventsAndResources.Module.BusinessObjects; 
//[DefaultProperty(nameof(Caption))]
public class CustomResource : BaseObject, IResource {
    public CustomResource(Session session) : base(session) { }
    public override void AfterConstruction() {
        base.AfterConstruction();
        Color = Color.White;
    }
    [Browsable(false)]
    public object Id {
        get => Oid;
    }
    public string Caption {
        get => GetPropertyValue<string>(nameof(Caption));
        set => SetPropertyValue(nameof(Caption), value);
    }
    [Browsable(false)]
    public Int32 OleColor {
        get { return ColorTranslator.ToOle(Color); }
    }
    [Association("CustomEventWithCustomResource-CustomResource")]
    public XPCollection<CustomEventWithCustomResource> Events {
        get { return GetCollection<CustomEventWithCustomResource>(nameof(Events)); }
    }
    [ValueConverter(typeof(ColorToIntConverter))]
    public Color Color {
        get => GetPropertyValue<Color>(nameof(Color));
        set => SetPropertyValue(nameof(Color), value);
    }
}
