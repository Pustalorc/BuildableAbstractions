namespace Pustalorc.Libraries.BuildableAbstractions.Commands;

internal static class CommandTranslationConstants
{
    public const string CommandExceptionValue =
        "An issue occurred during command execution of /{0} {1}. Error message: {2}. Stack trace: {3}";

    public const string NotAvailableKey = "not_available";
    public const string NotAvailableValue = "N/A";

    public const string CannotExecuteFromConsoleKey = "cannot_be_executed_from_console";

    public const string CannotExecuteFromConsoleValue =
        "That command cannot be executed from console with those arguments!";

    public const string BuildCountKey = "build_count";

    public const string BuildCountValue =
        "There are a total of {0} builds. Specific Item: {1}, Radius: {2}, Player: {3}, Planted Barricades Included: {4}, Filter by Barricades: {5}, Filter by Structures: {6}";

    public const string NotLookingBuildableKey = "not_looking_buildable";
    public const string NotLookingBuildableValue = "Cannot get any info! Are you looking at a structure/barricade?";

    public const string CannotTeleportNoBuildsKey = "cannot_teleport_no_builds";

    public const string CannotTeleportNoBuildsValue =
        "Cannot teleport anywhere, no buildables found with the following filters. Specific Item: {0}, Player: {1}, Planted Barricades Included: {2}, Filter by Barricades: {3}, Filter by Structures: {4}";

    public const string CannotTeleportBuildsTooCloseKey = "cannot_teleport_builds_too_close";

    public const string CannotTeleportBuildsTooCloseValue =
        "Cannot teleport anywhere, all buildables with the specified filters are too close. Specific Item: {0}, Player: {1}, Planted Barricades Included: {2}, Filter by Barricades: {3}, Filter by Structures: {4}";

    public const string TopBuilderFormatKey = "top_builder_format";
    public const string TopBuilderFormatValue = "At number {0}, {1} with {2} buildables!";
}