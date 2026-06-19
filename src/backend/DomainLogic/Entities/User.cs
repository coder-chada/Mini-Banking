using DomainLogic.Exceptions;
using DomainLogic.Seedwork;

namespace DomainLogic.Entities
{

    public class User : Entity
    {
        public string DNI { get; private set; } = string.Empty;
        public string Nombres { get; private set; } = string.Empty;
        public string Apellidos { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        // For ORM rehydration
        public User(int id, string dni, string nombres, string apellidos, string email) : base(id)
        {
            SetDNI(dni);
            SetNombres(nombres);
            SetApellidos(apellidos);
            setEmail(email);
        }

        public User(string dni,
                    string nombres,
                    string apellidos,
                    string email)
        {
            SetDNI(dni);
            SetNombres(nombres);
            SetApellidos(apellidos);
            setEmail(email);
        }

        private void setEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Email is required.", nameof(email));

            this.Email = email;
        }

        private void SetApellidos(string apellidos)
        {
            if (string.IsNullOrWhiteSpace(apellidos))
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Apellidos is required.", nameof(apellidos));

            this.Apellidos = apellidos;
        }

        private void SetNombres(string nombres)
        {
            if (string.IsNullOrWhiteSpace(nombres))
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "Nombres is required.", nameof(nombres));

            this.Nombres = nombres;
        }

        private void SetDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new DomainLogicException(DomainLogicErrorCode.EntityInvalidData, "DNI is required.", nameof(dni));

            this.DNI = dni;
        }
    }
}