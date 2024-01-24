namespace RegfreeNetComServer.Com
{
    public enum RPC_C_AUTHN_LEVEL
    {
        RPC_C_AUTHN_LEVEL_DEFAULT = 0,
        RPC_C_AUTHN_LEVEL_NONE = 1,
        RPC_C_AUTHN_LEVEL_CONNECT = 2,
        RPC_C_AUTHN_LEVEL_CALL = 3,
        RPC_C_AUTHN_LEVEL_PKT = 4,
        RPC_C_AUTHN_LEVEL_PKT_INTEGRITY = 5,
        RPC_C_AUTHN_LEVEL_PKT_PRIVACY = 6
    }
}
