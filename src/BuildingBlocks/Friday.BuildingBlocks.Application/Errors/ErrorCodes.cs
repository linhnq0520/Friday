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

        public const string RoleNotFound = "ADMIN_ROLE_NOT_FOUND";
        public const string RoleCodeExists = "ADMIN_ROLE_CODE_EXISTS";

        public const string RightNotFound = "ADMIN_RIGHT_NOT_FOUND";
        public const string RightCodeExists = "ADMIN_RIGHT_CODE_EXISTS";
    }
}
