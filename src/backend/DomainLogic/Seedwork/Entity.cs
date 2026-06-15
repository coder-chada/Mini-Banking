namespace DomainLogic.Seedwork
{
    public class Entity
    {
        public int Id { get; set; }

        protected Entity()
        {
            
        }

        protected Entity(int id)
        {
            this.Id = id;
        }
    }
}
