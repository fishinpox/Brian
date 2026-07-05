namespace Calendar.Application.Features.Calendar.Queries.GetCalendarView;

public record CalendarViewDto(
    List<PersonalEventDto> PersonalEvents,
    List<StreamEventDto> StreamEvents);
