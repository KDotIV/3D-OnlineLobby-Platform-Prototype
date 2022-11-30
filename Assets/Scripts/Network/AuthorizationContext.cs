using System;
using System.Collections.Generic;

[Serializable]
public class AuthorizationContext
{
    public UserDbContext User { get; set; }
    public string Token { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; }
}
