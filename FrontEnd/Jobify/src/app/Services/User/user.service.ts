import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, of } from 'rxjs';
import { UserSummary } from '../../Models/User/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {



private apiUrl = 'https://localhost:7026/api/User';
    constructor(private http: HttpClient) {}

  // Get user profile by ID
  getUserProfile(userId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${userId}`);
  }

   private readonly url = 'https://localhost:7026/api/User/me/summary';
 
  private readonly _user$ = new BehaviorSubject<UserSummary | null>(null);
  readonly user$ = this._user$.asObservable();
 
  load(): void {
    this.http.get<UserSummary>(this.url)
      .pipe(catchError(() => of(null)))
      .subscribe(u => this._user$.next(u));
  }
 
  /** On logout. */
  clear(): void {
    this._user$.next(null);
  }
}
