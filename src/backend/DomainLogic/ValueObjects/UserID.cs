namespace DomainLogic.ValueObjects
{
    public sealed class UserID
    {
        public int Value { get; private set; }

        public UserID(int value)
        {
            this.Value = value;
        }
    }
}
