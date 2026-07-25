using MediatR;
using Shared.Infrastructure.Common;

namespace Calendar.Application.Features.Backgrounds.Commands.SubmitCalendarBackground;

public record SubmitCalendarBackgroundCommand(
    byte[] ImageData,
    string ContentType,
    string FileName) : IRequest<Result<Guid>>;
