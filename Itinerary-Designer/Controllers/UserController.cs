using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Trips.ViewModels;

public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.Select(u => new UserViewModel
        {
            Id = u.Id,
            Username = u.UserName,
            Email = u.Email
           
        }).ToList();

        return View(users); 
    }
}
