
import { Routes } from '@angular/router';
import { PublicProfilesComponent } from './public-profiles/public-profiles.component';
import { PublicCompanyComponent } from './public-profiles/Company/public-company/public-company.component';

export const  PUBLICPROFILES_ROUTES: Routes = [
  {
    path: 'public-profiles/:id',
    component: PublicProfilesComponent
  },
    {
    path: 'public-Profiles-Comapny/:id',
    component: PublicCompanyComponent
  },

];