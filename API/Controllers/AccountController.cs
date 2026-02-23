using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
    {
        if ( await EmailExists(registerDto.Email)) return BadRequest("email already exixt");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            Displayname = registerDto.DisplayName,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
           PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login([FromBody]LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x=>x.Email == loginDto.Email);

        if(user == null)return Unauthorized("invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (var i = 0; i < ComputeHash.Length; i++)
        {
            if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("password is invalid");

        }
        return user;
      
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x=>x.Email.ToLower() == email.ToLower());
    }

}
