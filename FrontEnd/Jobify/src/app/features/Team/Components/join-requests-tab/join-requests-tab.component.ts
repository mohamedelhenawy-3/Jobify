import { Component } from '@angular/core';
import { JoinRequestFilter, JoinRequestItem, JoinStatus } from '../../Models/join-request';
import { PagedResult } from '../../Models/team';
import { JoinRequestServiceService } from '../../Services/onboarding/join-request-service.service';
import { PagerComponent } from '../pager/pager.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-join-requests-tab',
  standalone: true,
  imports: [PagerComponent,CommonModule,FormsModule],
  templateUrl: './join-requests-tab.component.html',
  styleUrl: './join-requests-tab.component.css'
})
export class JoinRequestsTabComponent {

  statuses: JoinStatus[] = ['Pending', 'Approved', 'Rejected'];

  filter: JoinRequestFilter = { status: 'Pending', pageNumber: 1, pageSize: 5 };
  result: PagedResult<JoinRequestItem> | null = null;
  loading = false;
  error = '';

  // confirm modal
  target: JoinRequestItem | null = null;
  approving = true;
  saving = false;
  modalError = '';


   constructor(private api: JoinRequestServiceService) {}

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.error = '';
    this.api.list(this.filter).subscribe({
      next: (r) => { this.result = r; this.loading = false; },
      error: (e) => {
        this.error = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Could not load requests.';
        this.loading = false;
      }
    });
  }
  setStatus(s: JoinStatus) {
    this.filter = { ...this.filter, status: s, pageNumber: 1 };
    this.load();
  }

  onPage(page: number) {
    this.filter = { ...this.filter, pageNumber: page };
    this.load();
  }

  initials(name: string) {
    return name.split(' ').map(p => p[0]).slice(0, 2).join('').toUpperCase();
  }

 open(r: JoinRequestItem, approve: boolean) {
    this.target = r; this.approving = approve; this.modalError = '';
}
close() { if (!this.saving) this.target = null; }
confirm() {
    if (!this.target) return;
    const t = this.target;
    this.saving = true; this.modalError = '';

  
    this.api.process(t.id, this.approving).subscribe({
      next: () => this.afterProcess(),
      error: e => {
        this.saving = false;
        this.modalError = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Something went wrong.';
      }
    });
   }
    private afterProcess() {
    if (this.result && this.result.items.length === 1 && this.filter.pageNumber > 1) {
      this.filter = { ...this.filter, pageNumber: this.filter.pageNumber - 1 };
    }
    this.saving = false;
    this.target = null;
    this.load();
  }

}
