namespace Model;

[Flags]
public enum ProfilePermissionOnEvent
{
    None = 0,
    Read = 1 << 0,    // 1
    Write = 1 << 1,   // 2
    Execute = 1 << 2, // 4
    Delete = 1 << 3   // 8
}