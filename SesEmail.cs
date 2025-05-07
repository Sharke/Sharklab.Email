using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MimeKit;
using MimeKit.Utils;
using System.Text;
using System.Text.RegularExpressions;

public class SesEmail
{
    private readonly MimeMessage _message;
    private readonly BodyBuilder _builder;
    private readonly string _from;
    private readonly string _to;

    public SesEmail(string from, string to, string subjectTemplate, Dictionary<string, string>? tokens = null)
    {
        _from = from;
        _to = to;

        if (tokens != null)
        {
            foreach (var token in tokens)
            {
                subjectTemplate = subjectTemplate.Replace("{{" + token.Key + "}}", token.Value);
            }
        }

        _message = new MimeMessage();
        _message.From.Add(MailboxAddress.Parse(from));
        _message.To.Add(MailboxAddress.Parse(to));
        _message.Subject = subjectTemplate;

        _builder = new BodyBuilder();
    }

    public void SetBody(string content, bool isHtml = false)
    {
        if (isHtml)
            _builder.HtmlBody = content;
        else
            _builder.TextBody = content;
    }

    public void SetHtmlTemplate(string filePath, string? assetPath = null, Dictionary<string, string>? tokens = null)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("HTML template file not found", filePath);

        var html = File.ReadAllText(filePath);

        // Apply {{tokens}}
        if (tokens != null)
        {
            foreach (var token in tokens)
            {
                html = html.Replace("{{" + token.Key + "}}", token.Value);
            }
        }

        // Parse <img src="...">
        var imgTagRegex = new Regex("<img[^>]*src=[\"']([^\"']+)[\"'][^>]*>", RegexOptions.IgnoreCase);
        var matches = imgTagRegex.Matches(html);

        foreach (Match match in matches)
        {
            var originalTag = match.Value;
            var src = match.Groups[1].Value;

            // Resolve relative to base path if provided
            var imagePath = assetPath != null ? Path.Combine(assetPath, src) : src;
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Image not found: {imagePath}");
                continue;
            }

            var cid = MimeUtils.GenerateMessageId();
            _builder.LinkedResources.Add(imagePath).ContentId = cid;

            // Replace src with cid
            var updatedTag = originalTag.Replace(src, $"cid:{cid}");
            html = html.Replace(originalTag, updatedTag);
        }

        _builder.HtmlBody = html;
    }

    public void AddAttachment(string filePath)
    {
        if (File.Exists(filePath))
            _builder.Attachments.Add(filePath);
    }

    public async Task SendAsync(string? configurationSet = null, RegionEndpoint? region = null)
    {
        _message.Body = _builder.ToMessageBody();

        using var stream = new MemoryStream();
        _message.WriteTo(stream);
        stream.Position = 0;

        using var client = new AmazonSimpleEmailServiceClient(region ?? RegionEndpoint.USEast1);

        var request = new SendRawEmailRequest
        {
            RawMessage = new RawMessage(stream),
            Source = _from,
            Destinations = new List<string> { _to }
        };

        if (!string.IsNullOrWhiteSpace(configurationSet))
            request.ConfigurationSetName = configurationSet;

        var result = await client.SendRawEmailAsync(request);
        Console.WriteLine($"Email sent. Message ID: {result.MessageId}");
    }
}
