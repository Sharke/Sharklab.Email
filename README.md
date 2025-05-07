# 🦈 SharkLab.Mail

**SharkLab.Mail** is a simple wrapper for sending beautiful, tokenized HTML emails via Amazon SES

## ✨ Features

| Feature             | Description                                   |
|---------------------|-----------------------------------------------|
| ✅ Tokenized Templates | Replace `{{Token}}` values in HTML/subject    |
| ✅ Inline Images      | Use `<img src="logo.png">`                   |
| ✅ Attachments        | `.AddAttachment("path.pdf")`                 |
| ✅ AWS SES            | Sends raw email via AWS SDK                  |

---

### 🧪 Sample Code

```csharp
var tokens = new Dictionary<string, string>
{
    { "Name", "John" },
    { "Buisness", "Acme Inc" },
    { "Date", DateTime.UtcNow.ToString("MMMM dd, yyyy") }
};

var email = new SesEmail(
    from: "hello@sharklab.io",
    to: "client@example.com",
    subjectTemplate: "📊 Daily Report for {{Name}} - {{Date}}",
    tokens: tokens
);

email.SetHtmlTemplate(
    filePath: "Templates/daily-report.html",
    assetPath: "Templates/Assets",
    tokens: tokens
);

email.AddAttachment("Reports/DailySalesReport.pdf");
await email.SendAsync();
```

```html
<!DOCTYPE html>
<html>
<head>
  <style>
    body { font-family: sans-serif; }
  </style>
</head>
<body>
  <h1>Hello {{Name}},</h1>
  <p>Your report for {{Buisness}} is attached.</p>
  <img src="logo.png" alt="Logo" />
</body>
</html>
```

### 🔐 Credential Setup

Sharklab.Email uses the standard **AWS SDK credential chain**. The SDK will automatically use credentials from the first source it finds, in this order:

1. **Environment Variables**

   ```bash
   AWS_ACCESS_KEY_ID=your_key
   AWS_SECRET_ACCESS_KEY=your_secret
   AWS_REGION=us-east-1
   ```

2. **Shared Credentials File**

   - Location:
     - Windows: `%USERPROFILE%\.aws\credentials`
     - macOS/Linux: `~/.aws/credentials`

   - Example:
     ```ini
     [default]
     aws_access_key_id = your_key
     aws_secret_access_key = your_secret
     ```

   - Optionally set region in `~/.aws/config`:
     ```ini
     [default]
     region = us-east-1
     ```

3. **Named AWS Profile**

   - Configure with:
     ```bash
     aws configure --profile myprofile
     ```

   - Use in code:
     ```csharp
     var creds = new StoredProfileAWSCredentials("myprofile");
     var client = new AmazonSimpleEmailServiceClient(creds, RegionEndpoint.USEast1);
     ```
