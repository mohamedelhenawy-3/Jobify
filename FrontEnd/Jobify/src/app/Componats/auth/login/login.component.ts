import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,RouterLink,CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginform!:FormGroup;
    constructor(private fb:FormBuilder,private authservice:AuthserviceService,private router:Router){
     this.loginform = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    }
  login() {
    if (this.loginform.invalid) {
      this.loginform.markAllAsTouched();
      return;
    }

    this.authservice.login(this.loginform.value).subscribe({
      next: () => {
        alert('Login successful');
        this.router.navigate(['/home']);
      },
      error: () => {
        alert('Email or Password Invalid');
      }
    });
  }
}
