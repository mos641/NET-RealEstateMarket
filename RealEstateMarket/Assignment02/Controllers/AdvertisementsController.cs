using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstateMarket.Data;
using RealEstateMarket.Models;
using RealEstateMarket.Models.ViewModels;
using Azure.Storage.Blobs;
using Azure;
using System.Diagnostics;
using System.ComponentModel;

namespace RealEstateMarket.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        private readonly long _fileSizeLimit = 20000000;
        private readonly string[] permittedExtensions = { ".png", ".jpg", ".jpeg", ".gif" };
        private readonly string container = "adimages";

        public AdvertisementsController(MarketDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string? Id)
        {
            if (Id != null)
            {
                // find the brokerage
                var brokerage = await _context.Brokerages
                .Include(b=>b.Advertisements)
                .SingleAsync(b => b.Id == Id);

                return View(brokerage);

            }
            else
            {
                return NotFound();
            }
        }


        // GET: Advertisements/Create
        public async Task<IActionResult> Create(string? Id)
        {
            if (Id != null)
            {
                // find the brokerage
                var brokerage = await _context.Brokerages
                .SingleAsync(b => b.Id == Id);

                return View(brokerage);
            }
            else
            {
                return NotFound();
            }
        }

        [BindProperty]
        public Advertisement Advertisement { get; set; }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, string brokerageId)
        {
            // perform validation checks
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                String message = "File extension must be one of ";
                foreach (string extension in permittedExtensions)
                {
                    message += extension + " ";
                }
                // Debug.WriteLine(message);
                throw new Exception(message);
            }

            if (file.Length > _fileSizeLimit || file.Length == 0)
            {
                // Debug.WriteLine("File size must be greater than 0 bytes and upto 20MB");
                throw new Exception("File size must be greater than 0 bytes and upto 20MB");
            }

            // create container client to attach to a container
            BlobContainerClient containerClient;
            try
            {
                // attempt to create a new container and make public
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(container);
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                // container already exists
                containerClient = _blobServiceClient.GetBlobContainerClient(container);
            }

            try
            {
                string filePath = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(filePath);
                // if blob exists already throw an error
                if (await blockBlob.ExistsAsync())
                {
                    throw new Exception("Blob with specified url already exists");
                }

                Advertisement.FileName = filePath;
                Advertisement.Url = blockBlob.Uri.AbsoluteUri;

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }


            }
            catch (RequestFailedException)
            {
                throw new Exception("Image blob already exists");
            }

            // find the brokerage
            var brokerage = await _context.Brokerages
            .Include(b=>b.Advertisements)
            .SingleAsync(b => b.Id == brokerageId);
            
            // add brokerage to the advertisement
            Advertisement.Brokerage = brokerage;

            // add advertisement to brokerage
            brokerage.Advertisements.Add(Advertisement);

            // update in context
            _context.Advertisements.Add(Advertisement);
            _context.Brokerages.Update(brokerage);

            // save context
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = brokerageId });
        }


        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Advertisements == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .Include(a => a.Brokerage)
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }


        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int AdId)
        {
            // find the advertisement
            var advertisement = await _context.Advertisements
            .Include(b => b.Brokerage)
            .SingleAsync(b => b.AdId == AdId);
            var brokerageId = advertisement.Brokerage.Id;

            BlobContainerClient containerClient;
            try
            {
                // get the container and store container object
                containerClient = _blobServiceClient.GetBlobContainerClient(container);
            }
            catch (RequestFailedException)
            {
                throw new Exception("Container does not exist");
            }

            try
            {
                await containerClient.DeleteBlobAsync(advertisement.FileName);
            }
            catch (RequestFailedException)
            {
                throw new Exception("Specified blob does not exist");
            }

            // find the brokerage and remove ad
            //var brokerage = await _context.Brokerages
            //.Include(b => b.Advertisements)
            //.SingleAsync(b => b.Id == brokerageId);

            // brokerage.Advertisements.Remove(advertisement);

            // update in context
            // _context.Brokerages.Update(brokerage);
            _context.Advertisements.Remove(advertisement);

            await _context.SaveChangesAsync();


            return RedirectToAction("Index", new { id = brokerageId });
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.AdId == id);
        }
    }
}
