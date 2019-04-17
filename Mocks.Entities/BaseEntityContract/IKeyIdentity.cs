namespace Mocks.Entities.BaseEntityContract
{
    public interface IKeyIdentity<TKey>
    {
        TKey Id { get; set; }
    }
}
