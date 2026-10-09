import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalSearchParams, GlobalSearchResponse } from '../Models/Search';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

 private baseUrl = `https://localhost:7026/api/Search`;

  constructor(private http: HttpClient) {}
 
  globalSearch(params: GlobalSearchParams): Observable<GlobalSearchResponse> {
    return this.http.get<GlobalSearchResponse>(this.baseUrl, {
      params: this.buildHttpParams(params)
    });
  }
 
  private buildHttpParams(params: GlobalSearchParams): HttpParams {
    let httpParams = new HttpParams();
 
    Object.entries(params).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        httpParams = httpParams.set(key, String(value));
      }
    });
 
    return httpParams;
  }
}
