import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from "@angular/router";
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { Console } from 'console';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterLink,ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
 userType: 'Employer' | 'JobSeeker' = 'JobSeeker';
  loading = false;
  errorMessage = '';

 
  registerForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthserviceService,
    private router: Router

  ) {

     this.registerForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['JobSeeker'],

    // JobSeeker
    fullName: [''],
    phoneNumber: [''],

    // Employer
    companyName: [''],
    companyWebsite: ['']
  });

  }
  switch(type: 'JobSeeker' | 'Employer') {
    this.userType = type;
    this.registerForm.patchValue({ role: type });

    if (type === 'JobSeeker') {
      this.registerForm.get('companyName')?.reset();
      this.registerForm.get('companyWebsite')?.reset();
    } else {
      this.registerForm.get('fullName')?.reset();
      this.registerForm.get('phoneNumber')?.reset();
    }
  }

  submit() {
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.errorMessage = '';

    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.loading = false;
        console.log(this.registerForm.value);
        this.router.navigate(['/login']);
        alert('Suucefully registeration');
      },
      error: (err) => {
                console.log(err);

        this.loading = false;
        this.errorMessage = err.error?.message || 'Registration failed';
      }
    });
  }

}
