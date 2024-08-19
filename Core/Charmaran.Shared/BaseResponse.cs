using System.Collections.Generic;

namespace Charmaran.Shared
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}