namespace Infrastructure.PersistenceModels
{
    public record AccountEntity(int ID,
                                string Numero,
                                int Tipo,
                                int Currency,
                                int OwnerID,
                                decimal Balance)
    {   
    }
}
