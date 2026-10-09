import { Component } from '@angular/core';
import { SavedJob } from '../../models/saved-job';
import { SavedJobService } from '../../Service/savedjob.service';
import { PagedResult } from '../../models/Job';
import { PagerComponent } from '../../../Team/Components/pager/pager.component';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-saved-jobs',
  standalone: true,
  imports: [PagerComponent,CommonModule,RouterLink],
  templateUrl: './saved-jobs.component.html',
  styleUrl: './saved-jobs.component.css'
})
export class SavedJobsComponent {
 pageNumber = 1;
  pageSize = 8;

  result: PagedResult<SavedJob> | null = null;
  loading = false;
  error = '';
  toast = '';

  // remove confirm modal
  target: SavedJob | null = null;
  removing = false;
  modalError = '';

  constructor(private api: SavedJobService) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.error = '';
    this.api.list(this.pageNumber, this.pageSize).subscribe({
      next: r => { this.result = r; this.loading = false; },
      error: e => {
        this.error = e.status === 401
          ? 'Log in to see your saved jobs.'
          : e.error?.message ?? 'Could not load your saved jobs.';
        this.loading = false;
      }
    });
  }

  onPage(page: number) {
    this.pageNumber = page;
    this.load();
  }

  initials(title: string) {
    return title.split(' ').map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }

  askRemove(j: SavedJob) { this.target = j; this.modalError = ''; }
  close() { if (!this.removing) this.target = null; }

  confirmRemove() {
    if (!this.target) return;
    const t = this.target;
    this.removing = true;
    this.modalError = '';

    this.api.unsave(t.id).subscribe({
      next: () => {
        // if that was the last item on this page, go back one page
        if (this.result && this.result.items.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }
        this.removing = false;
        this.target = null;
        this.showToast('Removed from saved jobs');
        this.load();
      },
      error: e => {
        this.removing = false;
        this.modalError = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Something went wrong.';
      }
    });
  }

  private showToast(msg: string) {
    this.toast = msg;
    setTimeout(() => this.toast = '', 2200);
  }
}
