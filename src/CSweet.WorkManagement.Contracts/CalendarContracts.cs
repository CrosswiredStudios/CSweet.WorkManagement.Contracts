namespace CSweet.WorkManagement.Contracts;

public static class CalendarCapabilities
{
    public const string Read = "work.calendar.read.v1";
    public const string Create = "work.calendar.create.v1";
    public const string Update = "work.calendar.update.v1";
    public const string Cancel = "work.calendar.cancel.v1";
    public const string Schedule = "work.calendar.schedule.v1";
    public static IReadOnlyList<string> All { get; } = [Read, Create, Update, Cancel, Schedule];
}

public static class CalendarEvents
{
    public const string ReminderDue = "com.csweet.calendar.reminder-due.v1";
}

public sealed record CalendarRecurrence(string Frequency, int Interval = 1, int? Count = null, DateOnly? Until = null);
public sealed record CalendarWork(string Kind, Guid TargetOrganizationUserId, string? Instructions = null, Guid? ItemId = null);
public sealed record CalendarEventInput(
    string Title, DateTime StartLocal, DateTime EndLocal, string TimeZoneId = "UTC",
    bool AllDay = false, string? Description = null, string? Location = null,
    Guid? OwnerOrganizationUserId = null, IReadOnlyList<Guid>? AttendeeIds = null,
    IReadOnlyList<int>? ReminderMinutes = null, CalendarRecurrence? Recurrence = null, CalendarWork? Work = null);
public sealed record CalendarQuery(DateTimeOffset From, DateTimeOffset To);
public sealed record CreateCalendarEventRequest(CalendarEventInput Event, string IdempotencyKey);
public sealed record UpdateCalendarEventRequest(Guid EventId, long ExpectedRevision, CalendarEventInput Event,
    DateTime? OccurrenceLocal = null);
public sealed record CancelCalendarEventRequest(Guid EventId, long ExpectedRevision, DateTime? OccurrenceLocal = null);
public sealed record CalendarEventView(Guid Id, long Revision, Guid OwnerOrganizationUserId,
    CalendarEventInput Event, bool Cancelled, bool CanEdit);
public sealed record CalendarOccurrence(Guid EventId, DateTime OccurrenceLocal, DateTimeOffset Start,
    DateTimeOffset End, CalendarEventView Series, CalendarEventInput Event,
    string? WorkStatus = null, string? WorkError = null, Guid? WorkItemId = null);
public sealed record CalendarMember(Guid Id, string Name);
public sealed record BusinessCalendarView(string TimeZoneId, long Revision, Guid ActorId, bool CanCreate,
    bool CanConfigure, IReadOnlyList<CalendarMember> Members, IReadOnlyList<CalendarOccurrence> Occurrences);
public sealed record UpdateCalendarSettingsRequest(string TimeZoneId, long ExpectedRevision);
public sealed record CalendarReminder(Guid Id, Guid EventId, DateTimeOffset Start, string Title,
    DateTimeOffset CreatedAt, bool Read);
public sealed record CalendarReminderDueEvent(Guid ReminderId, Guid EventId, DateTimeOffset Start,
    string Title, Guid RecipientOrganizationUserId);
