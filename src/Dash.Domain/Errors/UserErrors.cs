using Dash.Domain.Common;

namespace Dash.Domain.Errors;

public static class UserErrors
{
    public static readonly Error EmailAlreadyInUse = new(
        "User.EmailInUse",
        "The provided email is already registered.");


    public static readonly Error UsernameAlreadyInUse = new(
        "User.UsernameInUse",
        "The provided username is already taken.");


    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The email/username or password provided is incorrect.");

    public static readonly Error AccountIsLocked = new(
        "User.AccountIsLocked",
        "Too many login attempts. Please try again later.");
}
