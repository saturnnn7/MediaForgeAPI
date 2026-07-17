namespace MediaForge.Media.Infrastructure.Models;

public sealed record SilenceSegment(double StartSeconds, double EndSeconds)
{
    public double MidpointSeconds => (StartSeconds + EndSeconds) / 2.0;
}
