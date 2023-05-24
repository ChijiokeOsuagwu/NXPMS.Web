using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NXPMS.Base.Repositories.PMSRepositories
{
    public interface IReviewStageRepository
    {
        Task<IList<ReviewStage>> GetAllAsync();
        Task<IList<ReviewStage>> GetAllPreviousAsync(int currentStageId);
    }
}
