$AccessKey = Read-Host "Enter your AWS Access Key ID"
$SecretKey = Read-Host "Enter your AWS Secret Access Key"
$Region = Read-Host "Enter your AWS Region (e.g. us-east-1)"

$awsDir = "$HOME\.aws"
if (!(Test-Path $awsDir)) {
    New-Item -ItemType Directory -Path $awsDir | Out-Null
}

$credentialsPath = "$awsDir\credentials"
$configPath = "$awsDir\config"

@"
[default]
aws_access_key_id = $AccessKey
aws_secret_access_key = $SecretKey
"@ | Out-File -Encoding ASCII -FilePath $credentialsPath

@"
[default]
region = $Region
"@ | Out-File -Encoding ASCII -FilePath $configPath

Write-Host "`n✅ AWS credentials saved successfully to $awsDir"
