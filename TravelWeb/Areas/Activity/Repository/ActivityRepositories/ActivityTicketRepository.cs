using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
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
                .Include(t => t.TicketCategory)
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
                .AsNoTracking()
                .ToListAsync();
            return vm;
        }

        public async Task<List<AcitivityTicket>> GetByActivityIdAsync(int activityId)
        {
            var products = await _dbContext.AcitivityTickets
                .Include(t => t.TicketCategory)
                .Include(t => t.ActivityTicketDetail)
                .Where(t => t.ActivityTicketDetail!.ActivityId == activityId)
                .AsNoTracking()
                .ToListAsync();
            
            return products;
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
            // 新產品 ProductCode 定義
            var lastProductCode = await _dbContext.AcitivityTickets
                .OrderByDescending(p => p.ProductCode)
                .Select(p => p.ProductCode)
                .FirstOrDefaultAsync();

            string newProductCode;

            if (string.IsNullOrEmpty(lastProductCode))
            {
                newProductCode = "ACT-0001";
            }
            else
            {
                if (int.TryParse(lastProductCode.Substring(4), out int lastNumber))
                {
                    int nextNumber = lastNumber + 1;
                    newProductCode = $"ACT-{nextNumber:D4}";
                }
                else
                {
                    newProductCode = "ACT-0001";
                }
            }

            // 將 VM mapping 到 EF model

            var activtiyName = await _dbContext.Activities.Where(a => a.ActivityId == vm.AcivityId).Select(p => p.Title).FirstOrDefaultAsync();

            var product = new AcitivityTicket()
            {
                ProductCode = newProductCode,
                ProductName = $"{activtiyName} | {vm.TicketCategoryName}",
                TicketCategoryId = await _dbContext.TicketCategories.Where(t => t.CategoryName == vm.TicketCategoryName).Select(p => p.TicketCategoryId).FirstOrDefaultAsync(),
                StartDate = vm.StartDate,
                ExpiryDate = vm.ExpiryDate,
                CurrentPrice = vm.CurrentPrice,
                Status = vm.Status
            };

            var productDetail = new ActivityTicketDetail()
            {
                ProductCode = newProductCode,
                ActivityId = await _dbContext.Activities.Where(a => a.ActivityId == vm.AcivityId).Select(p => p.ActivityId).FirstOrDefaultAsync(),
                ProdcutDescription = vm.ProdcutDescription,
                TermsOfService = vm.TermsOfService
            };

            _dbContext.AcitivityTickets.Add(product);
            _dbContext.ActivityTicketDetails.Add(productDetail);
        }

        public async Task UpdateAsync(string ProductCode, ActivityTicketViewModel vm)
        {
            var OriginTicket = await _dbContext.AcitivityTickets
                .FirstOrDefaultAsync(t => t.ProductCode == vm.ProductCode);


            OriginTicket!.CurrentPrice = vm.CurrentPrice;
            OriginTicket!.Status = vm.Status;
            OriginTicket!.StartDate = vm.StartDate;
            OriginTicket!.ExpiryDate = vm.ExpiryDate;



            var OriginTicketDetail = await _dbContext.ActivityTicketDetails
                .FirstOrDefaultAsync(d => d.ProductCode == vm.ProductCode);

            OriginTicketDetail!.ProdcutDescription = vm.ProdcutDescription;
            OriginTicketDetail!.TermsOfService = vm.TermsOfService;

        }

        public async Task SaveChangeAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task<List<string>> TakeTicketCategoryNames()
        {
            return (await _dbContext.TicketCategories.Select(c => c.CategoryName).ToListAsync())!;
        }


    }
}
