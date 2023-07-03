namespace Pustalorc.Libraries.BuildableAbstractions.Commands;

internal static class CommandTranslationConstants
{
    public const string CommandExceptionValue =
        "An issue occurred during command execution of /{0} {1}. Error message: {2}. Stack trace: {3}";

    public const string NotAvailableKey = "not_available";
    public const string NotAvailableValue = "N/A";

    public const string CannotExecuteFromConsoleKey = "cannot_be_executed_from_console";
    public const string CannotExecuteFromConsoleValue = "This command cannot be executed from console!";

    public const string BuildCountKey = "build_count";
    public const string BuildCountValue = "";

    public const string NotLookingBuildableKey = "not_looking_buildable";
    public const string NotLookingBuildableValue = "";

    public const string CannotTeleportNoBuildsKey = "cannot_teleport_no_builds";
    public const string CannotTeleportNoBuildsValue = "";

    public const string CannotTeleportBuildsTooCloseKey = "cannot_teleport_builds_too_close";
    public const string CannotTeleportBuildsTooCloseValue = "";

    public const string TopBuilderFormatKey = "";
    public const string TopBuilderFormatValue = "";
}