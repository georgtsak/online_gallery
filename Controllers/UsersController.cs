using OnlineGallery.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using OnlineGallery.Data;


namespace OnlineGallery.Controllers
{
	public class UsersController : Controller
	{
		private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;
		private readonly AppDbContext _dbContext; // Προσθήκη του DbContext

		public UsersController(AppDbContext dbContext) // Constructor για Injection
		{
			_dbContext = dbContext;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Register(UserModel model)
		{
			if (ModelState.IsValid)
			{
				// Ελέγχουμε αν ο χρήστης υπάρχει ήδη στη βάση
				var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError("Email", "Αυτό το email χρησιμοποιείται ήδη.");
					return View(model);
				}

				// Κρυπτογράφηση κωδικού
				Hash_Password(model);

				// Προσθήκη χρήστη στη βάση
				_dbContext.Users.Add(model);
				_dbContext.SaveChanges();

				// Ανακατεύθυνση στη σελίδα σύνδεσης μετά την επιτυχή εγγραφή
				return RedirectToAction("Login");
			}

			return View(model);
		}

		// GET:Users/Login
		public IActionResult Login()
		{
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CookieAuth");
			return RedirectToAction("Index", "Home");
		}

		private string Create_Salt()
		{
			var salt = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(salt);
		}

		private void Hash_Password(UserModel user)
		{
			user.Salt = Create_Salt();
			byte[] pass = Encoding.ASCII.GetBytes(user.Password);
			var hash = Rfc2898DeriveBytes.Pbkdf2(pass, Convert.FromBase64String(user.Salt), 10, hashAlgorithm, 64);
			user.Password = Convert.ToBase64String(hash);
		}

		private bool Verify_Password(string givenPass, UserModel user)
		{
			var derivedHash = Rfc2898DeriveBytes.Pbkdf2(givenPass, Convert.FromBase64String(user.Salt), 10, hashAlgorithm, 64);
			return CryptographicOperations.FixedTimeEquals(derivedHash, Convert.FromBase64String(user.Password));
		}
	}
}
