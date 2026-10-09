import { Routes } from '@angular/router';
import { SeekerProfileComponent } from './Components/seeker-profile/seeker-profile.component';


export const SeekerProfile_ROUTES: Routes = [

  {
    path: 'ownseekerprofile',
    component: SeekerProfileComponent
  }
  // job-details-page, job-create-page, job-edit-page will be added here later
];