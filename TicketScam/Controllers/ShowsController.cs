using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using TicketScam.Models;
using TicketScam.Services;



namespace TicketScam.Controllers
{
    [Authorize]
    public class ShowsController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly BlobContainerClient _containerClient;

        private readonly TicketScamContext _context;
      
        private readonly IImageService _imageService;

        public ShowsController(TicketScamContext context, IImageService imageService, IConfiguration configuration)
        {
            _context = context;
            _imageService = imageService;
            _configuration = configuration;

            //blob
            var connectionString = _configuration["AzureStorage"];
            var containerName = "showpics";
            _containerClient = new BlobContainerClient(connectionString, containerName);
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
        // POST: Shows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowId,CategoryId,VenueId,ImageFile,ShowDate,ShowDescription")] Show show)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (show.ImageFile != null && show.ImageFile.Length > 0)
                {
                    // Save to local file system (if you still want this)
                    show.ImageFileName = await _imageService.SaveImageAsync(show.ImageFile);

                    // Upload to Azure Blob Storage
                    string blobName = Guid.NewGuid().ToString() + Path.GetExtension(show.ImageFile.FileName);

                    var blobClient = _containerClient.GetBlobClient(blobName);

                    using (var stream = show.ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = show.ImageFile.ContentType });
                    }

                    // Get URL of the uploaded blob file
                    string fileURL = blobClient.Uri.ToString();
                    show.ImageFileName = fileURL; // Store the blob URL
                }

                _context.Add(show);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            // If something failed; redisplay form with dropdowns
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
            if (show.ImageFile != null && show.ImageFile.Length > 0)
            {
                // Delete old blob if it exists
                if (!string.IsNullOrEmpty(show?.ImageFileName))
                {
                    // Extract blob name from URL or store separately
                    var oldBlobName = show.ImageFileName; // You might need to adjust this
                    var oldBlobClient = _containerClient.GetBlobClient(oldBlobName);
                    await oldBlobClient.DeleteIfExistsAsync();
                }

                // Upload new blob (same code as Create method)
                string blobName = Guid.NewGuid().ToString() + Path.GetExtension(show.ImageFile.FileName);
                var blobClient = _containerClient.GetBlobClient(blobName);

                using (var stream = show.ImageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = show.ImageFile.ContentType });
                }

                show.ImageFileName = blobClient.Uri.ToString();
                show.ImageFileName = blobName;
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
            if (!string.IsNullOrEmpty(show.ImageFileName))
            {
                var blobClient = _containerClient.GetBlobClient(show.ImageFileName);
                await blobClient.DeleteIfExistsAsync();
                _imageService.DeleteImage(show.ImageFileName); 
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