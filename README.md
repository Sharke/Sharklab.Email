# 🦈 SharkLab.Email

**SharkLab.Email** is a simple wrapper for sending beautiful, tokenized HTML emails via Amazon SES

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
    { "Name", "John Doe" },
    { "Year", DateTime.Now.Year.ToString() },
    { "Date", DateTime.UtcNow.ToString("MMMM dd, yyyy") }
};

var email = new SesEmail(
    from: "hello@sharklab.io",
    to: "example@email.org",
    subjectTemplate: "SharkLab.Mail Example - {{Date}}",
    tokens: tokens
);

email.SetHtmlTemplate(
    filePath: "templates/example.html",
    assetPath: "assets",
    tokens: tokens
);

email.AddAttachment(@"dummy.pdf");
await email.SendAsync();
```

```html
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8" />
  <title>Welcome to SharkLab</title>
  <style>
    body {
      margin: 0;
      padding: 0;
      background-color: #f4f4f4;
      font-family: Arial, sans-serif;
    }

    .container {
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border-radius: 8px;
      overflow: hidden;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
    }

    .header {
      background-color: #0f4c81;
      padding: 30px;
      text-align: center;
      color: white;
    }

    .header img {
      max-width: 120px;
      margin-bottom: 10px;
    }

    .content {
      padding: 30px;
      color: #333;
    }

    .content h1 {
      color: #0f4c81;
      margin-top: 0;
    }

    .button {
      display: inline-block;
      padding: 12px 24px;
      background-color: #0f4c81;
      color: #ffffff;
      text-decoration: none;
      border-radius: 6px;
      margin-top: 20px;
    }

    .footer {
      font-size: 12px;
      color: #888;
      text-align: center;
      padding: 15px;
      background-color: #fafafa;
    }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <img src="logo.png" alt="SharkLab Logo" />
      <h2>Welcome to SharkLab</h2>
    </div>
    <div class="content">
      <h1>Hello {{Name}},</h1>
      <p>We’re thrilled to have you join us.</p>
      <p>This is a fully tokenized HTML email with an embedded logo, customizable content, and an optional attachment — built with 💙 by SharkLab.Mail.</p>

      <a href="https://sharklab.io" class="button">Explore SharkLab</a>

      <p style="margin-top: 30px;">Sent on {{Date}}</p>
    </div>
    <div class="footer">
      &copy; {{Year}} SharkLab. All rights reserved.
    </div>
  </div>
</body>
</html>
```

![Example](https://i.imgur.com/kXrWOKn.png)

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
