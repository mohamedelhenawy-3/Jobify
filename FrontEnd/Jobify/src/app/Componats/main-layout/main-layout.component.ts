import { Component } from '@angular/core';
import { SideBarComponent } from "../side-bar/side-bar.component";
import { NavbarComponent } from "../navbar/navbar.component";
import { Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [SideBarComponent, NavbarComponent, SideBarComponent, NavbarComponent,RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {
  constructor(private router: Router) {}

onSearch(keyword: string) {
  this.router.navigate([''], {
    queryParams: { keyword }
  });
}
}
