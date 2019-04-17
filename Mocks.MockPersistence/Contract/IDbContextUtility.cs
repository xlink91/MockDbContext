namespace Mocks.MockPersistence.Contract
{
    public interface IDbContextUtility<TKey>
    {
        TKey GenerateNewKey();
    }
}
