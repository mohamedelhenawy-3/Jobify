import { Routes } from '@angular/router';
import { LandingPageComponent } from './Componats/landing-page/landing-page.component';
import { EmployeregisterComponent } from './Componats/auth/employer/employeregister/employeregister.component';
import { JobseekerregisterComponent } from './Componats/auth/jobseeker/jobseekerregister/jobseekerregister.component';
import { LoginComponent } from './Componats/auth/login/login.component';
import { RegisterComponent } from './Componats/auth/register/register.component';
import { HomeComponent } from './Componats/home/home.component';
import { JobListPageComponentComponent } from './Componats/JobComponents/job-list-page-component/job-list-page-component.component';
import { JobspagesComponent } from './Componats/JobComponents/jobspages/jobspages.component';

export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    { path: 'register/employer', component: EmployeregisterComponent },
    { path: 'register/job-seeker', component: JobseekerregisterComponent }, 
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent },
    { path: 'home', component: HomeComponent },
       { path: 'findjob', component: JobspagesComponent },

];
