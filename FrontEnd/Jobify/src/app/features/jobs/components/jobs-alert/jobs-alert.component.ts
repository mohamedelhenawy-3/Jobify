import { Component, OnInit } from '@angular/core';
import { JobAlert } from '../../models/Job';
import { JobalertService } from '../../Service/jobalert.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-jobs-alert',
  standalone: true,
  imports: [FormsModule,CommonModule,RouterLink],
  templateUrl: './jobs-alert.component.html',
  styleUrl: './jobs-alert.component.css'
})
export class JobsAlertComponent implements OnInit {
readonly max = 10;                 // same limit as the backend

  alerts: JobAlert[] = [];
  loading = false;
  error = '';
  toast = '';

  keyword = '';
  adding = false;
  addError = '';

  target: JobAlert | null = null;
  deleting = false;
  modalError = '';


    constructor(private api: JobalertService) {}

  ngOnInit() { this.load(); }

  get limitReached() { return this.alerts.length >= this.max; }

  load() {
    this.loading = true;
    this.error = '';
    this.api.mine().subscribe({
      next: r => { this.alerts = r; this.loading = false; },
      error: e => {
        this.error = e.status === 401
          ? 'Log in to manage your alerts.'
          : e.error?.message ?? 'Could not load your alerts.';
        this.loading = false;
      }
    });
  }
    toggle(a: JobAlert) {
    const next = !a.isActive;
    a.isActive = next;
    this.api.setActive(a.id, next).subscribe({
      next: r => this.showToast(r.message),
      error: e => {
        a.isActive = !next;
        this.showToast(typeof e.error === 'string' ? e.error : 'Could not update the alert.');
      }
    });
  }

  askDelete(a: JobAlert) { this.target = a; this.modalError = ''; }
  close() { if (!this.deleting) this.target = null; }

  confirmDelete() {
    if (!this.target) return;
    const id = this.target.id;
    this.deleting = true;
    this.modalError = '';

    this.api.delete(id).subscribe({
      next: r => {
        this.alerts = this.alerts.filter(x => x.id !== id);
        this.deleting = false;
        this.target = null;
        this.showToast(r.message);
      },
      error: e => {
        this.deleting = false;
        this.modalError = typeof e.error === 'string' ? e.error : e.error?.message ?? 'Something went wrong.';
      }
    });
  }

  private showToast(msg: string) {
    this.toast = msg;
    setTimeout(() => this.toast = '', 2400);
  }

}
