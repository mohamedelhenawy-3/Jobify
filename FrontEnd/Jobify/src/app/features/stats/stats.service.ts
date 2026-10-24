import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PlatformStats } from './model/state';

@Injectable({
  providedIn: 'root'
})
export class StatsService {

    private http = inject(HttpClient);
  private url = `https://localhost:7026/api/stats`;

  getStats(): Observable<PlatformStats> {
    return this.http.get<PlatformStats>(this.url);
  }
}
