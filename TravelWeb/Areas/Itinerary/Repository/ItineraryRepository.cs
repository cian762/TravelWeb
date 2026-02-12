using Microsoft.EntityFrameworkCore;

namespace TravelWeb.Areas.Itinerary.Repository
{
    public class ItineraryRepository<table> : IItineraryGenericRepository<table> where table : class
    {
        private readonly ItineraryDBContext.TravelContext _context;
        private readonly DbSet<table> _dbset;
        public ItineraryRepository(ItineraryDBContext.TravelContext context)
        {
            _context = context;
            _dbset = _context.Set<table>();
        }

        public IEnumerable<table> GetAll()
        {
            return _dbset;
        }

        public table GetById<Type>(Type id)
        {
            return _dbset.Find(id);
        }

        public void Add(table Entity)
        {
            _dbset.Add(Entity);
        }

        public void Update(table Entity)
        {
            _dbset.Update(Entity);
        }

        public void Delete<Type>(Type id)
        {
            _dbset.Remove(GetById(id));
        }

        public bool SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch { return false; }
        }
    }
}
