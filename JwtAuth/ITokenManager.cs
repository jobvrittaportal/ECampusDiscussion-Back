namespace ECampusDiscussion.JwtAuth
{
    public interface ITokenManager
    {
        string NewToken(string employeeID);
    }
}
