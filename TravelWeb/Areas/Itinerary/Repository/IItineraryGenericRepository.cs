namespace TravelWeb.Areas.Itinerary.Repository
{
    public interface IItineraryGenericRepository<table> where table : class
    {
        public IQueryable<table> GetAll();



        public table GetById<Type>(Type id);



        public void Add(table Entity);



        public void Update(table Entity);


        public void Delete<Type>(Type id);



        public bool SaveChanges();

    }
}
