using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UserDbContext
{
    public string UserID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}