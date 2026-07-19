namespace MediaForge.Library.Application.Commands.ResolveReport;

public sealed record ResolveReportCommand(Guid ReportId) : IRequest<Result>;
