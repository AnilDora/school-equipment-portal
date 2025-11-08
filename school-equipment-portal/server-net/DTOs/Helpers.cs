namespace SchoolEquipmentApi.DTOs
{
    public static class Helpers
    {
        public static string? GetBearer(Microsoft.AspNetCore.Http.HttpRequest req)
        {
            if (!req.Headers.TryGetValue("Authorization", out var v)) return null;
            var s = v.ToString();
            if (s.StartsWith("Bearer ")) return s.Substring(7);
            return s;
        }

        public static bool Overlaps(string s1, string e1, string s2, string e2)
        {
            if (!DateTimeOffset.TryParse(s1, out var a)) return false;
            if (!DateTimeOffset.TryParse(e1, out var b)) return false;
            if (!DateTimeOffset.TryParse(s2, out var c)) return false;
            if (!DateTimeOffset.TryParse(e2, out var d)) return false;
            return a <= d && c <= b;
        }
    }
}