using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketScam.Models;
using TicketScam.Services;

namespace TicketScam.Controllers
{
    [Authorize]
    public class ShowsController : Controller
    {
        private readonly TicketScamContext _context;
        private readonly IImageService _imageService;

        public ShowsController(TicketScamContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            var shows = await _context.Show
                .Include(s => s.Venue)
                .Include(s => s.Category)
                .ToListAsync();
            return View(shows);
        }

        // GET: Shows/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            return View();
        }

        // POST: Shows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Show show)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (show.ImageFile != null && show.ImageFile.Length > 0)
                {
                    show.ImageFileName = await _imageService.SaveImageAsync(show.ImageFile);
                }

                _context.Add(show);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; redisplay form with dropdowns
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", show.VenueId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", show.CategoryId);
            return View(show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", show.VenueId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", show.CategoryId);
            return View(show);
        }

        // POST: Shows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Show show)
        {
            if (id != show.ShowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing show to preserve current image if no new file uploaded
                    var existingShow = await _context.Show.AsNoTracking().FirstOrDefaultAsync(s => s.ShowId == id);

                    if (show.ImageFile != null && show.ImageFile.Length > 0)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingShow?.ImageFileName))
                        {
                            _imageService.DeleteImage(existingShow.ImageFileName);
                        }
                        // Save new image
                        show.ImageFileName = await _imageService.SaveImageAsync(show.ImageFile);
                    }
                    else
                    {
                        // Keep existing image
                        show.ImageFileName = existingShow?.ImageFileName;
                    }

                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.ShowId))
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

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", show.VenueId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", show.CategoryId);
            return View(show);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .Include(s => s.Venue)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Show.FindAsync(id);
            if (show != null)
            {
                // Delete associated image file
                if (!string.IsNullOrEmpty(show.ImageFileName))
                {
                    _imageService.DeleteImage(show.ImageFileName);
                }

                _context.Show.Remove(show);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Show.Any(e => e.ShowId == id);
        }
    }
}