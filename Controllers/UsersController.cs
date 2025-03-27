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

public class UsersController : Controller
{
	private readonly AppDbContext _context;

	public UsersController(AppDbContext context)
	{
		_context = context;
	}

	// GET: Users/Create
	public IActionResult Create()
	{
		return View();
	}

	// POST: Users/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("FullName,Email,Password")] User user)
	{
		if (ModelState.IsValid)
		{
			// Δημιουργία Salt για το Password
			var salt = GenerateSalt();
			var hashedPassword = HashPassword(user.Password, salt);

			user.Password = hashedPassword;  // Αποθήκευση του hashed password
			user.Salt = salt;  // Αποθήκευση του Salt
			user.Role = (Role)2; // Default Role = Visitor (Role 2)

			_context.Add(user);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(user);
	}

	// GET: Users/Register
	public IActionResult Register()
	{
		return View();
	}

    // POST: Users/Register
    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        // Δημιουργία salt και κρυπτογράφηση κωδικού πριν την επικύρωση του μοντέλου
        var salt = GenerateSalt();
        user.Salt = salt; // Ορίζουμε το salt εδώ
        user.Password = HashPassword(user.Password, salt);
        user.Role = Role.Visitor;
        user.CreatedAt = DateTime.UtcNow;

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
            ModelState.AddModelError("Email", "!");
            return View(user);
        }

        try
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            Debug.WriteLine($"SAVED USER TO DB!!!!!");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            ModelState.AddModelError("", "Ο χρήστης δεν καταχωρήθηκε λόγω σφάλματος.");
        }

        return View(user);
    }




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

	// GET: Users/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var user = await _context.Users
			.FirstOrDefaultAsync(m => m.Id == id);
		if (user == null)
		{
			return NotFound();
		}

		return View(user);
	}

	// GET: Users/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var user = await _context.Users.FindAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		return View(user);
	}

	// POST: Users/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Password,Role")] User user)
	{
		if (id != user.Id)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			try
			{
				// Αν θέλουμε να ενημερώσουμε το password, πρέπει να γίνει hashing
				if (!string.IsNullOrEmpty(user.Password))
				{
					var salt = GenerateSalt();
					user.Password = HashPassword(user.Password, salt);
					user.Salt = salt;
				}

				_context.Update(user);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(user.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		return View(user);
	}

	// GET: Users/Delete/5
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var user = await _context.Users
			.FirstOrDefaultAsync(m => m.Id == id);
		if (user == null)
		{
			return NotFound();
		}

		return View(user);
	}

	// POST: Users/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var user = await _context.Users.FindAsync(id);
		_context.Users.Remove(user);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool UserExists(int id)
	{
		return _context.Users.Any(e => e.Id == id);
	}
    public IActionResult Login()
    {
        Debug.WriteLine("login page opened");
        return View();
    }
}
