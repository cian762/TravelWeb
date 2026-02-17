using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Collections.Generic;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;
using static System.Net.Mime.MediaTypeNames;

namespace TravelWeb.Areas.TripProduct.Services.Implementation
{
    
    public class STripItineraryItem : ITripItineraryItem
    {
        private readonly TripDbContext _context;
        public STripItineraryItem(TripDbContext context) 
        {
         _context = context;
        }
        public async Task<IEnumerable<ViewModelTripItineraryItems>> IGetAny(int tripProductId)
        {
          var q=  _context.TripItineraryItems
                .Include(t=>t.Attraction)
                .Include(t=>t.Resource)
                .Include(t=>t.Activity)
                .Where(t=>t.TripProductId==tripProductId);
         return  await q.Select(s => new ViewModelTripItineraryItems {
                DayNumber=s.DayNumber,
                SortOrder=s.SortOrder,
                TripProductId=tripProductId,
             ResourceName = s.Attraction != null ? s.Attraction.Name:
               s.Activity != null ? s.Activity.Title:
               s.Resource != null ? s.Resource.ResourceName:""
            }).ToListAsync();
   
        }

        public async Task<bool> ICreate(ViewModelTripItineraryItems items)
        {
            var q = new TripItineraryItem();
            q.DayNumber = items.DayNumber;
            q.SortOrder = items.SortOrder;
            q.TripProductId = items.TripProductId;
            q.CustomText = items.CustomText;
            q.ActivityId = items.ActivityId;
            q.ResourceId = items.ResourceId;
            q.AttractionId = items.AttractionId;
           _context.TripItineraryItems.Add(q);
            return await _context.SaveChangesAsync()>0;
        }

        public Task<bool> IDelete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IUpdate(ViewModelTripItineraryItems items)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewModelTripItineraryItems> PrepareViewModel(int tripProductId)
        {
            var vm = new ViewModelTripItineraryItems();
            vm.TripProductId = tripProductId;
            vm.AttractionList = await _context.Attractions.Select(s=>new SelectListItem {
             Value=s.AttractionId.ToString(),
             Text = s.Name
            }).ToListAsync();
            vm.ActivityList=await _context.Activities.Select(s=>new SelectListItem { 
             Value =s.ActivityId.ToString(),
             Text = s.Title
            }).ToListAsync();
            var product = await _context.TripProducts.FindAsync(tripProductId);
            vm.MaxDays = product?.DurationDays ?? 1;
            return vm;
        }  
    }
}
