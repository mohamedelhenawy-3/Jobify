import { Component, OnInit } from '@angular/core';
import { SidebarItem } from '../../Models/SideBar/sidebar';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthserviceService } from '../../Services/AuthService/authservice.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [CommonModule,RouterModule],
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.css']
})
export class SideBarComponent implements OnInit {

  userRole: string = ''; // will be set from token
  active: string = '';
  sidebarItems: SidebarItem[] = [];
   isLoggedIn$: Observable<boolean>;

  constructor(private router: Router, private authService: AuthserviceService) {
    // Listen for route changes to highlight active menu item
    this.isLoggedIn$ = this.authService.isLoggedIn$; // tracks if user is logged in

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.setActive(event.urlAfterRedirects);
      }
    });
  }
  ngOnInit(): void {
    // Subscribe to login state and set sidebar dynamically
    this.authService.isLoggedIn$.subscribe(loggedIn => {
      if (loggedIn) {
        const role = this.authService.getUserRole() ?? 'JobSeeker';
        this.userRole = role;
        this.setSidebarItems();
      } else {
        // clear sidebar if logged out
        this.sidebarItems = [];
      }
    });
  }

  setSidebarItems(): void {
    if (this.userRole === 'JobSeeker') {
      this.sidebarItems = [
        { label: 'home', route: '/Jobify/home', icon: 'fas fa-home' },
        { label: 'Find Jobs', route: '/Jobify/jobs', icon: 'fas fa-search' },
        { label: 'My Applications', route: '/Jobify/seeker-applications', icon: 'fas fa-clipboard-list' },
        { label: 'Saved Jobs', route: '/Jobify/SavedJobs', icon: 'fas fa-bookmark' },
        { label: 'Alert Jobs', route: '/Jobify/AlertJobs', icon: 'fas fa-bell' },
        { label: 'Profile', route: '/Jobify/ownseekerprofile', icon: 'fas fa-user' },
        { label: 'Company Profile', route: '/Jobify/mycompany', icon: 'fas fa-building' },

      ];
    } else if (this.userRole === 'Employer') {
      this.sidebarItems = [
        { label: 'Dashboard', route: '/Jobify/employerdashboard', icon: 'fas fa-home' },
        { label: 'Find Jobs', route: '/Jobify/jobs', icon: 'fas fa-search' },
        { label: 'My Jobs', route: '/Jobify/employerjobs', icon: 'fas fa-briefcase' },
        { label: 'Company Profile', route: '/Jobify/mycompany', icon: 'fas fa-building' },
        { label: 'Profile', route: '/Jobify/employer/profile', icon: 'fas fa-building' }

        
      ];
    }
  }

  setActive(url: string): void {
    const foundItem = this.sidebarItems.find(item => url.includes(item.route));
    this.active = foundItem ? this.getKeyFromRoute(foundItem.route) : '';
  }

  getKeyFromRoute(route: string): string {
    return route.replace('/', '');
  }
}
