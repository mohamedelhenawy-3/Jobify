import { Component } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-jobseekerregister',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './jobseekerregister.component.html',
  styleUrl: './jobseekerregister.component.css'
})
export class JobseekerregisterComponent {
  constructor(private fb:FormBuilder){

  }
   registerform = this.fb.group({
     FullName:new FormControl(''),
     Email:new FormControl(''),
     password:new FormControl(''),
     ConfirmPassword:new FormControl(''),
   });

  }

  
