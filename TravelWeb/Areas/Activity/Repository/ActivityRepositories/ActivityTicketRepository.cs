using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Repository.IActivityRepositories;

namespace TravelWeb.Areas.Activity.Repository.ActivityRepositories
{
    public class ActivityTicketRepository : IActivityTicketReposiotry
    {
        private readonly ActivityDbContext _dbContext;
        public ActivityTicketRepository(ActivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ActivityTicketViewModel>> GetAsync()
        {
            var vm = await _dbContext.AcitivityTickets
                .Select(t => new ActivityTicketViewModel
                {
                    ProductCode = t.ProductCode,
                    ProductName = t.ProductName,
                    TicketCategoryName = t.TicketCategory!.CategoryName,
                    StartDate = t.StartDate,
                    ExpiryDate = t.ExpiryDate,
                    CurrentPrice = t.CurrentPrice,
                    Status = t.Status,
                })
                .ToListAsync();
            return vm;
        }

        public async Task<ActivityTicketViewModel> GetByProductCodeAsync(string ProductCode)
        {
            var product = await _dbContext.AcitivityTickets
                .Where(p => p.ProductCode == ProductCode)
                .Select(p => new ActivityTicketViewModel
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    TicketCategoryName = p.TicketCategory!.CategoryName,
                    CurrentPrice = p.CurrentPrice,
                    Status = p.Status,
                    StartDate = p.StartDate,
                    ExpiryDate = p.ExpiryDate,
                    ProdcutDescription = p.ActivityTicketDetail!.ProdcutDescription,
                    TermsOfService = p.ActivityTicketDetail.TermsOfService,
                    AcivityId = p.ActivityTicketDetail.ActivityId,
                })
                .FirstOrDefaultAsync();

            return product!;
        }

        public async Task CreateAsync(ActivityTicketViewModel vm)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ActivityTicketViewModel vm)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangeAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
