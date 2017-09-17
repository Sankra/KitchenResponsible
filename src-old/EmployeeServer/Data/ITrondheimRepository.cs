using System.Collections.Generic;
using KitchenResponsible.Model;

namespace KitchenResponsible.Data {
    public interface ITrondheimRepository {
        IReadOnlyList<string> GetNicks();
        IReadOnlyList<Week> GetWeeksWithResponsible();
        void RemovePastWeeksAndAddNewOnces(ushort[] passedWeeks, Week[] newWeeks);
    }
}