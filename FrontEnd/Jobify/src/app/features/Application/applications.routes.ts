import { Routes } from '@angular/router';
import { SeekerApplicationsComponent } from './Components/seeker-applications/seeker-applications.component';
import { EmployerApplicationsComponent } from './Components/employer-applications/employer-applications.component';
import { ApplicationDetailsComponent } from './Components/application-details/application-details.component';
import { CreatejobapplicationComponent } from './Components/createjobapplication/createjobapplication.component';

export const APPLICATIONS_ROUTES: Routes = [
  {
    path: 'seeker-applications',
    component: SeekerApplicationsComponent
  },
  {
    path: 'employer-applications',
    component: EmployerApplicationsComponent
  },
  {
    path: 'apply/:id',
    component: CreatejobapplicationComponent
  },
  {
    path: 'employer-applications-details/:id',
    component: ApplicationDetailsComponent
  }
  // job-details-page, job-create-page, job-edit-page will be added here later
];