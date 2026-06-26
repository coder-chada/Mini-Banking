namespace Infrastructure.PersistenceModels
{
    public class EntityPersistenceModel
    {
        public int RowVersion { get; private set; } = default!;
    }
}
