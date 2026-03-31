import { Component, OnInit } from '@angular/core';
import { EmployerProfileResponseDto } from '../../../Models/Employer/EmployerProfileResponse';
import { EmployerService } from '../../../Services/EmployweService/employer.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-employerprofile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './employerprofile.component.html',
  styleUrl: './employerprofile.component.css'
})
export class EmployerprofileComponent implements OnInit {
 profile!: EmployerProfileResponseDto;
  loading = true;

  constructor(private employerService: EmployerService,private router:Router) {}
  ngOnInit(): void {
    this.employerService.getMyProfile().subscribe({
      next: (res) => {
        this.profile = res;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
  view(){
   this.router.navigate(['/Jobify/managejobs'])
  }

}
