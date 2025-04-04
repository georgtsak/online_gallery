using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using OnlineGallery.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using OnlineGallery.Helper;

public class UsersController : Controller
{
	private readonly AppDbContext _context;

	public UsersController(AppDbContext context)
	{
		_context = context;
	}

	// **************************************************** return View ******
	// GET: Users/Create
	public IActionResult Create()
	{
		return View();
	}

	// GET: Users/Register
	public IActionResult Register()
	{
		return View();
	}
	public IActionResult Login()
	{
		//Debug.WriteLine("login page opened");
		return View();
	}

	// ********************************************************* create ******
	// POST: Users/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("FullName,Email,Password")] User user)
	{
		if (ModelState.IsValid)
		{
			var salt = GenerateSalt();
			var hashedPassword = HashPassword(user.Password, salt);

			user.Password = hashedPassword;  // save hashed password
			user.Salt = salt;  // save salt
			user.Role = (Role)2; // default role --> Visitor 

			_context.Add(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(user);
	}

	// ******************************************************* register ******
	// POST: Users/Register
	[HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        var salt = GenerateSalt();
        user.Salt = salt;
        user.Password = HashPassword(user.Password, salt);
        user.Role = Role.Visitor;
        user.CreatedAt = TimeAthens.GetAthensTime();

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine($"ModelState error: {error.ErrorMessage}");
            }
            return View(user);
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Το email αυτό χρησιμοποιείται ήδη.");
            return View(user);
        }

        try
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            Debug.WriteLine($"SAVED USER TO DB!!!!!");

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserFullName", user.FullName);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            ModelState.AddModelError("", "Ο χρήστης δεν καταχωρήθηκε.");
        }

        return View(user);
    }

	// ********************************************************** login ******
	// POST: Users/Login
	[HttpPost]
	public async Task<IActionResult> Login(string email, string password)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

		if (user == null)
		{
			ModelState.AddModelError("Email", "Το email δεν βρέθηκε.");
			return View();
		}

		var hashedPassword = HashPassword(password, user.Salt);

		if (user.Password != hashedPassword)
		{
			ModelState.AddModelError("Password", "Λάθος κωδικός.");
			return View();
		}

        // session
		HttpContext.Session.SetInt32("UserId", user.Id);
		HttpContext.Session.SetString("UserRole", user.Role.ToString());
		HttpContext.Session.SetString("UserFullName", user.FullName);

		Debug.WriteLine($"User {user.Email} logged in!");

		return RedirectToAction("Index", "Home");
	}

	// ******************************************************** hashing ******
	private string GenerateSalt()
	{
		var saltBytes = new byte[16];
		using (var rng = new RNGCryptoServiceProvider())
		{
			rng.GetBytes(saltBytes);
		}
		return Convert.ToBase64String(saltBytes);
	}

	private string HashPassword(string password, string salt)
	{
		using (var sha256 = SHA256.Create())
		{
			var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
			var hashedBytes = sha256.ComputeHash(combinedBytes);
			return Convert.ToBase64String(hashedBytes);
		}
	}

    // ********************************************************* logout ******
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // delete data
        return RedirectToAction("Index", "Home");
    }
}