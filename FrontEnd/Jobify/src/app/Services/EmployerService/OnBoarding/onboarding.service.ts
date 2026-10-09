import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { EmployerOnboardingStatus } from '../../../Models/OnBoarding/OnboardingStatus';
import { CompanySearchDto } from '../../../Models/OnBoarding/CompanySearch';
import { ProcessJoinRequestDto } from '../../../Models/OnBoarding/ProcessJoinRequest';
import { JoinCompanyRequestDto } from '../../../Models/OnBoarding/JoinCompanyRequest';

@Injectable({
  providedIn: 'root'
})
export class OnboardingService {

  private baseUrl = 'https://localhost:7026/api';

  private statusSubject = new BehaviorSubject<EmployerOnboardingStatus | null>(null);
  public status$ = this.statusSubject.asObservable();

  constructor(private http: HttpClient) {}
  
  
  
    // 🔹 Get logged-in user's profile
CreateCompany(companyData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Onboarding`, companyData);
  }

  getOnboardingStatus(): Observable<EmployerOnboardingStatus> {
    return this.http.get<EmployerOnboardingStatus>(`${this.baseUrl}/Onboarding/status`).pipe(
      tap(status => this.statusSubject.next(status))
    );
  }

searchCompanies(query: string = ''): Observable<CompanySearchDto[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<CompanySearchDto[]>(`${this.baseUrl}/Onboarding/search-companies`, { params });
  }

  // POST: /api/onboarding/join-company
  joinCompany(companyId: number): Observable<{ message: string }> {
    const body: JoinCompanyRequestDto = { companyId };
    return this.http.post<{ message: string }>(`${this.baseUrl}/Onboarding/join-company`, body);
  }

  // POST: /api/onboarding/process-join-request (For Company Owners)
  processJoinRequest(dto: ProcessJoinRequestDto): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.baseUrl}/Onboarding/process-join-request`, dto);
  }

  get currentStatusValue(): EmployerOnboardingStatus | null {
    return this.statusSubject.value;
  }
    
}
