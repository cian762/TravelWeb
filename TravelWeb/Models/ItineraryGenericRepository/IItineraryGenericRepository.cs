namespace TravelWeb.Models.ItineraryGenericRepository
{
    public interface IItineraryGenericRepository<Table> where Table : class
    {
        public IEnumerable<Table> GetAll();
        public Table GetById<Type>(Type id);
        public void Add(Table Entity);
        public void Update(Table Entity);
        public void Delete<Type>(Type id);
        public bool SaveChanges();
    }
}
