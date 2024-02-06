namespace FinansoData.Data
{
    public interface ISeed
    {
        Task<bool> SeedCurrencies();
    }
}
