namespace Infrastructure.PersistenceModels
{
    public record UserEntity (int ID,
                              string DNI,
                              string Nombres,
                              string Apellidos,
                              string Correo)
    {
    }
}
