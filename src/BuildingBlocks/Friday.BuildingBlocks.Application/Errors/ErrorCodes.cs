namespace Friday.BuildingBlocks.Application.Errors;

public static class ErrorCodes
{
    public static class Common
    {
        public const string Success = "SUCCESS";
        public const string BadRequest = "BAD_REQUEST";
        public const string NotFound = "NOT_FOUND";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    }

    public static class Admin
    {
        public const string UserNotFound = "ADMIN_USER_NOT_FOUND";
        public const string UserUsernameExists = "ADMIN_USER_USERNAME_EXISTS";
        public const string UserEmailExists = "ADMIN_USER_EMAIL_EXISTS";
        public const string UserRoleNotFound = "ADMIN_USER_ROLE_NOT_FOUND";
        public const string UserCodeExists = "ADMIN_USER_CODE_EXISTS";
        public const string InvalidCredentials = "ADMIN_INVALID_CREDENTIALS";
        public const string UserInactive = "ADMIN_USER_INACTIVE";
        public const string UserLockedAuth = "ADMIN_USER_LOCKED";
        public const string PasswordRequired = "ADMIN_PASSWORD_REQUIRED";
        public const string InvalidRefreshToken = "ADMIN_INVALID_REFRESH_TOKEN";
        public const string SessionInvalid = "ADMIN_SESSION_INVALID";
        public const string RegistrationDisabled = "ADMIN_REGISTRATION_DISABLED";
        public const string OAuth2ProviderInvalid = "ADMIN_OAUTH2_PROVIDER_INVALID";
        public const string OAuth2ProviderDisabled = "ADMIN_OAUTH2_PROVIDER_DISABLED";
        public const string OAuth2AuthorizationCodeInvalid = "ADMIN_OAUTH2_AUTHORIZATION_CODE_INVALID";
        public const string OAuth2TokenExchangeFailed = "ADMIN_OAUTH2_TOKEN_EXCHANGE_FAILED";
        public const string OAuth2UserInfoFailed = "ADMIN_OAUTH2_USERINFO_FAILED";
        public const string OAuth2EmailMissing = "ADMIN_OAUTH2_EMAIL_MISSING";
        public const string OAuth2DefaultRoleInvalid = "ADMIN_OAUTH2_DEFAULT_ROLE_INVALID";

        public const string RoleNotFound = "ADMIN_ROLE_NOT_FOUND";
        public const string RoleCodeExists = "ADMIN_ROLE_CODE_EXISTS";

        public const string RightNotFound = "ADMIN_RIGHT_NOT_FOUND";
    }
}
