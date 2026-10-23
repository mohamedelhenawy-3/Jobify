import { Component, ElementRef, EventEmitter, HostListener, OnInit, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { Observable, filter, map } from 'rxjs';
import { AuthserviceService } from '../../Services/AuthService/authservice.service';
import { UserSummary } from '../../Models/User/user';
import { NAV_ITEMS, NavGroup } from '../../features/Nav-Bar-Items/nav-items';
import { UserService } from '../../Services/User/user.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  @Output() searchEvent = new EventEmitter<string>();
  @ViewChild('searchBox') searchBox!: ElementRef<HTMLElement>;

  // navigation
  navItems$: Observable<NavGroup[]>;
  openMenu: string | null = null;
  mobileOpen = false;

  // auth / user
  isLoggedIn = false;
  profileRoute = '/Jobify/ownseekerprofile';
  imgFailed = false;
  user$: Observable<UserSummary | null>;      // comes from UserService (name + photo)

  // search
  keyword = '';
  isFocused = false;
  showSuggestions = false;
  filteredSuggestions: string[] = [];
  private allSuggestions: string[] = [        // replace with your real suggestions if you have them
    'Angular Developer', '.NET Developer', 'Frontend Engineer', 'Backend Engineer', 'UI/UX Designer'
  ];

  constructor(
    private authService: AuthserviceService,
    private userService: UserService,
    private router: Router
  ) {
    this.user$ = this.userService.user$;

    this.navItems$ = this.authService.isLoggedIn$.pipe(
      map(loggedIn => loggedIn
        ? NAV_ITEMS[this.authService.getUserRole() ?? 'JobSeeker'] ?? []
        : [])
    );

    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => { this.openMenu = null; this.mobileOpen = false; });
  }

  ngOnInit(): void {
    this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      this.imgFailed = false;

      if (loggedIn) {
        this.userService.load();                          // fetch name + photo
        this.profileRoute = this.authService.getUserRole() === 'Employer'
          ? '/Jobify/employer/profile'
          : '/Jobify/ownseekerprofile';
      } else {
        this.userService.clear();                         // wipe data when logged out
      }
    });
  }

  // dropdowns
  toggle(label: string, event: Event): void {
    event.stopPropagation();
    this.openMenu = this.openMenu === label ? null : label;
  }
  toggleMobile(event: Event): void {
    event.stopPropagation();
    this.mobileOpen = !this.mobileOpen;
    this.openMenu = null;
  }
  @HostListener('document:click', ['$event'])
  onDocumentClick(e: Event): void {
    this.openMenu = null;
    if (this.searchBox && !this.searchBox.nativeElement.contains(e.target as Node)) {
      this.showSuggestions = false;
    }
  }
  @HostListener('document:keydown.escape')
  onEsc(): void { this.openMenu = null; this.mobileOpen = false; }

  // search
  onInputChange(): void {
    const k = this.keyword.trim().toLowerCase();
    this.filteredSuggestions = k
      ? this.allSuggestions.filter(s => s.toLowerCase().includes(k)).slice(0, 6)
      : [];
    this.showSuggestions = true;
  }
  onFocus(): void { this.isFocused = true; this.onInputChange(); }
  onBlur(): void  { this.isFocused = false; }
onSearch(): void {
  const keyword = this.keyword.trim();
  this.showSuggestions = false;
  if (!keyword) return;

  this.searchEvent.emit(keyword); // optional, keep if a parent listens

  this.router.navigate(['/Jobify/searchsystem'], {
    queryParams: { keyword }
  });
}
  selectSuggestion(item: string): void { this.keyword = item; this.onSearch(); }

  // user helpers
  firstName(full: string): string { return (full ?? '').trim().split(' ')[0]; }
  initials(full: string): string {
    const p = (full ?? '').trim().split(' ').filter(Boolean);
    return ((p[0]?.[0] ?? '') + (p[1]?.[0] ?? '')).toUpperCase();
  }

  logout(): void {
    this.userService.clear();
    this.authService.logout();            // use your original logout call if it's named differently
    this.router.navigate(['/login']);
  }
}