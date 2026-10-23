import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboarderviceService {

 private apiUrl = 'https://localhost:7026/api/JobSeekerDashBoard/me';

  constructor(private http: HttpClient) { }

  getDashboard(): Observable<any> {
    return this.http.get<any>(this.apiUrl);
  }
}
