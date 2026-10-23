import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-team-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './team-layout.component.html',
  styleUrl: './team-layout.component.css'
})
export class TeamLayoutComponent {
  tabs = [
    { label: 'Members', link: 'members' },
    { label: 'Join requests', link: 'join-requests' },
  ];
}
