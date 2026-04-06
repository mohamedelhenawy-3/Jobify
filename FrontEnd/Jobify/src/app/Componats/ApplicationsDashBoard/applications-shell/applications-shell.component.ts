import { Component, OnInit } from '@angular/core';
import { ApplicationfiltersComponent } from "../applicationfilters/applicationfilters.component";
import { ApplicationFilter } from '../../../Models/JobApplications/ApplicationsFilters/jobappfilters';
import { JobApplicationService } from '../../../Services/JobApplication/jopapplicationservice.service';
import { JobseekerapplicationsComponent } from "../../jobseekerapplications/jobseekerapplications.component";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { ApplicationsComponent } from "../../applications/applications.component";
import { Observable } from 'rxjs';

@Component({
  selector: 'app-applications-shell',
  standalone: true,
  imports: [ApplicationfiltersComponent, JobseekerapplicationsComponent, FormsModule, CommonModule, ApplicationsComponent],
  templateUrl: './applications-shell.component.html',
  styleUrl: './applications-shell.component.css'
})
export class ApplicationsShellComponent implements OnInit {
   loading:boolean=false;
   applications:any[]=[];
   currentFilters: ApplicationFilter = {
    search: '',
    status: 'all'
  }; 
  userRole: string | null = null; // To store role

  constructor(private applicationService: JobApplicationService,private authService:AuthserviceService) {}
  ngOnInit(): void {
 this.userRole = this.authService.getUserRole();
 
 this.loadData(); }


handleFilterChange(event: ApplicationFilter) {
  this.currentFilters = event;
  this.loadData();
}
loadData() {
  this.loading = true;
  

  let obs$: Observable<any>; 

  if (this.userRole === 'Employer') {
    obs$ = this.applicationService.getEmpApplications(this.currentFilters);
  } else {
    obs$ = this.applicationService.MyApplications(this.currentFilters);
  }

  obs$.subscribe({
    next: (data) => {
      this.applications = data;
      console.log('Loaded applications:', this.applications);
      this.loading = false;
    },
    error: (err) => {
      console.error('Failed to load applications', err);
      this.loading = false;
    }
  });
}

}
