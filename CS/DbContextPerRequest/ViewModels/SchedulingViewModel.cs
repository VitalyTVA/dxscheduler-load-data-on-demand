using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Scheduling;
using DXSample.Data;

namespace DXSample.ViewModels {
    public class SchedulingViewModel : EventToCommandShowCases.ViewModelBase {
        public SchedulingViewModel() {
            using(var dbContext = new SchedulingContext()) {
                Resources = dbContext.ResourceEntities.ToList();
            }
        }
        public virtual List<ResourceEntity> Resources { get; set; }
        [Command]
        public object CreateSourceObject(ItemType itemType) {
            if(itemType == ItemType.AppointmentItem)
                return new AppointmentEntity();
            return null;
        }

        [Command]
        public object[] FetchAppointments(Expression<Func<AppointmentEntity, bool>> fetchExpression) {
            using(var dbContext = new SchedulingContext()) {
                return dbContext.AppointmentEntities.Where(fetchExpression).ToArray();
            }
        }

        [Command]
        public void ProcessChanges(IList<AppointmentEntity> addToSource, IList<AppointmentEntity> updateInSource, IList<AppointmentEntity> deleteFromSource) {
            using(var dbContext = new SchedulingContext()) {
                dbContext.AppointmentEntities.AddRange(addToSource);
                foreach(var appt in updateInSource)
                    AppointmentEntityHelper.CopyProperties(appt, dbContext.AppointmentEntities.Find(appt.Id));
                foreach(var appt in deleteFromSource)
                    dbContext.AppointmentEntities.Remove(dbContext.AppointmentEntities.Find(appt.Id));
                dbContext.SaveChanges();
            }
        }
    }
}