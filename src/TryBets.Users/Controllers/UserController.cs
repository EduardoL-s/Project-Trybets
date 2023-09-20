using System;
using Microsoft.AspNetCore.Mvc;
using TryBets.Users.Repository;
using TryBets.Users.Services;
using TryBets.Users.Models;
using TryBets.Users.DTO;

namespace TryBets.Users.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _repository;
    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("signup")]
    public IActionResult Post([FromBody] User user)
    {
       var newUser = _repository.Post(user);

       if (newUser == null) {
        return BadRequest(new { message = "E-mail already used" });
       }

       string tokenGenereted = new TokenManager().Generate(newUser);
       AuthDTOResponse token = new AuthDTOResponse { Token = tokenGenereted };

       return Created("", token);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthDTORequest login)
    {
        var userLogin = _repository.Login(login);

        if (userLogin == null) {
            return BadRequest(new { message = "uthentication failed" });
        }

        string tokenGenereted = new TokenManager().Generate(userLogin);
        AuthDTOResponse token = new AuthDTOResponse { Token = tokenGenereted };

        return Ok(token);
    }
}