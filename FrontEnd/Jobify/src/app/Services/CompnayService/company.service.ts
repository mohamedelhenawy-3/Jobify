import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CompanyOwner } from '../../Models/Company/Owner';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

 
   private baseUrl = 'https://localhost:44309/api/Company';
 
   constructor(private http: HttpClient) {}

     getMyProfile(): Observable<CompanyOwner> {
       return this.http.get<CompanyOwner>(`${this.baseUrl}/privatrcompany`);
     }
}
