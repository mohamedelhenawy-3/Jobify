import { Component, OnInit } from '@angular/core';
import { CompanyOwner } from '../../../Models/Company/Owner';
import { CompanyService } from '../../../Services/CompnayService/company.service';
import { CommonModule } from '@angular/common';
import { JobService } from '../../../Services/JobService/job.service';
import { OpeningJobs } from '../../../Models/Job/openjobsmodel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-company-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './company-profile.component.html',
  styleUrl: './company-profile.component.css'
})
export class CompanyProfileComponent implements OnInit{


  openingjobs:OpeningJobs[]=[];
   profile!: CompanyOwner;
    loading = true;
  
    constructor(private companyservice: CompanyService,private jobservice:JobService,private router:Router) {}
  
  ngOnInit(): void {
 this.companyservice.getMyProfile().subscribe({
        next: (res) => {
          this.profile = res;
          console.log(this.profile);
          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }  }
      );

      this.loadopenjobs();
    }
   loadopenjobs(){
  this.jobservice.getopeningjobs().subscribe({
    next:(res)=>{
      console.log(res);
      this.openingjobs=res;
    }
  })
   }
  

   filters: string[] = ['All', 'Full Time', 'Part Time', 'Remote'];
activeFilter: string = 'All';

get filteredJobs(): OpeningJobs[] {
  if (this.activeFilter === 'All') return this.openingjobs;

  return this.openingjobs.filter(j => {
    // adjust based on your backend IDs
    if (this.activeFilter === 'Remote') return j.workModeId === 1;
    if (this.activeFilter === 'Full Time') return j.employmentTypeId === 1;
    if (this.activeFilter === 'Part Time') return j.employmentTypeId === 2;

    return true;
  });
}

setFilter(filter: string) {
  this.activeFilter = filter;
}
View(jobid:number){
 this.router.navigate(['/Jobify/details',jobid]);
}
}
