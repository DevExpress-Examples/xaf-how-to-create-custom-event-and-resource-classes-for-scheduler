using CustomEventsAndResources.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEventsAndResources.Win.Controllers {
    public class CustomEventWithUserResourcesController : ObjectViewController<ListView, CustomEventWithUserResources> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ((SchedulerListEditor)View.Editor).ResourcesMappings.Id = "ID";
        }
    }
}
