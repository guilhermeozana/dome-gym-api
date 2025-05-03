using ErrorOr;

namespace UserManagement.Domain.Common.EventualConsistency;

public static class EventualConsistencyError
{
    public const int EventualConsistencyType = 100;

    public static Error From(string code, string description)
    {
        return Error.Custom(EventualConsistencyType, code, description);
    }
}