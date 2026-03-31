import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LookupService } from '../../Services/LookUpService/lookup.service';
import { JobService } from '../../Services/JobService/job.service';
import { CommonEngine } from '@angular/ssr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-createjob',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './createjob.component.html',
  styleUrl: './createjob.component.css'
})
export class CreatejobComponent  implements OnInit {
  jobForm!: FormGroup;

  industries: any[] = [];
  employmentTypes: any[] = [];
  workModes: any[] = [];
  JobStatus:any[]=[];
  
  
  constructor(private Lookupservice:LookupService,private jobservice:JobService ,private fb:FormBuilder){
  }
  

  ngOnInit(): void {
  this.buildform()
    this.loadLookups();  
  }

  buildform(){
    this.jobForm=this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      location: ['', Validators.required],
      minSalary: [0, [Validators.required, Validators.min(0)]],
      maxSalary: [0, [Validators.required, Validators.min(0)]],
      industryId: [null, Validators.required],
      employmentTypeId: [null, Validators.required],
      workModeId: [null, Validators.required],
      responsibilities:['', Validators.required],
      requirements:['', Validators.required],  
      educationOrExperience:['', Validators.required],    
      statusId: [null,Validators.required] // Active by default
    })
  }
    loadLookups() {
    this.Lookupservice.getIndustries().subscribe(res => this.industries = res);
    this.Lookupservice.getEmploymentTypes().subscribe(res => this.employmentTypes = res);
    this.Lookupservice.getWorkModes().subscribe(res => this.workModes = res);
        this.Lookupservice.getStatuses().subscribe((res )=>{
          this.JobStatus = res;
          console.log(this.JobStatus);
        } );
  }
    submit() {
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    console.log('Submitting job:', this.jobForm.value);

    this.jobservice.createJob(this.jobForm.value).subscribe({
      next: () => {
        alert('Job posted successfully 🎉');
        this.jobForm.reset({ statusId: 3 });
      },
      error: err => console.error(err)
    });
  }
}
