<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/697272304/23.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1192223)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# XAF - Create Custom Event and Resource Classes for XAF Scheduler

Scheduler List Views work with [events](https://docs.devexpress.com/eXpressAppFramework/112812/event-planning-and-notifications/scheduler/scheduler-module-overview#events) (`IEvent` objects) and  [resources](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule) (`IResource` objects). 

XAF's [Business Class Library](https://docs.devexpress.com/eXpressAppFramework/112571/business-model-design-orm/built-in-business-classes-and-interfaces) implements these interfaces in `Event` and `Resource` classes. In some cases, you may need to extend these classes or even implement their corresponding interfaces from scratch.

The example in this repository implements the following scenarios:

1. [Use custom event and resource objects](#implement-custom-events-and-resources).

2. [Use ApplicationUser as a resource](#implement-the-iresource-interface-in-the-applicationuser-class).

3. [Create an event with a single resource](#create-an-event-with-a-single-resource).

> **NOTE**  
> Instructions below describe classes specific to Entity Framework Core ORM. For XPO-specific classes refer to the following files:
> * "c:\Program Files\DevExpress 2X.Y\Components\Sources\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Event.cs"
> * "c:\Program Files\DevExpress 2X.Y\Components\Sources\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Resource.cs"

## Implement Custom Events and Resources
1. Create a new event class and add properties. See the [#region Base Properties](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L29) section in the source code. It contains all `IEvent` properties except `ResourceId`.

2. Implement a resource object. See the [#region Resources](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L57) section in the source code.
    
    The `IEvent` interface requires a string `ResourceId` property that stores a collection of resources in an XML string (required by the Scheduler control). In XAF, we recommend that you add an associated collection of objects instead. Add the `ResourceId` string property and the `Resources` collection property and set up synchronization between them.

3. Implement `IRecurrentEvent`. See the [#region IRecurrentEvent](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L112) section in the source code.

4. Implement `IReminderEvent`. See the [#region IReminderEvent](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L128) section in the source code.

    > **Note**  
    > Reminders are currently not supported in XAF Scheduler Module for ASP.NET Core Blazor. Skip this section for ASP.NET Core Blazor applications.

5. Add Blazor compatibility options. See the [#region Blazor compatibility](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L269) section in the source code. Skip this step if you do not plan to implement the `Event` class in an ASP.NET Core Blazor application.

6. Implement object construction. See the [#region Construction](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L293) section in the source code.

    If you need to initialize a property (for example the `StartOn` or `EndOn` property) when XAF creates a new `CustomEvent`, override the `OnCreated` event.

7. If you want to add validation, see the [#region Validation](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs#L301) section in the source code.

    To use the [Validation Module](https://docs.devexpress.com/eXpressAppFramework/113684/validation-module), add validation rules to your object. For example, you can demand that the `StartOn` date always precedes the `EndOn` date.

## Implement the IResource Interface in the ApplicationUser Class

1. Implement the `IResource` interface in the [`ApplicationUser`](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/ApplicationUser.cs) class.

    > **Note**  
    > In this example, we implement `IResource.Id` explicitly because `ApplicationUser` already has an `ID` property. This implicit implementation causes an ambiguous exception in Windows Forms applications that use Entity Framework Core ORM. To resolve this issue, remap the Scheduler to the `ID` property directly instead of the `IResource.Id` property. For implementation details, refer to [CustomEventWithUserResourcesController.cs](./CS/EFCore/CustomEventsAndResources.Win/Controllers/CustomEventWithUserResourcesController.cs).

3. If you implement `IResource` in multiple classes, specify a Resource class for the corresponding Scheduler List View. For more information, refer to the following topic: [Use a Custom Resource Class](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule#use-a-custom-resource-class).

## Use Application Model to Customize Objects in Scheduler

1. Enable drag-and-drop in Scheduler.

    By default, Scheduler does not allow to edit objects. You need to set the [AllowEdit](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowEdit) property to `true` in the [Model.DesignedDiffs.xafml](./CS/EFCore/CustomEventsAndResources.Module/Model.DesignedDiffs.xafml) file.

2. Customize the `Start` and `End` properties to show time.

    By default, `DateTime` property editors only display date. Change `DisplayFormat` and `EditMask` to show and edit time. For details, see the [Model.DesignedDiffs.xafml](./CS/EFCore/CustomEventsAndResources.Module/Model.DesignedDiffs.xafml) file.

## Create an Event with a Single Resource

1. If you want to create a One-to-Many relationship between a resource and events, change `ResourceId` implementation. For details, see the following source file: [CustomEventWithCustomResource.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithCustomResource.cs).

2. Create a custom `Resource` class and implement the `IResource` interface. For implementation details, see the following source file: [CustomResource.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomResource.cs).

## Files to Review

- [CustomEventWithUserResources.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithUserResources.cs)
- [ApplicationUser.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/ApplicationUser.cs)
- [CustomEventWithUserResourcesController.cs](./CS/EFCore/CustomEventsAndResources.Win/Controllers/CustomEventWithUserResourcesController.cs)
- [CustomResource.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomResource.cs)
- [CustomEventWithCustomResource.cs](./CS/EFCore/CustomEventsAndResources.Module/BusinessObjects/CustomEventWithCustomResource.cs)

## Documentation

- [Scheduler Module Overview](https://docs.devexpress.com/eXpressAppFramework/112812/event-planning-and-notifications/scheduler/scheduler-module-overview)
- [Resources in a Schedule](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule)
- [Recurring Events](https://docs.devexpress.com/eXpressAppFramework/113128/event-planning-and-notifications/scheduler/recurring-events)
