using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Linq;
using System.Threading.Tasks;

public class AdminActionsController : Controller
{
    private readonly AppDbContext _context;

    public AdminActionsController(AppDbContext context)
    {
        _context = context;
    }

    private bool IsNotAdmin()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("UserRole");

        return userId == null || role != "Admin";
    }

    public async Task<IActionResult> Index()
    {
        if (IsNotAdmin())
            return RedirectToAction("Login", "Users");

        var usersWithHistory = await _context.Users
            .Where(u => !u.IsDeleted && u.Role != Role.Admin)
            .Select(u => new
            {
                User = u,
                IsCurrentlyBanned = _context.AdminActions
                    .Where(a => a.UserId == u.Id)
                    .OrderByDescending(a => a.ActionDate)
                    .Select(a => a.ActionType == ActionType.Ban)
                    .FirstOrDefault(),
                History = _context.AdminActions
                    .Where(a => a.UserId == u.Id)
                    .OrderByDescending(a => a.ActionDate)
                    .ToList()
            })
            .ToListAsync();

        var model = usersWithHistory.Select(u => new BanStatusModel
        {
            User = u.User,
            IsCurrentlyBanned = u.IsCurrentlyBanned,
            History = u.History
        }).ToList();

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Ban(int id)
    {
        if (IsNotAdmin())
            return RedirectToAction("Login", "Users");

        var admin_id = HttpContext.Session.GetInt32("UserId");
        if (admin_id == null)
        {
            return RedirectToAction("Login", "Users");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var action = new AdminModel
        {
            UserId = user.Id,
            AdminId = admin_id.Value,
            ActionType = ActionType.Ban,
            ActionDate = DateTime.UtcNow
        };

        _context.AdminActions.Add(action);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Unban(int id)
    {
        if (IsNotAdmin())
            return RedirectToAction("Login", "Users");

        var admin_id = HttpContext.Session.GetInt32("UserId");
        if (admin_id == null)
        {
            return RedirectToAction("Login", "Users");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var action = new AdminModel
        {
            UserId = user.Id,
            AdminId = admin_id.Value,
            ActionType = ActionType.Unban,
            ActionDate = DateTime.UtcNow
        };

        _context.AdminActions.Add(action);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
