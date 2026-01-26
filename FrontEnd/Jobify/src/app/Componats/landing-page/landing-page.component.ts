import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css'] // ✅ fixed
})
export class LandingPageComponent {

  constructor(private router: Router) {} // ✅ Angular Router


}
