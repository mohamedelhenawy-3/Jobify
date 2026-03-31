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
import { SideBarComponent } from './Componats/side-bar/side-bar.component';
import { MainLayoutComponent } from './Componats/main-layout/main-layout.component';
import { JobseekerapplicationsComponent } from './Componats/jobseekerapplications/jobseekerapplications.component';
import { SavedjobsComponent } from './Componats/savedjobs/savedjobs.component';
import { JobseekerdashboardComponent } from './Componats/jobseekerdashboard/jobseekerdashboard.component';
import { EmployerdashboardComponent } from './Componats/employerdashboard/employerdashboard.component';
import { EmployerprofileComponent } from './Componats/EmployerProfile/employerprofile/employerprofile.component';
import { CompanyProfileComponent } from './Componats/Company/company-profile/company-profile.component';
import { JobsearchComponent } from './Componats/jobsearch/jobsearch.component';

export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    
      { path: 'Jobify',
      component: MainLayoutComponent,
     children: [
        { path: 'applications', component: ApplicationsComponent },
        {path: 'jobsearch', component: JobsearchComponent },
        {path:'seeker',component:SeekerprofileComponent},
        { path: 'home', component: HomeComponent },
        { path: 'findjob', component: JobspagesComponent },
        { path: 'createjob', component: CreatejobComponent },
        { path: 'managejobs', component: ManagejobsComponent },
        {path:'jobapplications',component:ApplicationsComponent},
        {path: 'ApplicationDetails/:id', component: ApplicationDetailsComponent },
        {path:'updateJob/:id',component:UpdateJobComponent},
        {path:'details/:id',component:JobdetailsComponent},
        { path: 'apply/:id', component: CreatejobapplicationComponent },
        { path: 'myapplications', component: JobseekerapplicationsComponent },
        { path: 'savedjobs', component: SavedjobsComponent },
        { path: 'jobseekerdashboard', component: JobseekerdashboardComponent },
        { path: 'employerdashboard', component: EmployerdashboardComponent },
        { path: 'employerprofile', component: EmployerprofileComponent },
        { path: 'mycompany',component :CompanyProfileComponent}





   
       ]
     },
    {path:'sidebar',component:SideBarComponent}, 
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent },


    // {path:'updateJob/:id',component:UpdateJobComponent},

    // {path:'jobapplications',component:ApplicationsComponent},
    //  {path: 'ApplicationDetails/:id', component: ApplicationDetailsComponent },
  // other routes...

];
