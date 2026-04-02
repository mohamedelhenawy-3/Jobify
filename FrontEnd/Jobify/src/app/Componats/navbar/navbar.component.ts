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

  this.router.navigate(['/Jobify/jobbashoard'], {
    queryParams: { keyword: this.keyword }
  });
}
isFocused = false;
showSuggestions = false;

suggestions: string[] = [
  'Backend Developer',
  'Frontend Developer',
  'Fullstack Developer',
  'Angular Developer',
  'Remote Jobs',
  'Java Developer'
];

filteredSuggestions: string[] = [];

onInputChange() {
  this.filteredSuggestions = this.suggestions.filter(x =>
    x.toLowerCase().includes(this.keyword.toLowerCase())
  );

  this.showSuggestions = true;
}

onFocus() {
  this.isFocused = true;
  this.showSuggestions = true;
}

onBlur() {
  this.isFocused = false;
  setTimeout(() => this.showSuggestions = false, 200);
}

selectSuggestion(value: string) {
  this.keyword = value;
  this.showSuggestions = false;
  this.onSearch();
}


 logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

}
