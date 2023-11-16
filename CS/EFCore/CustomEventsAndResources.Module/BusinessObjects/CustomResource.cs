using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DevExpress.Persistent.Base.General;
using System.ComponentModel.DataAnnotations;

namespace CustomEventsAndResources.Module.BusinessObjects; 
//[DefaultProperty(nameof(Caption))]
public class CustomResource : IResource, IXafEntityObject {
    [Key, VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
    public virtual Guid Key { get; set; }
    public virtual String Caption { get; set; }
    [Browsable(false)]
    public virtual Int32 Color_Int { get; protected set; }
    public virtual IList<CustomEventWithCustomResource> Events { get; set; } = new ObservableCollection<CustomEventWithCustomResource>();

    [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
    public Object Id => Key;
    [VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
    public Int32 OleColor => ColorTranslator.ToOle(Color.FromArgb(Color_Int));
    [NotMapped]
    public Color Color {
        get { return Color.FromArgb(Color_Int); }
        set { Color_Int = value.ToArgb(); }
    }
    // IXafEntityObject
    public virtual void OnCreated() {
        Color = Color.White;
    }
    public virtual void OnSaving() { }
    public virtual void OnLoaded() { }
}
