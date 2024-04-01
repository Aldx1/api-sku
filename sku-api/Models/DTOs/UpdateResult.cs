/// <summary>
/// A model object to represent a successful update
/// Contains the overall update success for a request - 
///     true if fully or partly successful (Some or all update performed)
///     false if completely unsuccessful (No updates performed)
/// Contains an object (cart/list<order> etc) if successful
/// Contains any status messages if fully or partly unsuccessful
/// </summary>

public class UpdateResult
{
    public bool Success { get; set; }
    public object? UpdateResultObject { get; set; }
    public string? Message { get; set; }
}