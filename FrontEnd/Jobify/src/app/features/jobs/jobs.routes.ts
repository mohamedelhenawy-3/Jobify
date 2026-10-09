import { Routes } from '@angular/router';
import { CreateJobComponent } from './components/create-job/create-job.component';
import { JobDetailsPageComponent } from './components/job-details-page/job-details-page.component';
import { EmployerJobsComponent } from './components/employer-jobs-compoents/employer-jobs/employer-jobs.component';
import { JobsListComponent } from './components/jobs-list/jobs-list.component';
import { UpdateJobComponent } from './components/update-job/update-job.component';
import { SavedJobsComponent } from './components/saved-jobs/saved-jobs.component';
import { JobsAlertComponent } from './components/jobs-alert/jobs-alert.component';


export const JOBS_ROUTES: Routes = [

    {
    path: 'jobs',
    component: JobsListComponent
  },
  {
    path: 'Jobcreate',
    component: CreateJobComponent
  },
   {
    path: 'SavedJobs',
    component: SavedJobsComponent
  },
     {
    path: 'AlertJobs',
    component: JobsAlertComponent
  },
   {
    path: 'jobs/:id',
    component: JobDetailsPageComponent
  },
  {
    path: 'employerjobs',
    component: EmployerJobsComponent
  }  , {
    path: 'update/:id',
    component: UpdateJobComponent
  }
  
  // job-details-page, job-create-page, job-edit-page will be added here later
];