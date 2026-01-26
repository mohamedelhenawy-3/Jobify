import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-employeregister',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './employeregister.component.html',
  styleUrl: './employeregister.component.css'
})
export class EmployeregisterComponent {

  
  constructor(private fb:FormBuilder){

  }

   registerform = this.fb.group({
     FullName:new FormControl(''),
     Email:new FormControl(''),
     password:new FormControl(''),
     ConfirmPassword:new FormControl(''),
   });
}
