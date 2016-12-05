using System;
using System.Linq;
using System.Collections.Generic;
using KitchenResponsible.Model;
using KitchenResponsible.Data;
using KitchenResponsible.Utils.DateAndTime;

namespace KitchenResponsible.Services {
    public class KitchenResponsibleService : IKitchenResponsibleService {
        readonly ITrondheimRepository repository;
        readonly IWeekNumberFinder weekNumberFinder;

        public KitchenResponsibleService(ITrondheimRepository trondheimRepository, IWeekNumberFinder weekNumberFinder) {
            this.repository = trondheimRepository;  
            this.weekNumberFinder = weekNumberFinder;   
        }

        public ResponsibleForWeek GetEmployeeForWeek() {
            var week = weekNumberFinder.GetIso8601WeekOfYear();
            var weeksWithResponsible = repository.GetWeeksWithResponsible();
            
            var weeksToDelete = RemovePastWeeks(week, weeksWithResponsible);
            if (weeksToDelete.Count > 0) {
                AddNewWeeks(weeksToDelete, weeksWithResponsible);
            }

            var responsible = weeksWithResponsible.Single(w => w.WeekNumber == week).Responsible;
            var upNext = weeksWithResponsible.Single(w => w.WeekNumber == WeekNumberFinder.GetNextWeek(week)).Responsible;
            return new ResponsibleForWeek(week, responsible, upNext);
        }

        private IReadOnlyList<Week> RemovePastWeeks(ushort week, IReadOnlyList<Week> weeksWithResponsible) {
            var weeksToDelete = new List<Week>();
            var previousWeek = WeekNumberFinder.GetPreviousWeek(week);
            Week weekWithResponsible;
            while((weekWithResponsible = weeksWithResponsible.SingleOrDefault(w => w.WeekNumber == previousWeek)).Responsible != null) {
                weeksToDelete.Add(weekWithResponsible);
                previousWeek = WeekNumberFinder.GetPreviousWeek(previousWeek);
            }

            return weeksToDelete;
        }

        private void AddNewWeeks(IReadOnlyList<Week> weeksToDelete, IReadOnlyList<Week> weeksWithResponsible) {              
            var newResponsiblesForWeeks = new Week[weeksToDelete.Count];
            ushort lastWeek = 0;
            int prev = 0;
            if (weeksWithResponsible.Last().WeekNumber == 52) {
                for (int i = 0; i < 52; ++i) {
                    if (weeksWithResponsible[i].WeekNumber - prev > 1) {
                        lastWeek = weeksWithResponsible[i - 1].WeekNumber;
                        break;
                    } else {
                        prev++;
                    }
                }
            } else {
                lastWeek = weeksWithResponsible.Last().WeekNumber;
            }

            for (int i = 0; i < weeksToDelete.Count; i++) {
                lastWeek = WeekNumberFinder.GetNextWeek(lastWeek);
                newResponsiblesForWeeks[i] = new Week(lastWeek, weeksToDelete[i].Responsible);
            }
            
            repository.RemovePastWeeksAndAddNewOnces(weeksToDelete.Select(w => w.WeekNumber).ToArray(), newResponsiblesForWeeks);
        }
    }
}