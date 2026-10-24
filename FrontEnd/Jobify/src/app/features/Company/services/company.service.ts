import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { SuccessResponse } from '../../../Models/Job/SuccessResponse';
import { CreateJobRequest } from '../../jobs/models/Job';
import { CompanyMembersPreview, HiringSnapshot, MyCompany, TopHiringCompany, UpdateCompany } from '../models/company';


@Injectable({ providedIn: 'root' })
export class CompanyService {
  private baseUrl = `https://localhost:7026/api`;

  constructor(private http: HttpClient) {}
getMyCompany(): Observable<MyCompany> {
  return this.http.get<MyCompany>(`${this.baseUrl}/CompanyMembers`);
}



updateMyCompany(formValue: any, logo?: File | null): Observable<MyCompany> {
  const fd = new FormData();
  fd.append('CompanyName', formValue.companyName ?? '');
  fd.append('CompanyWebsite', formValue.companyWebsite ?? '');
  fd.append('Description', formValue.description ?? '');
  fd.append('Location', formValue.location ?? '');
 if (formValue.industryId != null) {
    fd.append('industryId', String(formValue.industryId));   
  }  fd.append('CompanySize', formValue.companySize ?? '');
  fd.append('Founded', formValue.founded ?? '');
  if (logo) fd.append('Logo', logo);

  return this.http.put<MyCompany>(`${this.baseUrl}/CompanyMembers`, fd);
}
getHiringSnapshot(): Observable<HiringSnapshot> {
  return this.http.get<HiringSnapshot>(`${this.baseUrl}/company/hiring-snapshot`);
}


getMembersPreview(take = 5): Observable<CompanyMembersPreview> {
  return this.http.get<CompanyMembersPreview>(
    `${this.baseUrl}/CompanyMembers/members`, { params: { take } });
}

  getTopHiring(count = 8): Observable<TopHiringCompany[]> {
    const params = new HttpParams().set('count', count);
    return this.http.get<TopHiringCompany[]>(`${this.baseUrl}/company/top-hiring`, { params });
  }
}
