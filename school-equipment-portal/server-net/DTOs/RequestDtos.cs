namespace SchoolEquipmentApi.DTOs
{
    public record SignupDto(string Username, string Password, string? Role);
    public record LoginDto(string Username, string Password);
    public record EquipmentInput(string Name, string? Category, string? Condition, int Quantity, bool Available);
    public record RequestInput(string EquipmentId, string StartDate, string EndDate);
}