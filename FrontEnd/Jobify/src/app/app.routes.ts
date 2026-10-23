import { Routes } from '@angular/router';
import { LandingPageComponent } from './Componats/landing-page/landing-page.component';
import { LoginComponent } from './Componats/auth/login/login.component';
import { RegisterComponent } from './Componats/auth/register/register.component';
import { HomeComponent } from './Componats/home/home.component';

import { SideBarComponent } from './Componats/side-bar/side-bar.component';
import { MainLayoutComponent } from './Componats/main-layout/main-layout.component';
import { SavedjobsComponent } from './Componats/savedjobs/savedjobs.component';
import { JobseekerdashboardComponent } from './Componats/jobseekerdashboard/jobseekerdashboard.component';
import { EmployerdashboardComponent } from './Componats/employerdashboard/employerdashboard.component';
import { UserProfileComponent } from './Componats/user-profile/user-profile.component';
import { OnboardingComponent } from './Componats/employer/onboarding/onboarding.component';
import { CreatecompanyComponent } from './Componats/employer/createcompany/createcompany.component';
import { SearchcompanyComponent } from './Componats/employer/searchcompany/searchcompany.component';
import { PendingApprovalComponent } from './Componats/employer/pending-approval/pending-approval.component';
import { companyOnboardingGuard } from './Componats/auth/Guard/company-onboarding-guard.guard';
import { JoinCompanyComponent } from './Componats/employer/join-company/join-company.component';
import { JOBS_ROUTES } from './features/jobs/jobs.routes';
import path from 'path/win32';
import { OwnerProfileComponent } from './features/Owner-Profile/owner-profile/owner-profile.component';
import { COMPANY_ROUTES } from './features/Company/company.routes';
import { Search_ROUTES } from './features/SearchSystem/search.routes';
import { APPLICATIONS_ROUTES } from './features/Application/applications.routes';
import { TeamLayoutComponent } from './features/Team/Components/team-layout/team-layout.component';
import { Team_ROUTES } from './features/Team/team.routes';
import { SeekerProfile_ROUTES } from './features/SeekerProfile/seeker.route';
import { PUBLICPROFILES_ROUTES } from './features/PublicProfiles/publicprofiles.routes';

export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    
      { path: 'Jobify',
      component: MainLayoutComponent,
      children: [
         ...JOBS_ROUTES,
         ...COMPANY_ROUTES,
         ...Search_ROUTES,
         ...APPLICATIONS_ROUTES,
        ...Team_ROUTES,
        ...SeekerProfile_ROUTES,
        ...PUBLICPROFILES_ROUTES,

        { path: 'home', component: HomeComponent },
        { path: 'savedjobs', component: SavedjobsComponent },
        { path: 'jobseekerdashboard', component: JobseekerdashboardComponent },
        { path: 'employerdashboard',canActivate: [companyOnboardingGuard], component: EmployerdashboardComponent },
        {
          path: 'employer/profile',
          component: OwnerProfileComponent,
        },
        { path: 'user-profile/:id',component :UserProfileComponent}



   
       ]
     },
    {path:'sidebar',component:SideBarComponent}, 
    { path: 'register', component: RegisterComponent },
     { path: 'login', component: LoginComponent },



     { path: 'employer/onboarding', component: OnboardingComponent },
     { path: 'employer/pending-approval', component: PendingApprovalComponent },
     { path: 'employer/createcompany', component: CreatecompanyComponent },
     { path: 'employer/searchcompany', component: SearchcompanyComponent },
       { path: 'employer/JoinCompany', component: JoinCompanyComponent },
    // {path:'updateJob/:id',component:UpdateJobComponent},

    // {path:'jobapplications',component:ApplicationsComponent},
    //  {path: 'ApplicationDetails/:id', component: ApplicationDetailsComponent },
  // other routes...

];
