public class RequestUpdateUser
{
    public int UserId { get; set; }
    public string? Name { get; set; }      // allow null if not changing
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public int? GenderId { get; set; }
    public int? RoleId { get; set; }
}
