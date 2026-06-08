namespace EMerx.Auth;

public static class TestUsers
{
    public static AuthUser Admin => new (
        "q0znPGETJqMApKd3TITjzeOfFWv2",
        "test-admin-user",
        "test-admin@test.com",
        Roles.Admin);

    public static AuthUser User => new (
        "deec9588-4258-4830-9e92-d091e2a691e3",
        "test-user",
        "test-user@test.com",
        Roles.User);
}