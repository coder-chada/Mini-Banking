namespace Infrastructure.PersistenceModels
{
    public class IdempotencyEntity
    {
        public string idempotency_key { get; set; } = string.Empty;
        public string request_hash { get; set; } = string.Empty;
        public int status { get; set; }
        public string? response_body { get; set; } = string.Empty;
        public int status_code { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime completed_at { get; set; }
        public DateTime expires_at { get; set; }

        public IdempotencyEntity()
        {
            this.created_at = DateTime.UtcNow;
        }
    }
}
