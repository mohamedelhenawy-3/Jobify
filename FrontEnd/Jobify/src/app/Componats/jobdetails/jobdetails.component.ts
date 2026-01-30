import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { jobdetails } from '../../Models/Job/dteailsjob';
import { JobService } from '../../Services/JobService/job.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-jobdetails',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobdetails.component.html',
  styleUrl: './jobdetails.component.css'
})
export class JobdetailsComponent implements OnInit{
  loading: boolean = true;

  jobs!:jobdetails;
constructor(private router:ActivatedRoute,private jobservice:JobService){

}
  getJobIcon(job: jobdetails): string {
  const title = job.title?.toLowerCase() || '';

  // 1️⃣ Specific roles
  if (title.includes('full')) return 'fas fa-laptop-code';
  if (title.includes('backend')) return 'fas fa-server';
  if (title.includes('frontend')) return 'fas fa-code';
  if (title.includes('ai') || title.includes('ml')) return 'fas fa-robot';
  if (title.includes('data')) return 'fas fa-database';
  if (title.includes('devops')) return 'fas fa-tools';

  // 2️⃣ Industry-based fallback (optional)
  switch (job.industry) {
    case "Software": return 'fas fa-laptop-code'; // Software
    case "Business": return 'fas fa-chart-line';  // Business
    case "Manufacturing": return 'fas fa-industry';    // Manufacturing
  }

  // 3️⃣ Final fallback (ALWAYS exists)
  return 'fas fa-briefcase';
}
  ngOnInit(): void {
    const jobid=this.router.snapshot.paramMap.get('id');
    if(jobid){this.jobservice.getJobDetails(+jobid).subscribe(res=>{
      
      this.jobs=res;
              this.loading = false;

    console.log("jobdetails",res);})
    }
  }
}
