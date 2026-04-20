namespace Friday.Modules.Admin.Application.Configuration;

public sealed class PasswordFlowOptions
{
    public const string SectionName = "Authentication:PasswordFlow";

    public string AppBaseUrl { get; set; } = string.Empty;
    public string ResetPath { get; set; } = "/auth/reset-password";
    public int ForceChangeTokenMinutes { get; set; } = 30;
    public int ForgotPasswordTokenMinutes { get; set; } = 30;
}
