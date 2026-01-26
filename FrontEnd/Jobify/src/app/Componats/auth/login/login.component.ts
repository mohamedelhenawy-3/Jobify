import { Component } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

    constructor(private fb:FormBuilder){
  
    }
  
     registerform = this.fb.group({
       FullName:new FormControl(''),
       Email:new FormControl(''),
       password:new FormControl(''),
       ConfirmPassword:new FormControl(''),
     });
}
