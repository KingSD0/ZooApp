using System.Threading.Tasks;

namespace ZooApp.Services
{
    /// <summary>
    /// Service voor dierentuinbrede acties.
    /// </summary>
    public interface IZooService
    {
        /// <summary>Overzicht van dieren bij zonsopkomst.</summary>
        Task<object> GetSunriseOverviewAsync();

        /// <summary>Overzicht van dieren bij zonsondergang.</summary>
        Task<object> GetSunsetOverviewAsync();

        /// <summary>Voedingsinfo per verblijf + waarschuwingen.</summary>
        Task<object> GetFeedingOverviewAsync();

        /// <summary>Checkt of verblijven voldoen aan eisen.</summary>
        Task<object> GetConstraintOverviewAsync();

        /// <summary>Wijs dieren automatisch toe (strategie: 'nieuw' of 'aanvullen').</summary>
        Task AutoAssignAsync(string strategy);
    }
}
