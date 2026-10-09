import { Component, EventEmitter, OnInit, output, Output } from '@angular/core';
import { AuthserviceService } from '../../Services/AuthService/authservice.service';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, NgModel } from '@angular/forms';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink,FormsModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
 isLoggedIn: boolean = false;
  userRole: string | null = null;
private sub?: Subscription;
keyword:string ='';
searchText = '';

@Output () searchEvent=new EventEmitter<string>();
    constructor(private auth: AuthserviceService  ,private router:Router) {}

ngOnInit(): void {
this.syncFromUrl();                                    // when the page first loads
  this.sub = this.router.events
    .pipe(filter(e => e instanceof NavigationEnd))
    .subscribe(() => this.syncFromUrl());
  this.auth.isLoggedIn$.subscribe(isLogged => {
    this.isLoggedIn = isLogged;
    this.userRole = this.auth.getUserRole(); // 'Admin' or 'User'

  

      // Fetch initial count once (optional if already triggered from MenuComponent)
    });

}
ngOnDestroy() { this.sub?.unsubscribe(); }

private syncFromUrl() {
  const tree = this.router.parseUrl(this.router.url);
  const path = tree.root.children['primary']?.segments.map(s => s.path).join('/').toLowerCase();
  // keep the input in sync with the URL when on the search page, clear it elsewhere
  this.keyword = path === 'jobify/searchsystem' ? (tree.queryParams['keyword'] ?? '') : '';
}

onSearch() {
  const keyword = this.keyword.trim();
  if (!keyword) return;

  this.showSuggestions = false;
  this.router.navigate(['/Jobify/searchsystem'], {
    queryParams: { keyword, type: 'all' }
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
