# XAF - Create Custom Event and Resource Classes for XAF Scheduler

In XAF Scheduler, you can view and edit Business Objects that implement the `IEvent` interface for [events](https://docs.devexpress.com/eXpressAppFramework/112812/event-planning-and-notifications/scheduler/scheduler-module-overview#events) and the `IResource` interface for [resources](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule). XAF's [Business Class Library(https://docs.devexpress.com/eXpressAppFramework/112571/business-model-design-orm/built-in-business-classes-and-interfaces)] offers the `Event` and `Resource` classes implementing these interfaces that you can use out-of-the-box.

However, you may need to implement custom `Event` and `Resource` classes or extend your existing classes. The following example covers the following popular scenarios:

1. Create custom Event and Resource classes

2. Use ApplicationUser as a resource in Scheduler

3. Create an event with a single resource

> [!NOTE]
> These instructions describe a specific to Entity Framework Core ORM.

NOTE: This example shows EF Core classes. For XPO refer to source codes C:\Work\2023.2\XAF\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Event.cs C:\Work\2023.2\XAF\DevExpress.Persistent\DevExpress.Persistent.BaseImpl.Xpo\Resource.cs

## Create a Custom Event Class and Implement the IEvent, IRecurrentEvent, IReminderEvent, and IXafEntityObject interfaces

Source code C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomEvent.cs

The source code contains `#region` sections with logically grouped properties and methods.

### Step-by-Step Implementation

1. Add simple properties. See #region `Base Properties` section in the source code. It contains all `IEvent` properties except `ResourceId`.

2. Add resources support. See `#region Resources` section in the source code.
    
    The `IEvent` interface requires a string `ResourceId` property that stores a collection of resources in an XML string. XML string is suitable to manipulate in an application UI. In XAF, we recommend to have an associated collection of objects. Add the `ResourceId` string property and the `Resources` collection property and set up a synchronization between them.

3. Implement `IRecurrentEvent`. See `#region IRecurrentEvent` section in the source code.

4. Implement `IReminderEvent`. See `#region IReminderEvent` section in the source code.

    > [!NOTE]
    > Currently XAF Scheduler Blazor Module does not support reminders. Skip this section for ASP.NET Core Blazor applications.

5. Add Blazor-compatibility options. See `#region Blazor compatibility` section in the source code.

    Skip this step if you do not plan to implement the `Event` class in an XAF ASP.NET Core Blazor application.

    > [!NOTE]
    > Currently, XAF Blazor Scheduler Module supports only one resource. You need the `ResourceIdBlazor` property to select a resource from the `Resources` collection. You can implement you own resource selection logic.

6. Add the construction section. See `#region Construction` section in the source code.

    If you need to initialize a property (for example the `StartOn` and `EndOn` properties) when XAF creates a new `CustomEvent`, override the `OnCreated` event.

7. Add validation. See `#region Validation` section in the source code.

    If you want to use the [Validation Module](https://docs.devexpress.com/eXpressAppFramework/113684/validation-module), add validation rules to your object. For example, you can demand that a `StartOn` date always precedes an `EndOn` date.

## Implement the IResource Interface in the ApplicationUser Class

1. Implement the `IResource` interface in the `ApplicationUser` class.

    C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\ApplicationUser.cs

    > [!NOTE]
    > In this example, we implement `IResource.Id` explicitly because `ApplicationUser` already has the `ID` property. This implicit implementation causes an ambiguous exception in Windows Forms applications that use Entity Framework Core ORM. To resolve this issue, remap the Scheduler to the `ID` property directly instead of the `IResource.Id` property. For an example, refer to the following Controller implementation: C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Win\Controllers\CustomEventWithUserResourcesController.cs

3. If you have more than one `IResource` implementation in your application, specify a Resource class for the corresponding Scheduler List View. For more information, refer to the following topic: [Use a Custom Resource Class](https://docs.devexpress.com/eXpressAppFramework/112813/event-planning-and-notifications/scheduler/resources-in-a-schedule#use-a-custom-resource-class).

## Use Application Model to Customize Objects in Scheduler

Sources C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\Model.DesignedDiffs.xafml

1. Enable drag-and-drop in Scheduler.

    By default, Scheduler does not allow to edit objects. You need to set the [AllowEdit](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Model.IModelView.AllowEdit) property to `true` in the _Model.DesignedDiffs.xafml_ file.

2. Customize the `Start` and `End` properties to show time.

    By default, `DateTime` property editors only display date. Change `DisplayFormat` and `EditMask` to show and edit time.

## Create an Event with a Single Resource

1. If you want to create One-to-Many relationship between a resource and events, change the `ResourceId` implementation. For an example, see the `#region Resources` and `#region Blazor compatibility` sections of the following source file: C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomEventWithCustomResource.cs

2. Create a custom `Resource` class and implement the `IResource` interface.

    See the following source file: C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomResource.cs

## Files to Review

- C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomEvent.cs
- C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\ApplicationUser.cs
- C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Win\Controllers\CustomEventWithUserResourcesController.cs
- C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomResource.cs
- C:\Work\Samples\CustomEventsAndResources\CustomEventsAndResources\CustomEventsAndResources.Module\BusinessObjects\CustomEventWithCustomResource.cs

## Documentation

## More Examples
