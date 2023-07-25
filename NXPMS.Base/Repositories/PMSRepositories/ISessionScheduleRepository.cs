using Microsoft.Extensions.Configuration;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Base.Models.PMSModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface ISessionScheduleRepository
    {
        IConfiguration _config { get; }

        #region Session Schedule Write Action Methods
        Task<bool> AddAsync(SessionSchedule sessionSchedule);
        Task<bool> CancelAsync(int sessionScheduleId, string cancelledBy);
        Task<bool> DeleteAsync(int sessionScheduleId);

        #endregion

        #region Employee Session Schedule Read Action Methods
        Task<List<SessionActivityType>> GetForAllAsync(int reviewSessionId);
        Task<List<SessionActivityType>> GetForLocationAsync(int reviewSessionId, int locationId);
        Task<List<SessionActivityType>> GetForDepartmentAsync(int reviewSessionId, string departmentCode);
        Task<List<SessionActivityType>> GetForUnitAsync(int reviewSessionId, string unitCode);
        Task<List<SessionActivityType>> GetForEmployeeAsync(int reviewSessionId, int employeeId);
        #endregion

        #region Session Schedule Read Action Methods
        Task<IList<SessionSchedule>> GetAllAsync(int reviewSessionId);
        Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, string departmentCode);
        Task<IList<SessionSchedule>> GetByDepartmentCodeAsync(int reviewSessionId, string departmentCode, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByEmployeeCardinalsAsync(int reviewSessionId, SessionActivityType activityType, EmployeeCardinal employeeCardinal);
        Task<IList<SessionSchedule>> GetByIdAsync(int sessionScheduleId);
        Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId);
        Task<IList<SessionSchedule>> GetByLocationIdAsync(int reviewSessionId, int locationId, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId);
        Task<IList<SessionSchedule>> GetByReviewSessionIdAsync(int reviewSessionId, SessionActivityType activityType);
        Task<IList<SessionSchedule>> GetByTypeAsync(int reviewSessionId, SessionScheduleType scheduleType);
        Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, string unitCode);
        Task<IList<SessionSchedule>> GetByUnitCodeAsync(int reviewSessionId, string unitCode, SessionActivityType activityType);
        #endregion
    }
}