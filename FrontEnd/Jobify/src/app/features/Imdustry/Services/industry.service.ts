import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Industry, PopularIndustry } from '../Model/industry.model';

@Injectable({ providedIn: 'root' })
export class IndustryService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7026/api/Industries';

  getAll(includeInactive = false): Observable<Industry[]> {
    const params = new HttpParams().set('includeInactive', includeInactive);
    return this.http.get<Industry[]>(this.baseUrl, { params });
  }


  getPopular(take = 6) {
  const params = new HttpParams().set('take', take);
  return this.http.get<PopularIndustry[]>(`${this.baseUrl}/popular`, { params });
}
}