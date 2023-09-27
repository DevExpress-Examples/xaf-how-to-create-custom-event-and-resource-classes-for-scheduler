<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1192223)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# XAF - Create Custom Event and Resource Classes for XAF Scheduler

In XAF Scheduler, you can view and edit Business Objects that implement the `IEvent` interface for [events](https://docs.devexpress.com/eXpressAppFramework/112812/event-planning-and-notifications/scheduler/scheduler-module-overview#events) and the `IResource` interface for [resources](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule).

XAF's [Business Class Library](https://docs.devexpress.com/eXpressAppFramework/112571/business-model-design-orm/built-in-business-classes-and-interfaces) offers the `Event` and `Resource` classes implementing these interfaces that you can use out of the box. However, you may need to implement custom `Event` and `Resource` classes or extend your existing classes.

This example covers the following popular scenarios:

1. [Create custom Event and Resource classes](#create-a-custom-event-class-and-implement-the-ievent-irecurrentevent-ireminderevent-and-ixafentityobject-interfaces).

2. [Use ApplicationUser as a resource in Scheduler](#implement-the-iresource-interface-in-the-applicationuser-class).

3. [Create an event with a single resource](#create-an-event-with-a-single-resource).

> **NOTE**  
> The instructions describe classes specific to Entity Framework Core ORM. For XPO-specific classes refer to the following files:
> * "c:\Program Files\DevExpress 2X.Y\Components\Sources\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Event.cs"
> * "c:\Program Files\DevExpress 2X.Y\Components\Sources\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Resource.cs"

## Create a Custom Event Class and Implement the IEvent, IRecurrentEvent, IReminderEvent, and IXafEntityObject Interfaces

1. Add simple properties. See the [#region Base Properties](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L29) section in the source code. It contains all `IEvent` properties except `ResourceId`.

2. Implement resource support. See the [#region Resources](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L57) section in the source code.
    
    The `IEvent` interface requires a string `ResourceId` property that stores a collection of resources in an XML string as required by the Scheduler control. XAF, we recommend to have an associated collection of objects instead. Add the `ResourceId` string property and the `Resources` collection property and set up a synchronization between them.

3. Implement `IRecurrentEvent`. See the [#region IRecurrentEvent](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L115) section in the source code.

4. Implement `IReminderEvent`. See the [#region IReminderEvent](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L131) section in the source code.

    > **Note**  
    > Currently XAF Scheduler ASP.NET Core Blazor Module does not support reminders. Skip this section for ASP.NET Core Blazor applications.

5. Add Blazor-compatibility options. See the [#region Blazor compatibility](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L272) section in the source code.

    Skip this step if you do not plan to implement the `Event` class in an XAF ASP.NET Core Blazor application.

    > [!NOTE]
    > Currently, XAF Blazor Scheduler Module supports only one resource. You need the `ResourceIdBlazor` property to select a resource from the `Resources` collection. You can implement you own resource selection logic.

6. Implement object construction. See the [#region Construction](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L296) section in the source code.

    If you need to initialize a property (for example the `StartOn` and `EndOn` properties) when XAF creates a new `CustomEvent`, override the `OnCreated` event.

7. Add validation. See the [#region Validation](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L304) section in the source code.

    If you want to use the [Validation Module](https://docs.devexpress.com/eXpressAppFramework/113684/validation-module), add validation rules to your object. For example, you can demand that a `StartOn` date always precedes an `EndOn` date.

## Implement the IResource Interface in the ApplicationUser Class

1. Implement the `IResource` interface in the [`ApplicationUser`](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/ApplicationUser.cs) class.

   In this example, we implement `IResource.Id` explicitly because `ApplicationUser` already has the `ID` property. This implicit implementation causes an ambiguous exception in Windows Forms applications that use Entity Framework Core ORM. To resolve this issue, remap the Scheduler to the `ID` property directly instead of the `IResource.Id` property. For implementation details, refer to [CustomEventWithUserResourcesController.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Win/Controllers/CustomEventWithUserResourcesController.cs).

3. If you have more than one `IResource` implementation in your application, specify a Resource class for the corresponding Scheduler List View. For more information, refer to the following topic: [Use a Custom Resource Class](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule#use-a-custom-resource-class).

## Use Application Model to Customize Objects in Scheduler

1. Enable drag-and-drop in Scheduler.

    By default, Scheduler does not allow to edit objects. You need to set the [AllowEdit](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowEdit) property to `true` in the [Model.DesignedDiffs.xafml](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/Model.DesignedDiffs.xafml) file.

2. Customize the `Start` and `End` properties to show time.

    By default, `DateTime` property editors only display date. Change `DisplayFormat` and `EditMask` to show and edit time. For details, see the [Model.DesignedDiffs.xafml](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/Model.DesignedDiffs.xafml) file.

## Create an Event with a Single Resource

1. If you want to create One-to-Many relationship between a resource and events, change the `ResourceId` implementation. For details, see the following source file: [CustomEventWithCustomResource.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithCustomResource.cs).

2. Create a custom `Resource` class and implement the `IResource` interface. For implementation details, see the following source file: [CustomResource.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomResource.cs).

## Files to Review

- [CustomEventWithUserResources.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs)
- [ApplicationUser.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/ApplicationUser.cs)
- [CustomEventWithUserResourcesController.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Win/Controllers/CustomEventWithUserResourcesController.cs)
- [CustomResource.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomResource.cs)
- [CustomEventWithCustomResource.cs](https://github.com/DevExpress-Examples/xaf-how-to-create-csutom-event-and-resource-classes-for-scheduler/blob/23.1.4%2B/CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithCustomResource.cs)

## Documentation

- [Scheduler Module Overview](https://docs.devexpress.com/eXpressAppFramework/112812/event-planning-and-notifications/scheduler/scheduler-module-overview)
- [Resources in a Schedule](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule)
- [Recurring Events](https://docs.devexpress.com/eXpressAppFramework/113128/event-planning-and-notifications/scheduler/recurring-events)
