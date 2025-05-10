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
using static System.Collections.Specialized.BitVector32;

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
        //TempData["LoginError"] = null;
		if (user == null)
        {
			TempData["LoginError"] = "The email address you entered is not registered.";
			return RedirectToAction("Login");

		}

		var hashedPassword = HashPassword(password, user.Salt);

        if (user.Password != hashedPassword)
        {
			TempData["LoginError"] = "Incorrect email or password. Please try again.";
			return RedirectToAction("Login");
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

    // ******************************************************** profile ******
    public async Task<IActionResult> Profile(string section = "artworks")
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Login");

        var artworks = await _context.Artworks
            .Where(a => a.ArtistId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        foreach (var a in artworks)
        {
            if (a.Title.Length > 100)
                a.Title = a.Title[..100] + "...";
            if (a.Description.Length > 500)
                a.Description = a.Description[..500] + "...";
        }

        var purchases = await _context.Transactions
            .Include(t => t.Artwork)
            .Where(t => t.BuyerId == userId)
            //(t.Status == TransactionStatus.Completed || t.Status == TransactionStatus.Pending))
            .OrderByDescending(t => t.PurchasedAt)
            .ToListAsync();


        var sales = await _context.Transactions
            .Include(t => t.Artwork)
            .Where(t => t.Artwork.ArtistId == userId && t.Status == TransactionStatus.Completed)
            .OrderByDescending(t => t.PurchasedAt)
            .ToListAsync();

        var model = new ProfileModel
        {
            User = user,
            Artworks = artworks,
            Purchases = purchases,
            Sales = sales
        };

        var redirectToPurchases = HttpContext.Session.GetBool("RedirectToPurchases") ?? false;
        if (redirectToPurchases)
        {
            ViewData["ActiveSection"] = "purchases";
            HttpContext.Session.SetBool("RedirectToPurchases", false);
        }
        else
        {
            ViewData["ActiveSection"] = section;
        }

        return View(model);
    }

    // ************************************************ change password ******

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        var currentHashed = HashPassword(currentPassword, user.Salt);
        if (user.Password != currentHashed)
        {
            TempData["PassChangeError"] = "Invalid current password.";
            return RedirectToAction("Profile", new { section = "account" });
        }

        string newSalt = GenerateSalt();
        user.Salt = newSalt;
        user.Password = HashPassword(newPassword, newSalt);

        await _context.SaveChangesAsync();

        TempData["InfoMsg"] = "Password changed successfully!";
        return RedirectToAction("Profile", new { section = "account" });
    }


    // ************************************************* delete account ******

    [HttpPost]
    public async Task<IActionResult> DeleteAccount(string password)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsDeleted) return NotFound();

        var hashed = HashPassword(password, user.Salt);
        if (user.Password != hashed)
        {
            TempData["DeleteError"] = "Invalid password.";
            return RedirectToAction("Profile", new { section = "account" });
        }

        TempData["ConfirmDelete"] = true;
        TempData["InfoMsg"] = "Account deletion will log you out and return you to the homepage.";
        return RedirectToAction("Profile", new { section = "account" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccountConfirmed()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        //_context.Users.Remove(user);
        user.IsDeleted = true;
        await _context.SaveChangesAsync();
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    // ************************************************* clear messages ******

    public IActionResult ClearMessages()
    {
        TempData.Remove("PassChangeError");
        TempData.Remove("DeleteError");
        TempData.Remove("ConfirmDelete");
        return RedirectToAction("Profile", new { section = "account" });
    }

    // ************************************************** edit fullname ******

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFullName(string fullName)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.FullName = fullName;

        await _context.SaveChangesAsync();

        TempData["InfoMsg"] = "Full Name changed successfully!";
        return RedirectToAction("Profile", new { section = "account" });
    }
}