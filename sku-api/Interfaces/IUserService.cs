public interface IUserService
{
    /// <summary>
    /// Authenticate the user login
    /// </summary>
    /// <param name="loginModel"></param>
    /// <returns>
    /// - A successful HTTP response (status code 200) containing the user's auth token
    /// - An Unauthorized HTTP response (status code 401) 
    /// - An Internal Server Error HTTP response (status code 500) if an unexpected error occurs.
    /// </returns>
    Task<IResult> AuthenticateUser(LoginModel loginModel);

    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="loginModel"></param>
    /// <returns>
    /// - A successful HTTP response (status code 201) containing the user's auth token
    /// - A Not Found HTTP response (status code 404)
    /// - An Internal Server Error HTTP response (status code 500) if an unexpected error occurs.
    /// </returns>
    Task<IResult> CreateUser(LoginModel loginModel);

    /// <summary>
    /// Get the user id from the http context
    /// </summary>
    /// <returns>The user id if user is authenticated, null otherwise</returns>
    int? GetUserId();
}