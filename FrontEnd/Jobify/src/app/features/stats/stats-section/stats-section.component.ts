import { Component, inject, signal } from '@angular/core';
import { PlatformStats } from '../model/state';
import { StatsService } from '../stats.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stats-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stats-section.component.html',
  styleUrl: './stats-section.component.css'
})
export class StatsSectionComponent {
 private statsService = inject(StatsService);

  stats = signal<PlatformStats | null>(null);
  error = signal(false);

  ngOnInit(): void {
    this.statsService.getStats().subscribe({
      next: (data) => this.stats.set(data),
      error: () => this.error.set(true)
    });
  }
}
