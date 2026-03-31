import { Component, EventEmitter, OnInit, output, Output } from '@angular/core';
import { AuthserviceService } from '../../Services/AuthService/authservice.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, NgModel } from '@angular/forms';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink,FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
 isLoggedIn: boolean = false;
  userRole: string | null = null;

keyword:string ='';
@Output () searchEvent=new EventEmitter<string>();
    constructor(private auth: AuthserviceService  ,private router:Router) {}

ngOnInit(): void {

  this.auth.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.auth.getUserRole(); // 'Admin' or 'User'

  

      // Fetch initial count once (optional if already triggered from MenuComponent)
    });

}

onSearch() {
  if (!this.keyword.trim()) return;

  this.router.navigate(['/Jobify/jobsearch'], {
    queryParams: { keyword: this.keyword }
  });
}
 logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

}
