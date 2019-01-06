#region

#endregion

namespace LoESoft.GameServer.networking
{
    public enum ErrorIDs : int
    {
        OUTDATED_CLIENT = 0,
        DISABLE_GUEST_ACCOUNT = 1,
        SERVER_FULL = 2,
        ACCOUNT_BANNED = 3,
        INVALID_DISCONNECT_KEY = 4,
        LOST_CONNECTION = 5,
        NORMAL_CONNECTION = 6,
        OUTDATED_INTERNAL_CLIENT = 7,
        VIP_ACCOUNT_OVER = 8,
        ACCOUNT_IN_USE = 9,
        ACCESS_DENIED_DUE_RESTART = 10
    }

    public enum FailureIDs : int
    {
        DEFAULT = 0,
        BAD_KEY = 5,
        INVALID_TELEPORT_TARGET = 6,
        EMAIL_VERIFICATION_NEEDED = 7,
        JSON_DIALOG = 8
    }
}