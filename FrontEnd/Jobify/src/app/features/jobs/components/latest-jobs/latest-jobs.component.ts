import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { LatestJob } from '../../models/Job';
import { JobService } from '../../Service/job.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-latest-jobs',
  standalone: true,
  imports: [DatePipe, DecimalPipe, RouterLink],
  templateUrl: './latest-jobs.component.html',
  styleUrl: './latest-jobs.component.css'
})
export class LatestJobsComponent implements OnInit {
skeletons: any;
isNew(_t37: LatestJob) {
throw new Error('Method not implemented.');
}
load() {
throw new Error('Method not implemented.');
}
  private jobsService = inject(JobService);

  jobs = signal<LatestJob[]>([]);
  loading = signal(true);
  error = signal(false);

  ngOnInit(): void {
    this.jobsService.getLatest(6).subscribe({
      next: (data) => { this.jobs.set(data); this.loading.set(false); },
      error: () => { this.error.set(true); this.loading.set(false); }
    });
  }

  private readonly STORAGE_KEY = 'saved_jobs';
savedIds = signal<Set<number>>(this.loadSaved());

isSaved(id: number): boolean {
  return this.savedIds().has(id);
}

toggleSave(id: number): void {
  const next = new Set(this.savedIds());
  next.has(id) ? next.delete(id) : next.add(id);
  this.savedIds.set(next);
  localStorage.setItem(this.STORAGE_KEY, JSON.stringify([...next]));
  // later: call your backend, e.g. this.jobsService.toggleSave(id).subscribe()
}

private loadSaved(): Set<number> {
  try {
    return new Set<number>(JSON.parse(localStorage.getItem(this.STORAGE_KEY) ?? '[]'));
  } catch {
    return new Set<number>();
  }
}
}
