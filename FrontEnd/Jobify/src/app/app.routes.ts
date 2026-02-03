import { Routes } from '@angular/router';
import { LandingPageComponent } from './Componats/landing-page/landing-page.component';
import { EmployeregisterComponent } from './Componats/auth/employer/employeregister/employeregister.component';
import { JobseekerregisterComponent } from './Componats/auth/jobseeker/jobseekerregister/jobseekerregister.component';
import { LoginComponent } from './Componats/auth/login/login.component';
import { RegisterComponent } from './Componats/auth/register/register.component';
import { HomeComponent } from './Componats/home/home.component';
import { JobListPageComponentComponent } from './Componats/JobComponents/job-list-page-component/job-list-page-component.component';
import { JobspagesComponent } from './Componats/JobComponents/jobspages/jobspages.component';
import { CreatejobComponent } from './Componats/createjob/createjob.component';
import { ManagejobsComponent } from './Componats/managejobs/managejobs.component';
import { JobdetailsComponent } from './Componats/jobdetails/jobdetails.component';
import { SeekerprofileComponent } from './Componats/JoobSeeker/seekerprofile/seekerprofile.component';
import { UpdateJobComponent } from './Componats/update-job/update-job.component';
import { ApplicationsComponent } from './Componats/applications/applications.component';
import { ApplicationDetailsComponent } from './Componats/application-details/application-details.component';
import { CreatejobapplicationComponent } from './Componats/createjobapplication/createjobapplication.component';

export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    { path: 'register/employer', component: EmployeregisterComponent },
    { path: 'register/job-seeker', component: JobseekerregisterComponent }, 
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent },
    { path: 'home', component: HomeComponent },
    { path: 'findjob', component: JobspagesComponent },
    { path: 'createjob', component: CreatejobComponent },
    { path: 'managejobs', component: ManagejobsComponent },
    {path:'details/:id',component:JobdetailsComponent},
    { path: 'apply/:id', component: CreatejobapplicationComponent }, // new route


    {path:'seeker',component:SeekerprofileComponent},
    {path:'updateJob/:id',component:UpdateJobComponent},

    {path:'jobapplications',component:ApplicationsComponent},
     {path: 'ApplicationDetails/:id', component: ApplicationDetailsComponent },
  // other routes...

];
