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

    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Where(u => !u.IsDeleted && u.Role != Role.Admin)
            .ToListAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> Ban(int id)
    {
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
