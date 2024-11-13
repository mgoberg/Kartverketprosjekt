﻿using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services
{
    public interface ISakService
    {
        public Task<List<SakModel>> GetUserCasesAsync(string userEmail);

        public Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId);

        public Task<bool> DeleteCaseAsync(int id);
    }
}
