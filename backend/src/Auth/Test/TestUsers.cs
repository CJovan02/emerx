namespace EMerx.Auth;

public static class TestUsers
{
    public static AuthUser Admin => new (
        "q0znPGETJqMApKd3TITjzeOfFWv2",
        "test-admin-user",
        "test-admin@test.com",
        Roles.Admin);

    public static AuthUser User => new (
        "user-valid-uid",
        "test-user",
        "test-user@test.com",
        Roles.User);
}