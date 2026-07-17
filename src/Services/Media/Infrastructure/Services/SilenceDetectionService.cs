using System.Diagnostics;
using System.Globalization;
using MediaForge.Media.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Infrastructure.Services;

public sealed class SilenceDetectionService(ILogger<SilenceDetectionService> logger) : ISilenceDetectionService
{
    public async Task<IReadOnlyList<SilenceSegment>> DetectSilenceAsync(string filePath, CancellationToken ct)
    {
        var args = $"-i \"{filePath}\" -af silencedetect=noise=-30dB:d=0.5 -f null -";

        var psi = new ProcessStartInfo("ffmpeg", args)
        {
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;
        var stderr = await process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);

        var starts = new List<double>();
        var ends = new List<double>();

        foreach (var line in stderr.Split('\n'))
        {
            if (line.Contains("silence_start:", StringComparison.Ordinal))
            {
                var val = line.Split("silence_start:")[1].Trim().Split(' ')[0];
                if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    starts.Add(d);
            }
            else if (line.Contains("silence_end:", StringComparison.Ordinal))
            {
                var val = line.Split("silence_end:")[1].Trim().Split('|')[0].Trim();
                if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    ends.Add(d);
            }
        }

        logger.LogInformation("Detected {Count} silence segments in {FilePath}", Math.Min(starts.Count, ends.Count), filePath);

        var segments = new List<SilenceSegment>();
        for (var i = 0; i < Math.Min(starts.Count, ends.Count); i++)
            segments.Add(new SilenceSegment(starts[i], ends[i]));

        return segments;
    }
}
