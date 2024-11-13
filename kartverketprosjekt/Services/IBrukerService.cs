﻿using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services
{
    public interface IBrukerService
    {
        public Task<bool> RegisterUserAsync(RegistrerModel model);

        public Task<bool> UpdateNameAsync(string navn);

        public Task<bool> UpdatePasswordAsync(string password);
    }
}
