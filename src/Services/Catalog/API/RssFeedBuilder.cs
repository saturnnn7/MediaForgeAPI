using System.Globalization;
using System.Text;
using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.API;

internal static class RssFeedBuilder
{
    public static string Build(RssFeedDto feed, string channelUrl)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<rss version=\"2.0\" xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\">");
        sb.AppendLine("  <channel>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"    <title>{Escape(feed.ChannelTitle)}</title>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"    <link>{channelUrl}</link>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"    <description>{Escape(feed.ChannelDescription ?? feed.ChannelTitle)}</description>");
        sb.AppendLine(CultureInfo.InvariantCulture, $"    <lastBuildDate>{feed.LastBuildDate:R}</lastBuildDate>");

        if (feed.ChannelImageUrl != null)
        {
            sb.AppendLine("    <image>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <url>{feed.ChannelImageUrl}</url>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <title>{Escape(feed.ChannelTitle)}</title>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <link>{channelUrl}</link>");
            sb.AppendLine("    </image>");
        }

        foreach (var item in feed.Items.OrderBy(i => i.OrderMajor).ThenBy(i => i.OrderMinor))
        {
            sb.AppendLine("    <item>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <title>{Escape(item.Title)}</title>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <guid isPermaLink=\"false\">{item.ItemGuid}</guid>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"      <pubDate>{item.PublishedAt:R}</pubDate>");

            if (item.Description != null)
                sb.AppendLine(CultureInfo.InvariantCulture, $"      <description>{Escape(item.Description)}</description>");

            if (item.AudioUrl != null)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"      <enclosure url=\"{item.AudioUrl}\" type=\"audio/mpeg\" length=\"0\"/>");
                sb.AppendLine(CultureInfo.InvariantCulture, $"      <itunes:duration>{FormatDuration(item.DurationSeconds)}</itunes:duration>");
            }

            sb.AppendLine("    </item>");
        }

        sb.AppendLine("  </channel>");
        sb.AppendLine("</rss>");

        return sb.ToString();
    }

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private static string FormatDuration(double? seconds) =>
        seconds == null ? "0:00" : TimeSpan.FromSeconds(seconds.Value).ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture);
}
