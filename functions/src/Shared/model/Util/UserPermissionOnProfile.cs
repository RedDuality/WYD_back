namespace Model;
public enum UserPermissionOnProfile
{
    None = 0,
    CREATE_COMMUNITY = 1 << 0,    // 1
    ADD_PHOTO = 1 << 1,   // 2
    READ_COMMUNITY = 1 << 2, // 4
    READ_EVENTS = 1 << 3,   // 8
    CREATE_EVENT = 1 << 4,  // 16
    CONFIRM_EVENT = 1 << 5, // 32
    UPDATE_EVENT = 1 << 6, // 64
    DELETE_EVENT = 1 << 7, //128
}